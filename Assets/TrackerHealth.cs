using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

public class TrackerHealth : MonoBehaviour
{
    Slider healthSlider;
    RectTransform sliderRect;
    Image sliderImg;

    float health;
    public Color barColor, damageColor;

    private void Awake()
    {
        healthSlider = GetComponentInChildren<Slider>();
        sliderRect = healthSlider.GetComponent<RectTransform>();
        sliderImg = healthSlider.fillRect.GetComponent<Image>();
        sliderImg.color = barColor;
    }

    public void SetHealth(float amount)
    {
        if (amount <= 0)
            return;
        healthSlider.value = amount;
        health = amount;
    }

    public void ChangeHealth(float newHealth, float transitionDuration)
    {
        if (newHealth <= 0)
            return;
        StartCoroutine(ChangeHealthCor(newHealth, transitionDuration));
    }

    IEnumerator ChangeHealthCor(float newHealth, float transitionDuration)
    {        
        healthSlider.DOValue(newHealth, transitionDuration).SetEase(Ease.Linear);
        sliderImg.DOColor(damageColor, transitionDuration / 4).SetEase(Ease.OutExpo);
        //transform.DOScale(2, transitionDuration / 2).SetEase(Ease.Linear);
        sliderRect.DOSizeDelta(new Vector2(sliderRect.sizeDelta.x, 100), transitionDuration / 4).SetEase(Ease.Linear);
        yield return new WaitForSeconds(transitionDuration / 2);
        sliderImg.DOColor(barColor, transitionDuration / 4).SetEase(Ease.InExpo);
        //transform.DOScale(1, transitionDuration / 2).SetEase(Ease.Linear);
        sliderRect.DOSizeDelta(new Vector2(sliderRect.sizeDelta.x, 1), transitionDuration / 4).SetEase(Ease.Linear);
        health = newHealth;
    }
}
