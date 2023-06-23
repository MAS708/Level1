using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TimerManager : MonoBehaviour
{
    public float gameDuration = 120f; // Duration of the game in seconds
    public Text timerText; // Reference to the UI text component displaying the timer

    private float remainingTime; // Remaining time in seconds
    private ScoreManager scoreManager;
    private FruitManager fruitManager;
    private GameOverManager gameOverManager;

    private void Start()
    {
        remainingTime = gameDuration;
        UpdateTimerUI();

        scoreManager = FindObjectOfType<ScoreManager>();
        fruitManager = FindObjectOfType<FruitManager>();
        gameOverManager = FindObjectOfType<GameOverManager>();

        InvokeRepeating("UpdateTimer", 1f, 1f); // Call UpdateTimer() method every second
    }

    private void UpdateTimer()
    {
        remainingTime -= 1f;
        UpdateTimerUI();

        if (remainingTime <= 0f)
        {
            // Game over, reset the game
            ResetGame();
        }
    }

    private void UpdateTimerUI()
    {
        // Format remaining time into minutes and seconds
        int minutes = Mathf.FloorToInt(remainingTime / 60f);
        int seconds = Mathf.FloorToInt(remainingTime % 60f);

        // Update the UI text component with the formatted time
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private void ResetGame()
    {
        if (scoreManager != null && fruitManager != null && gameOverManager != null)
        {
            // Store the score before resetting
            int lastScore = scoreManager.GetScore();

            // Reset remaining time
            remainingTime = gameDuration;
            UpdateTimerUI();

            // Reset score
            scoreManager.ResetScore();

            // Disable fruit click during GameOverPanel display
            fruitManager.DisableFruitClick();

            // Show the game-over panel with the score
            gameOverManager.ShowGameOverPanel(lastScore);
        }
        else
        {
            Debug.LogWarning("One or more references are not assigned in the TimerManager script.");
        }
    }

}
