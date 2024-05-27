using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SnakeController : MonoBehaviour
{
    // Assuming you have a boolean variable 'alive'

    [SerializeField] private float moveSpeed;
    [SerializeField] private float bodySpeed;
    [SerializeField] private float steerSpeed;
    [SerializeField] private GameObject bodyPrefab;
    [SerializeField] private GameObject ground;
    public GameManager gm;
    private int gap = 10;
    private List<GameObject> bodyParts = new List<GameObject>();
    private List<Vector3> positionHistory = new List<Vector3>();
    private bool canDetectTrigger = false;
    private Dictionary<GameObject, float> bodyCreationTimes = new Dictionary<GameObject, float>();
    public StickController stickController;
    private Vector2 stickPosition;
    private Gyroscope gyro;


    void Start()
    {
        Debug.Log(gm.gyroscope);
        GrowSnake();
        GrowSnake();
        GrowSnake();
        GrowSnake();
        GrowSnake();
        GrowSnake();
        GrowSnake();
        
        gyro = Input.gyro;
        gyro.enabled = true;
        //positionHistory.Insert(0, transform.position);
        InvokeRepeating("UpdatePositionHistory", 0f, 0.01f);

        StartCoroutine(EnableTriggerDetection());
        stickController.StickChanged += OnStickChanged;
    }
    void OnStickChanged(object sender, StickEventArgs e)
    {
        stickPosition = e.Position;
    }

    void Update()
    {

        if (gm.play)
        {
            //move forward
            transform.position += transform.forward * moveSpeed * Time.deltaTime;

            //steer
            if (gm.gyroscope)
            {
                float steerDirection = -gyro.rotationRateUnbiased.z; // Use z-axis rotation for steering
                transform.Rotate(Vector3.up * steerDirection * steerSpeed * Time.deltaTime);
            }
            else
            {
                // Use the joystick to steer the snake
                float steerDirection = stickPosition.x;
                transform.Rotate(Vector3.up * steerDirection * steerSpeed * Time.deltaTime);
            }
            int index = 0;
            foreach (GameObject body in bodyParts)
            {
                Vector3 point = positionHistory[Math.Min(index * 10, positionHistory.Count - 1)];
                Vector3 moveDirection = point - body.transform.position;
                body.transform.position += moveDirection * bodySpeed * Time.deltaTime;

                body.transform.LookAt(point);

                index++;
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                GrowSnake();
            }

            // Check if the snake has gone past the borders of the ground
            if (transform.position.x > ground.transform.position.x + ground.transform.localScale.x / 2 ||
               transform.position.x < ground.transform.position.x - ground.transform.localScale.x / 2 ||
               transform.position.z > ground.transform.position.z + ground.transform.localScale.z / 2 ||
               transform.position.z < ground.transform.position.z - ground.transform.localScale.z / 2)
            {
                gm.alive = false;
            }
        }
    }
    IEnumerator EnableTriggerDetection()
    {
        // Wait for 3 seconds (you can adjust the time as needed)
        yield return new WaitForSeconds(3f);

        // Enable trigger detection after the delay
        canDetectTrigger = true;
    }

    void UpdatePositionHistory()
    {
        Debug.Log("UpdatePositionHistory");
        // Añadir la posición actual al inicio de la lista
        positionHistory.Insert(0, transform.position);

        // Si la lista excede el número máximo de posiciones, elimina la última
        if (positionHistory.Count > 500)
        {
            positionHistory.RemoveAt(positionHistory.Count - 1);
        }
    }

    private void GrowSnake()
    {
        GameObject body = Instantiate(bodyPrefab);
        bodyParts.Add(body);
        bodyCreationTimes.Add(body, Time.time);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if trigger detection is allowed
        if (!canDetectTrigger)
        {
            return; // Skip trigger detection if not allowed
        }

        // Check if the collided GameObject has a "food" tag
        if (other.gameObject.CompareTag("food"))
        {
            Debug.Log("Collision with food, executing GrowSnake method.");
            GrowSnake();
            Destroy(other.gameObject);
            gm.score++;
            gm.UpdateScore(gm.score);

            // You can add additional logic for handling food collisions if needed
        }
        else if (other.gameObject.CompareTag("snake"))
        {
            int snakeIndex = bodyParts.IndexOf(other.gameObject);

            if (snakeIndex == 0 || snakeIndex == 1)
            {
                Debug.Log("Collision with first or second snake part, skipping code execution.");
                return;
            }

            // Check the time since instantiation
            float elapsedTime;
            if (bodyCreationTimes.TryGetValue(other.gameObject, out elapsedTime) && (Time.time - elapsedTime) < 1f)
            {
                Debug.Log("Collision with a recently instantiated snake part, skipping code execution.");
                return;
            }

            // Continue with the rest of the code if it's not the last snake part
            gm.alive = false;
            Debug.Log("Snake trigger! Game over.");
        }
    }
    
}