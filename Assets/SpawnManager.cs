using System.Collections;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject foodPrefab;
    public Transform spawnArea;  // Reference to the transform where you want to spawn the food
    public float spawnInterval = 3f;  // Time interval between spawns
    private bool isFoodSpawning = false;  // Flag to check if food is currently being spawned
    [SerializeField] public GameManager gm;
    void Start()
    {
        // Start spawning coroutine
       
        
            StartCoroutine(SpawnFood());
        
    }

    IEnumerator SpawnFood()
    {
        while (true)  // Infinite loop for continuous spawning
        {
            
                // Check if food is currently being spawned
                if (!isFoodSpawning&& gm.play)
                {
                    isFoodSpawning = true;

                    // Spawn food within the local scale of the spawnArea
                    Vector3 spawnPosition = new Vector3(
                        Random.Range(-spawnArea.localScale.x / 42, spawnArea.localScale.x / 42),
                        0.5f,  // You can adjust the Y position as needed
                        Random.Range(-spawnArea.localScale.z / 42, spawnArea.localScale.z / 42)
                    );

                    Instantiate(foodPrefab, spawnArea.TransformPoint(spawnPosition), Quaternion.identity);

                    // Wait for the next spawnInterval before spawning the next food
                    yield return new WaitForSeconds(spawnInterval);

                    isFoodSpawning = false;
                }
                else
                {
                    // Wait for a short duration before checking again if food can be spawned
                    yield return new WaitForSeconds(0.1f);
                }
            
        }
    }
}