using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Targeter : MonoBehaviour
{
    public static Targeter instance;
    Image img;
    float flashSpeed = 0.25f;
    bool flashing;
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        img = GetComponent<Image>();
        img.enabled = false;
    }

    public void StartFlash(float duration, float frequency)
    {
        flashSpeed = frequency;
        if (flashing)
            return;
        StartCoroutine(Flasher(duration));        
    }
    IEnumerator Flasher(float duration)
    {
        flashing = true;
        float timeToStopFlashing = Time.time + duration;
        float flashTimer = 0;
        while (Time.time < timeToStopFlashing)
        {
            flashTimer += Time.deltaTime;
            if (flashTimer > flashSpeed)
            {
                img.enabled = !img.enabled;
                flashTimer = 0;
            }
            yield return null;
        }
        if (Time.time > timeToStopFlashing)
            flashing = false;
        img.enabled = false;
    }

    public bool ImgEnabled()
    {
        return img.enabled;
    }

    public void EnableImg(bool state)
    {
        img.enabled = state;
    }
}
