using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public Text scoreText;
    public int incrementAmount = 5; // Amount to increment the score
    public int decrementAmount = 5; // Amount to decrement the score

    private int score = 0;

    public int GetScore()
    {
        return score;
    }

    private void Start()
    {
        UpdateScoreText();
    }

    public void AddScore()
    {
        score += incrementAmount;
        UpdateScoreText();
    }

    public void SubtractScore()
    {
        score -= decrementAmount;
        UpdateScoreText();
    }

    public void ResetScore()
    {
        score = 0;
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        scoreText.text = score.ToString();
    }
}
