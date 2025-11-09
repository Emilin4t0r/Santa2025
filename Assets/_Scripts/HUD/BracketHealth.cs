using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

public class BracketHealth : MonoBehaviour
{
    TextMeshProUGUI healthText;
    Slider healthSlider;
    Image sliderImg;

    float health;
    bool changingHealth;

    public Color barColor, damageColor;
    public TextMeshProUGUI damageText;

    private Coroutine changeHealthCoroutine;

    private void Awake()
    {
        healthText = GetComponentInChildren<TextMeshProUGUI>();
        healthSlider = GetComponentInChildren<Slider>();
        sliderImg = healthSlider.fillRect.GetComponent<Image>();
        sliderImg.color = barColor;
        ResetDamageTextAlpha();
        damageText.enabled = true;
    }

    private void OnEnable()
    {
        sliderImg.color = barColor;
        ResetDamageTextAlpha();
    }

    private void Update()
    {
        if (changingHealth)
        {
            healthText.text = health.ToString("0") + "%";            
        }
    }

    void ResetDamageTextAlpha()
    {
        var col = damageText.color;
        col.a = 0f;
        damageText.color = col;
    }

    public void SetHealth(float amount)
    {
        if (amount <= 0)
            return;
        changingHealth = false;
        healthSlider.value = amount;
        healthText.text = amount.ToString("0") + "%";
        health = amount;
    }

    public void ChangeHealth(float newHealth)
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

            // Reset UI to a sane default so new animation starts from predictable state
            ResetDamageTextAlpha();
            sliderImg.color = barColor;
        }

        changeHealthCoroutine = StartCoroutine(ChangeHealthCor(newHealth, 0.35f));
    }

    IEnumerator ChangeHealthCor(float newHealth, float transitionDuration)
    {
        changingHealth = true;

        damageText.DOFade(1, transitionDuration / 4);
        damageText.text = "-" + (health - newHealth).ToString("0");
        healthSlider.DOValue(newHealth, transitionDuration).SetEase(Ease.Linear);
        sliderImg.DOColor(damageColor, transitionDuration / 4).SetEase(Ease.OutExpo);
        DOTween.To(() => health, x => health = x, newHealth, transitionDuration).SetEase(Ease.Linear);
        yield return new WaitForSeconds(transitionDuration / 2);
        yield return new WaitForSeconds(transitionDuration / 4);
        sliderImg.DOColor(barColor, transitionDuration / 4).SetEase(Ease.InExpo);
        damageText.DOFade(0, transitionDuration / 4);
        changingHealth = false;
        health = newHealth;
        healthText.text = health.ToString("0") + "%";

        changeHealthCoroutine = null;
    }
}
