using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class FadeBlack : MonoBehaviour
{
    public float endValue, duration;
    public bool destroy;

    private void Start()
    {
        if (endValue == 0)
        {
            DoFade();
        }
    }

    public void DoFade()
    {
        var img = GetComponent<Image>();
        img.DOFade(endValue, duration);
        if (destroy)
            Destroy(gameObject, 10);
    }
}
