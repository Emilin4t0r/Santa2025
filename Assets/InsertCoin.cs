using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InsertCoin : MonoBehaviour
{

    public GameObject insertCoinText;

    public float flashInterval = 0.5f;
    float nextTimeToFlash;

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            LoadMainMenu();
        }

        if (Time.time > nextTimeToFlash)
        {
            nextTimeToFlash = Time.time + flashInterval;
            ToggleInsertCoinText();
        }
    }

    void ToggleInsertCoinText()
    {
        if (insertCoinText.activeSelf)
            insertCoinText.SetActive(false);
        else 
            insertCoinText.SetActive(true);
    }

    void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
