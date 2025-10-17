using System.Collections;
using UnityEngine;

public class MusicController : MonoBehaviour
{

    public AudioSource musicEntrance, musicLoop;
    public bool mainMenu;
    public MainMenuController mmc;

    void Start()
    {
        if (InsertCoin.Instance.skipIntro)
        {
            InstantStart();
            return;
        }
        if (mainMenu)
            SetAudioSourcePlayTimes();
    }

    public void InstantStart()
    {
        musicEntrance.Stop();
        musicLoop.Play();
        StartCoroutine(ShowTitleAfterTime(0, true));
    }

    void SetAudioSourcePlayTimes()
    {
        double dspStart = AudioSettings.dspTime + 0.1;
        musicEntrance.PlayScheduled(dspStart);

        // Calculate exact length in seconds from samples/frequency (more exact than clip.length)
        double entranceLength = (double)musicEntrance.clip.samples / musicEntrance.clip.frequency;

        double loopStart = dspStart + entranceLength;
        if (loopStart < AudioSettings.dspTime)
            loopStart = AudioSettings.dspTime + 0.05; // fallback safety

        musicLoop.PlayScheduled(loopStart);
        StartCoroutine(ShowTitleAfterTime(loopStart, false));
    }
    IEnumerator ShowTitleAfterTime(double time, bool skipAnimations)
    {
        while (AudioSettings.dspTime < time)
        {
            yield return null;
        }
        mmc.ShowTitle(skipAnimations);
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

    public void FadeToLowPass(AudioLowPassFilter filter)
    {
        StartCoroutine(FadeLowPassCoroutine(filter, 5000, 1.5f));
    }    
    public void FadeFromLowPass(AudioLowPassFilter filter)
    {
        StartCoroutine(FadeLowPassCoroutine(filter, 22000, 1.5f));
    }
    IEnumerator FadeLowPassCoroutine(AudioLowPassFilter filter, float targetCutoff, float fadeOutTime)
    {
        float startCutoff = filter.cutoffFrequency;
        float time = 0f;

        while (time < fadeOutTime)
        {
            time += Time.deltaTime;
            filter.cutoffFrequency = Mathf.Lerp(startCutoff, targetCutoff, time / fadeOutTime);
            yield return null;
        }

        filter.cutoffFrequency = targetCutoff;
    }
}
