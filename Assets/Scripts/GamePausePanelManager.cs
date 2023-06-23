using UnityEngine;
using UnityEngine.SceneManagement;

public class GamePausePanelManager : MonoBehaviour
{
    public void OnMenuButtonClick()
    {
        Time.timeScale = 1f; // Resume the game
        SceneManager.LoadScene("Menu"); // Load the MenuScene or any desired scene
    }

    public void OnRestartButtonClick()
    {
        Time.timeScale = 1f; // Resume the game

        // Reload the current scene
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    public void OnResumeButtonClick()
    {
        FruitManager fruitManager = FindObjectOfType<FruitManager>();
        if (fruitManager != null)
        {
            fruitManager.ResumeGame(); // Call the ResumeGame() method in the FruitManager script
        }
        else
        {
            Debug.LogWarning("FruitManager script not found.");
        }
    }
}
