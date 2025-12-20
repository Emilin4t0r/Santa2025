using System.Collections;
using TMPro;
using UnityEngine;

public class Notifications : MonoBehaviour
{
    public static Notifications instance;

    TextMeshProUGUI text;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        text = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
    }

    public void ShowNotification(string txt)
    {
        text.text = txt;
        StartCoroutine(ShowNotificationForSeconds(3));
    }

    IEnumerator ShowNotificationForSeconds(float time)
    {
        text.enabled = true;
        yield return new WaitForSeconds(time);
        text.enabled = false;
    }
}
