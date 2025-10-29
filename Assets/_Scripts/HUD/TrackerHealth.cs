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
    public TextMeshProUGUI damageText;

    private Coroutine changeHealthCoroutine;

    private void Awake()
    {
        healthSlider = GetComponentInChildren<Slider>();
        sliderRect = healthSlider.GetComponent<RectTransform>();
        sliderImg = healthSlider.fillRect.GetComponent<Image>();
        sliderImg.color = barColor;
        ResetDamageTextAlpha();
        damageText.enabled = true;
    }

    public void SetHealth(float amount)
    {
        if (amount <= 0)
            return;
        healthSlider.value = amount;
        health = amount;
    }

    void ResetDamageTextAlpha()
    {
        var col = damageText.color;
        col.a = 0f;
        damageText.color = col;        
    }

    public void ChangeHealth(float newHealth, float transitionDuration)
    {
        if (newHealth <= 0)
            return;

        // If a coroutine is already running, stop it and kill related tweens
        if (changeHealthCoroutine != null)
        {
            StopCoroutine(changeHealthCoroutine);
            changeHealthCoroutine = null;

            // Make sure we don't leave any tweens running on these targets
            DOTween.Kill(healthSlider);
            DOTween.Kill(sliderImg);
            DOTween.Kill(sliderRect);

            // Reset UI to a sane default so new animation starts from predictable state
            ResetDamageTextAlpha();
            sliderImg.color = barColor;
            // reset slider height
            sliderRect.sizeDelta = new Vector2(sliderRect.sizeDelta.x, 1);
        }

        changeHealthCoroutine = StartCoroutine(ChangeHealthCor(newHealth, transitionDuration));
    }

    IEnumerator ChangeHealthCor(float newHealth, float transitionDuration)
    {        
        damageText.DOFade(1, transitionDuration / 4);
        damageText.text = "-" + (health - newHealth).ToString("0");
        healthSlider.DOValue(newHealth, transitionDuration).SetEase(Ease.Linear);
        sliderImg.DOColor(damageColor, transitionDuration / 4).SetEase(Ease.OutExpo);
        //transform.DOScale(2, transitionDuration / 2).SetEase(Ease.Linear);
        sliderRect.DOSizeDelta(new Vector2(sliderRect.sizeDelta.x, 100), transitionDuration / 4).SetEase(Ease.Linear);
        yield return new WaitForSeconds(transitionDuration / 2);
        yield return new WaitForSeconds(transitionDuration / 4);
        sliderImg.DOColor(barColor, transitionDuration / 4).SetEase(Ease.InExpo);
        //transform.DOScale(1, transitionDuration / 2).SetEase(Ease.Linear);
        sliderRect.DOSizeDelta(new Vector2(sliderRect.sizeDelta.x, 1), transitionDuration / 4).SetEase(Ease.Linear);
        damageText.DOFade(0, transitionDuration / 4);        
        health = newHealth;

        changeHealthCoroutine = null;
    }
}
