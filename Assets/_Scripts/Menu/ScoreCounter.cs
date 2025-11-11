using UnityEngine;

public class ScoreCounter : MonoBehaviour
{
    static float score;

    public static float GetScore()
    {
        return score;
    }
    public static void SetScore(float _score)
    {
        score = _score;
    }
    public static void ResetScore()
    {
        score = 0;
    }
}
