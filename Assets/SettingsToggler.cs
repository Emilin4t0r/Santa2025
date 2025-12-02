using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingsToggler : MonoBehaviour
{
    public GameObject settingsMenu;
    public static bool gamePaused;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleSettingsMenu();
        }
    }

    public void ToggleSettingsMenu()
    {
        settingsMenu.SetActive(!settingsMenu.activeSelf);
        if (settingsMenu.activeSelf)
        {
            Time.timeScale = 0;
            AudioListener.pause = true;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            gamePaused = true;
        } else
        {
            Time.timeScale = 1;
            AudioListener.pause = false;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            gamePaused = false;
        }
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1;
        AudioListener.pause = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        gamePaused = false;

        SceneManager.LoadScene("MainMenu");
    }
}
