using System.Collections;
using System.Collections.Generic;
using TMPro;
using TMPro.Examples;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject player;
    public bool alive = true;
    public bool play = false;
    public int score;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private Button playButton;
    [SerializeField] private Canvas mainMenu;
    [SerializeField] private Canvas gameMenu;
    [SerializeField] private Canvas gameOverMenu;
    [SerializeField] private TextMeshProUGUI scoreLost;
    [SerializeField] private GameObject stickControllerPrefab;

    private StickController stickControllerInstance;
    // PlayerPrefs key for storing and retrieving the high score
    private string highScoreKey = "HighScore";

    // Start is called before the first frame update
    void Start()
    {
        score = 0;
        play = false;
        // Load the high score from PlayerPrefs
        int savedHighScore = PlayerPrefs.GetInt(highScoreKey, 0);
        highScoreText.SetText("High Score: " + savedHighScore);
        UpdateScore(0);
        stickControllerInstance = Instantiate(stickControllerPrefab).GetComponent<StickController>();

    }

    // Update is called once per frame
    void Update()
    {
        // Check if the current score is greater than the saved high score
        if (score > PlayerPrefs.GetInt(highScoreKey, 0))
        {
            // Update the high score and save it to PlayerPrefs
            PlayerPrefs.SetInt(highScoreKey, score);
            PlayerPrefs.Save();

            // Update the high score text on the UI
            highScoreText.SetText("High Score: " + score);
        }
        if (alive== false)
        {
            play = false;
            gameMenu.gameObject.SetActive(false);
            gameOverMenu.gameObject.SetActive(true);
            scoreLost.SetText("Your Socre: " + score);
            stickControllerPrefab.gameObject.SetActive(false);    
        }
    }

    public void UpdateScore(int newScore)
    {
        // Update the score and update the score text on the UI
        score = newScore;
        scoreText.SetText("Score: " + score);
    }
    public void OnClick()
    {
        play = true;
        mainMenu.gameObject.SetActive(false);
        gameMenu.gameObject.SetActive(true);
        stickControllerPrefab.gameObject.SetActive(true);
    }
    public void ReloadScene()
    {
        // Get the currently active scene
        Scene currentScene = SceneManager.GetActiveScene();

        // Reload the current scene
        SceneManager.LoadScene(currentScene.name);
    }

}