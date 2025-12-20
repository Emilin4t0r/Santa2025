using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    public float gameTime = 180f;
    public static float gameTimer;
    public TextMeshProUGUI text;

    AircraftUtils au;
    bool fadeStarted;

    bool oneMinuteNotified, twoMinutesNotified;

    void Start()
    {
        gameTimer = gameTime;
        au = AircraftUtils.instance;
        CountTimer(); // Get initial timer reading
    }
    private void FixedUpdate()
    {
        if (!au.turnedOn)
            return;

        CountTimer();        
    }

    void StartEndFade()
    {
        GameObject.Find("ToBlack").GetComponent<FadeBlack>().DoFade();
    }

    void CountTimer()
    {
        gameTimer -= Time.fixedDeltaTime;
        if (gameTimer < 3 && !fadeStarted)
        {
            StartEndFade();
            fadeStarted = true;
        }
        if (gameTimer < 0)
        {
            Cursor.lockState = CursorLockMode.None;
            UnityEngine.SceneManagement.SceneManager.LoadScene("Scoreboard");
        }
        int minutes = (int)gameTimer / 60;
        int seconds = (int)(gameTimer - (minutes * 60));
        string s_sec = "";
        string s_min = "";
        if (seconds < 10)
        {
            s_sec = "0" + seconds.ToString();
        }
        else
        {
            s_sec = seconds.ToString();
        }
        s_min = "0" + minutes.ToString();

        text.text = s_min + ":" + s_sec;

        if (!twoMinutesNotified && gameTimer < 120)
        {
            Notifications.instance.ShowNotification("- 2 minutes remaining -");
            SoundSpawner.SpawnSound(transform.position, null, SoundLibrary.GetClip("2min_remaining"), 0, 0);
            MinsRemainingMap.instance.ShowTwoMins();
            twoMinutesNotified = true;
        }
        if (!oneMinuteNotified && gameTimer < 60)
        {
            Notifications.instance.ShowNotification("- 1 minute remaining -");
            SoundSpawner.SpawnSound(transform.position, null, SoundLibrary.GetClip("1min_remaining"), 0, 0);
            MinsRemainingMap.instance.ShowOneMin();
            oneMinuteNotified = true;
        }
    }
}
