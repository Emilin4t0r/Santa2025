using DG.Tweening;
using System.Collections;
using UnityEngine;

public class MinsRemainingMap : MonoBehaviour
{
    public static MinsRemainingMap instance;

    public Transform[] numbers;

    private void Awake()
    {
        instance = this;
    }

    public void ShowOneMin()
    {
        DisplayNumber(numbers[0]);
    }
    public void ShowTwoMins()
    {
        DisplayNumber(numbers[1]);
    }

    void DisplayNumber(Transform num)
    {
        StartCoroutine(NumberEffects(num));
    }

    IEnumerator NumberEffects(Transform num)
    {
        num.gameObject.SetActive(true);
        num.localScale = Vector3.zero;
        num.DOScale(1, 1.5f).SetEase(Ease.OutExpo);
        yield return new WaitForSeconds(3);
        num.gameObject.SetActive(false);
    }
}
