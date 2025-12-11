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
    public RawImage movieImg;
    public GameObject canvas;
    public GameObject menuMusicController;

    public float flashInterval = 0.5f;
    float nextTimeToFlash;
    bool faded;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (skipIntro || IntroSkipper.skipIntro)
        {
            music.Stop();
            movieImg.enabled = false;
            insertCoinText.SetActive(false);
            faded = true;
            canvas.SetActive(false);
            menuMusicController.SetActive(true);
        }
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.KeypadEnter) || Input.GetKeyUp(KeyCode.Return))
        {
            StartCoroutine(FadeToMainMenu());
        }

        if (faded)
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
        float fadeTime = 3;        

        mc.FadeMusicOut(music, fadeTime);
        movieImg.DOColor(new Color(0, 0, 0, 0), fadeTime).SetEase(Ease.Linear);
        insertCoinText.SetActive(false);
        menuMusicController.SetActive(true);        

        yield return new WaitForSeconds(fadeTime + 0.05f);
        faded = true;
        SoundSpawner.SpawnSound(transform.position, menuMusicController.transform, SoundLibrary.GetClip("coin_insert"), 0, 0);
        canvas.SetActive(false);
    }
}
