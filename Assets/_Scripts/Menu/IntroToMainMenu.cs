using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroToMainMenu : MonoBehaviour
{
    void Start()
    {
        Cursor.visible = false;
        SceneManager.LoadScene("MainMenu");
    }
}
