using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InsertCoin : MonoBehaviour
{

    public GameObject insertCoinText;
    public MusicController mc;
    public AudioSource music;
    public Image fadeToBlackImg;

    public float flashInterval = 0.5f;
    float nextTimeToFlash;

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            StartCoroutine(FadeToMainMenu());
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

    IEnumerator FadeToMainMenu()
    {
        float fadeTime = 2;
        mc.FadeMusicOut(music, fadeTime);
        fadeToBlackImg.DOColor(new Color(0, 0, 0, 1), fadeTime);
        yield return new WaitForSeconds(fadeTime + 0.05f);
        SceneManager.LoadScene("MainMenu");
    }
}
