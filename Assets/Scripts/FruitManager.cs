using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FruitManager : MonoBehaviour
{
    public Sprite[] fruitSprites; // Array to hold the fruit sprites
    public AudioClip[] fruitAudioClips; // Array to hold the fruit audio clips
    public AudioClip rightAudioClip; // Audio clip for the "Right" sound
    public AudioClip wrongAudioClip; // Audio clip for the "Wrong" sound
    public AudioClip pleaseChooseAudioClip; // Audio clip for the "Wrong" sound

    public float soundVolume = 1f; // Adjust this value to control the volume of the fruit sounds

    private AudioSource audioSource;
    private string playedFruitName; // The name of the fruit audio clip that was played

    private ScoreManager scoreManager;

    private List<GameObject> fruitObjects = new List<GameObject>(); // List to hold references to the fruit game objects

    public float fadeInDuration = 0.5f;
    public float fadeOutDuration = 0.5f;

    private GameObject gamePausePanel;

    private void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();

        // Get a reference to the ScoreManager component
        scoreManager = FindObjectOfType<ScoreManager>();

        gamePausePanel = GameObject.Find("Canvas/GamePausePanel");
        if (gamePausePanel != null)
        {
            gamePausePanel.SetActive(false); // Hide the game pause panel initially
        }
        else
        {
            Debug.LogWarning("GamePausePanel object not found in the Canvas.");
        }

        InitializeFruitObjects();
        SpawnRandomFruits();
    }

    public void PauseGame()
    {
        Time.timeScale = 0f; // Pause the game
        gamePausePanel.SetActive(true); // Show the game pause panel
        DisableFruitClick();
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f; // Resume the game
        gamePausePanel.SetActive(false); // Hide the game pause panel
        EnableFruitClick();
    }

    public void OnPauseButtonClick()
    {
        PauseGame(); // Call PauseGame() when the pause button is clicked
    }

    private void InitializeFruitObjects()
    {
        // Clear the fruitObjects list
        fruitObjects.Clear();

        // Get references to all the fruit game objects in the hierarchy
        foreach (Transform child in transform)
        {
            GameObject fruitObject = child.gameObject;
            fruitObjects.Add(fruitObject);
        }
    }

    private void SpawnRandomFruits()
    {
        // Generate a list of available fruit sprites
        var availableSprites = new List<Sprite>(fruitSprites);

        // Randomly select a sprite for each fruit object
        foreach (GameObject fruitObject in fruitObjects)
        {
            // Get a random fruit sprite
            Sprite randomFruit = GetRandomFruitSprite(availableSprites);

            // Set the sprite of the fruit object
            SpriteRenderer spriteRenderer = fruitObject.GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = randomFruit;

            // Set the fruit object to be invisible initially
            spriteRenderer.color = new Color(1f, 1f, 1f, 0f);

            // Fade in the fruit object
            StartCoroutine(FadeInFruit(fruitObject, fadeInDuration));
        }

        // Calculate the delay based on the length of the "Right" or "Wrong" sound
        float soundDelay = Mathf.Max(rightAudioClip.length, wrongAudioClip.length);

        audioSource.PlayOneShot(pleaseChooseAudioClip, soundVolume);

        // Play the audio clip that matches one of the selected fruits after the delay
        StartCoroutine(PlayRandomFruitSoundAfterDelay(soundDelay));
    }

    private IEnumerator PlayRandomFruitSoundAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Get the names of the selected fruits
        string[] fruitNames = new string[fruitObjects.Count];
        for (int i = 0; i < fruitObjects.Count; i++)
        {
            fruitNames[i] = fruitObjects[i].GetComponent<SpriteRenderer>().sprite.name;
        }

        AudioClip randomFruitAudioClip = GetRandomFruitAudioClip(fruitNames);
        audioSource.PlayOneShot(randomFruitAudioClip, soundVolume);
        playedFruitName = randomFruitAudioClip.name;
    }

    private Sprite GetRandomFruitSprite(List<Sprite> availableSprites)
    {
        // Generate a random index to select a fruit sprite
        int randomIndex = Random.Range(0, availableSprites.Count);

        // Get the selected sprite and remove it from the available list
        Sprite selectedSprite = availableSprites[randomIndex];
        availableSprites.RemoveAt(randomIndex);

        return selectedSprite;
    }

    private AudioClip GetRandomFruitAudioClip(string[] fruitNames)
    {
        // Create a list to store available fruit audio clips
        var availableAudioClips = new List<AudioClip>();

        // Iterate through the fruit audio clips and add the ones that match the selected fruit names
        foreach (AudioClip audioClip in fruitAudioClips)
        {
            string audioClipName = audioClip.name;
            foreach (string fruitName in fruitNames)
            {
                if (audioClipName.Contains(fruitName))
                {
                    availableAudioClips.Add(audioClip);
                    break;
                }
            }
        }

        // Select a random audio clip from the available ones
        int randomIndex = Random.Range(0, availableAudioClips.Count);
        return availableAudioClips[randomIndex];
    }

    public void OnFruitClick(string clickedFruitName)
    {
        if (playedFruitName != null && clickedFruitName != null && playedFruitName.Contains(clickedFruitName))
        {
            // Clicked the right fruit
            audioSource.PlayOneShot(rightAudioClip, soundVolume);
            Debug.Log("Right!");
            scoreManager.AddScore();
        }
        else
        {
            // Clicked the wrong fruit
            audioSource.PlayOneShot(wrongAudioClip, soundVolume);
            Debug.Log("Wrong!");
            scoreManager.SubtractScore();
        }

        // Fade out the fruit objects
        foreach (GameObject fruitObject in fruitObjects)
        {
            StartCoroutine(FadeOutFruit(fruitObject, fadeOutDuration));
        }

        // Spawn new random fruits after the fade-out
        StartCoroutine(SpawnRandomFruitsAfterDelay(fadeOutDuration));
    }

    private IEnumerator FadeOutFruit(GameObject fruitObject, float duration)
    {
        SpriteRenderer spriteRenderer = fruitObject.GetComponent<SpriteRenderer>();

        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            // Calculate the current alpha value based on the elapsed time and duration
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / duration);

            // Set the alpha value of the sprite renderer color
            Color color = spriteRenderer.color;
            color.a = alpha;
            spriteRenderer.color = color;

            // Increment the elapsed time
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        // Ensure the final alpha value is exactly 0
        Color finalColor = spriteRenderer.color;
        finalColor.a = 0f;
        spriteRenderer.color = finalColor;
    }

    private IEnumerator SpawnRandomFruitsAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Rest of the code to spawn new random fruits
        SpawnRandomFruits();
    }

    public void ResetFruit()
    {
        // Clear the list of fruit objects
        fruitObjects.Clear();

        // Get references to all the fruit game objects in the hierarchy
        foreach (Transform child in transform)
        {
            GameObject fruitObject = child.gameObject;
            fruitObjects.Add(fruitObject);
        }

        // Reset the sprites of the fruit objects
        foreach (GameObject fruitObject in fruitObjects)
        {
            fruitObject.GetComponent<SpriteRenderer>().sprite = null;
        }

        // Spawn new random fruits
        SpawnRandomFruits();
    }

    public void DisableFruitClick()
    {
        foreach (GameObject fruitObject in fruitObjects)
        {
            FruitClickHandler clickHandler = fruitObject.GetComponent<FruitClickHandler>();
            clickHandler.DisableClick();
        }
    }

    public void EnableFruitClick()
    {
        foreach (GameObject fruitObject in fruitObjects)
        {
            FruitClickHandler clickHandler = fruitObject.GetComponent<FruitClickHandler>();
            clickHandler.EnableClick();
        }
    }

    private IEnumerator FadeInFruit(GameObject fruitObject, float duration)
    {
        SpriteRenderer spriteRenderer = fruitObject.GetComponent<SpriteRenderer>();

        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            // Calculate the current alpha value based on the elapsed time and duration
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / duration);

            // Set the alpha value of the sprite renderer color
            Color color = spriteRenderer.color;
            color.a = alpha;
            spriteRenderer.color = color;

            // Increment the elapsed time
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        // Ensure the final alpha value is exactly 1
        Color finalColor = spriteRenderer.color;
        finalColor.a = 1f;
        spriteRenderer.color = finalColor;
    }

}
