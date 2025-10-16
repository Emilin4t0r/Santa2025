using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InsertCoin : MonoBehaviour
{
    public static InsertCoin Instance;

    public bool skipIntro;

    public GameObject insertCoinText;
    public MusicController mc;
    public AudioSource music;
    public Image movieImg;
    public GameObject canvas;
    public GameObject menuMusicController;

    public float flashInterval = 0.5f;
    float nextTimeToFlash;
    bool fading;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (skipIntro)
        {
            music.Stop();
            movieImg.enabled = false;
            insertCoinText.SetActive(false);
            fading = true;
            canvas.SetActive(false);
            menuMusicController.SetActive(true);
        }
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            StartCoroutine(FadeToMainMenu());
        }

        if (fading)
            return;
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
        fading = true;
        float fadeTime = 2;
        mc.FadeMusicOut(music, fadeTime);
        movieImg.DOColor(new Color(0, 0, 0, 0), fadeTime).SetEase(Ease.Linear);
        insertCoinText.SetActive(false);
        yield return new WaitForSeconds(fadeTime + 0.05f);
        canvas.SetActive(false);
        menuMusicController.SetActive(true);
    }
}
