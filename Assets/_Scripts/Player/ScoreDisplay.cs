using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScoreDisplay : MonoBehaviour
{
    
    static TextMeshProUGUI scoreDisplay;

    private void OnEnable()
    {
        SceneManager.activeSceneChanged += OnSceneChanged;
    }
    private void OnDisable()
    {
        SceneManager.activeSceneChanged -= OnSceneChanged;
    }

    void OnSceneChanged(Scene old, Scene now)
    {
        if (now.name == "Gameplay")
        {
            ScoreCounter.ResetScore(); // Reset score on new run
            scoreDisplay = GetComponent<TextMeshProUGUI>();
            UpdateScoreText();
        }
    }

    public static void AddScore(float _score)
    {
        ScoreCounter.SetScore(ScoreCounter.GetScore() + _score);
        UpdateScoreText();
    }

    static void UpdateScoreText()
    {
        if (scoreDisplay)
            scoreDisplay.text = ScoreCounter.GetScore().ToString("0");
    }
}
