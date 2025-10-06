using UnityEngine;

public class MusicController : MonoBehaviour
{

    public AudioSource musicEntrance, musicLoop;

    void Start()
    {
        SetAudioSourcePlayTimes();
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
    }

    void FadeMusicOut(AudioSource source)
    {

    }
    void FadeToLowPass(AudioSource source)
    {

    }
    void FadeFromLowPass(AudioSource source)
    {

    }
}
