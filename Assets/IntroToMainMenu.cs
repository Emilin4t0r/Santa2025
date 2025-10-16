using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroToMainMenu : MonoBehaviour
{
    void Start()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
