using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public Text totalStarText;
    public GameObject[] levelStarContainers;
    public GameObject[] levelLockOverlays;
    public Sprite activeStarSprite;
    public Sprite inactiveStarSprite;
    public Sprite lockSprite; // Add a sprite for the lock overlay
    public int[] starsRequiredForUnlock = { 0, 2, 5};

    public Text[] levelStarRequirementTexts;

    private int totalStarsEarned = 0;

    public static LevelManager Instance { get; private set; }

    private void Awake()
    {
        // Assign the instance to the static variable when the script is awakened
        Instance = this;
    }

    private void Start()
    {
        for (int levelIndex = 1; levelIndex <= 3; levelIndex++)
        {
            string levelStarsKey = "Level" + levelIndex + "StarsEarned";
            totalStarsEarned += PlayerPrefs.GetInt(levelStarsKey, 0);
        }

        totalStarText.text = "Total Stars: " + totalStarsEarned.ToString();

        UpdateMenuLevelStarSprites();
        UpdateMenuLevelLockState();
    }

    public void LoadLevel(string levelName)
    {
        int levelIndex = int.Parse(levelName.Substring(levelName.Length - 1));
        if (IsLevelUnlocked(levelIndex))
        {
            SceneManager.LoadScene(levelName);
        }
        else
        {
            Debug.Log("Level " + levelIndex + " is locked!");
        }
    }

    public bool IsLevelUnlocked(int levelIndex)
    {
        if (levelIndex == 1)
        {
            return true;
        }

        int requiredStars = starsRequiredForUnlock[levelIndex - 1];
        return totalStarsEarned >= requiredStars;
    }

    private void UpdateMenuLevelStarSprites()
    {
        for (int levelIndex = 0; levelIndex < levelStarContainers.Length; levelIndex++)
        {
            int starsEarned = PlayerPrefs.GetInt("Level" + (levelIndex + 1) + "StarsEarned", 0);
            Transform starContainer = levelStarContainers[levelIndex].transform;

            for (int starIndex = 0; starIndex < starContainer.childCount; starIndex++)
            {
                Image starImage = starContainer.GetChild(starIndex).GetComponent<Image>();
                starImage.sprite = starIndex < starsEarned ? activeStarSprite : inactiveStarSprite;
            }
        }
    }

    private void UpdateMenuLevelLockState()
    {
        for (int levelIndex = 0; levelIndex < levelLockOverlays.Length; levelIndex++)
        {
            bool isUnlocked = IsLevelUnlocked(levelIndex + 1);
            Image lockImage = levelLockOverlays[levelIndex].GetComponent<Image>();
            Text starRequirementText = levelStarRequirementTexts[levelIndex]; // Get the corresponding star requirement text

            if (isUnlocked)
            {
                // Level is unlocked, disable the lock overlay and star requirement text
                levelLockOverlays[levelIndex].SetActive(false);
                starRequirementText.gameObject.SetActive(false);
            }
            else
            {
                // Level is locked, enable the lock overlay and set its sprite, enable star requirement text
                levelLockOverlays[levelIndex].SetActive(true);
                lockImage.sprite = lockSprite;
                starRequirementText.gameObject.SetActive(true);

                // Set the star requirement text based on the stars required for unlock
                starRequirementText.text = "Requires " + starsRequiredForUnlock[levelIndex] + " Star";
            }
        }
    }
}
