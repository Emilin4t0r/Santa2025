using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadMainMenuFromReplay : MonoBehaviour
{
    public static LoadMainMenuFromReplay instance;

    public AudioSource music;
    public Image black;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        // Unlock cursor
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void FadeMusicOut(AudioSource source, float fadeOutTime)
    {
        StartCoroutine(FadeOutCoroutine(source, fadeOutTime));
    }
    IEnumerator FadeOutCoroutine(AudioSource source, float fadeOutTime)
    {
        float startVolume = source.volume;
        float time = 0f;

        while (time < fadeOutTime)
        {
            time += Time.deltaTime;
            source.volume = Mathf.Lerp(startVolume, 0f, time / fadeOutTime);
            yield return null;
        }

        source.volume = 0f;
        source.Stop();
        source.volume = startVolume; // reset for next playback if needed
    }

    public void LoadMainMenu()
    {        
        StartCoroutine(MenuLoader());
    }

    IEnumerator MenuLoader()
    {
        FadeMusicOut(music, 5);
        IntroSkipper.skipIntro = true;
        black.DOFade(1, 5).SetEase(Ease.InExpo);
        yield return new WaitForSeconds(5);
        SceneManager.LoadScene("MainMenu");
    }
}
