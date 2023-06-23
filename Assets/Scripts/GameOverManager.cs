using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverManager : MonoBehaviour
{
    public Text scoreText;
    public Image star1;
    public Image star2;
    public Image star3;
    public Sprite activeStarSprite;
    public Sprite inactiveStarSprite;
    public int minimumScoreForTwoStars = 100;
    public int minimumScoreForThreeStars = 200;
    public Button nextLevelButton;

    private int currentLevelStarsEarned = 0; // Stars earned in the current gameplay session
    private int totalStarsEarned = 0; // Total stars earned across levels
    private int totalStarsEarned2 = 0; // Total stars earned across levels
    private int currentLevelIndex;

    private int lastLevelIndex;

    private void Start()
    {
        // Hide the game-over panel initially
        gameObject.SetActive(false);

        // Retrieve the total stars earned from PlayerPrefs
        totalStarsEarned = PlayerPrefs.GetInt("TotalStarsEarned", 0);

        // Get the current level index
        currentLevelIndex = SceneManager.GetActiveScene().buildIndex;

        // Get the last level index
        lastLevelIndex = SceneManager.sceneCountInBuildSettings - 1;
    }

    public void ShowGameOverPanel(int score)
    {
        // Set the score text in the panel
        scoreText.text = "Score: " + score.ToString();

        // Show the game-over panel
        gameObject.SetActive(true);

        // Pause the game
        Time.timeScale = 0f;

        // Update star sprites based on the score
        UpdateStarSprites(score);

        // Update the stars earned in the current level
        string levelStarsKey = "Level" + currentLevelIndex + "StarsEarned";
        int previousLevelStarsEarned = PlayerPrefs.GetInt(levelStarsKey, 0);

        // Update the level stars earned only if it is greater than the previous value
        if (currentLevelStarsEarned > previousLevelStarsEarned)
        {
            PlayerPrefs.SetInt(levelStarsKey, currentLevelStarsEarned);
        }

        // Update the total stars earned across levels
        totalStarsEarned += currentLevelStarsEarned;
        PlayerPrefs.SetInt("TotalStarsEarned", totalStarsEarned);

        for (int levelIndex = 1; levelIndex <= 3; levelIndex++)
        {
            string levelStarsKey2 = "Level" + levelIndex + "StarsEarned";
            totalStarsEarned2 += PlayerPrefs.GetInt(levelStarsKey2, 0);
        }

        // Disable the next level button if the current level is the last level
        if (currentLevelIndex == lastLevelIndex)
        {
            nextLevelButton.interactable = false;
        } else {
            // Check if the next level is unlocked
            bool isNextLevelUnlocked = IsLevelUnlocked(currentLevelIndex + 1);
            nextLevelButton.interactable = isNextLevelUnlocked;
        }

    }

    private void UpdateStarSprites(int score)
    {
        // Check the score to determine the number of stars
        if (score >= minimumScoreForThreeStars)
        {
            star1.sprite = activeStarSprite;
            star2.sprite = activeStarSprite;
            star3.sprite = activeStarSprite;
            currentLevelStarsEarned = 3;
        }
        else if (score >= minimumScoreForTwoStars)
        {
            star1.sprite = activeStarSprite;
            star2.sprite = activeStarSprite;
            star3.sprite = inactiveStarSprite;
            currentLevelStarsEarned = 2;
        }
        else
        {
            star1.sprite = activeStarSprite;
            star2.sprite = inactiveStarSprite;
            star3.sprite = inactiveStarSprite;
            currentLevelStarsEarned = 1;
        }
    }

    public void RestartLevel()
    {
        // Resume the game
        Time.timeScale = 1f;

        // Reload the current scene
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    public void GoToHomeScene()
    {
        // Resume the game
        Time.timeScale = 1f;

        // Load the home scene or any desired scene
        SceneManager.LoadScene("Menu");
    }

    public void GoToNextLevel()
    {
        if (nextLevelButton.interactable)
        {
            // Resume the game
            Time.timeScale = 1f;

            // Load the next level scene
            SceneManager.LoadScene(currentLevelIndex + 1);
        }
    }

    private bool IsLevelUnlocked(int levelIndex)
    {
        // Check if the level index is within the build index range
        int requiredStars = LevelManager.Instance.starsRequiredForUnlock[levelIndex - 1];
        return totalStarsEarned2 >= requiredStars;
    }
}
