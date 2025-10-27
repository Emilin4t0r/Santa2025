using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
using UnityEditor.UIElements;

public class BracketHealth : MonoBehaviour
{
    TextMeshProUGUI healthText;
    Slider healthSlider;
    Image sliderImg;

    float health;
    bool changingHealth;

    public Color barColor, damageColor;

    private void Awake()
    {
        healthText = GetComponentInChildren<TextMeshProUGUI>();
        healthSlider = GetComponentInChildren<Slider>();
        sliderImg = healthSlider.fillRect.GetComponent<Image>();
        sliderImg.color = barColor;
    }

    private void OnEnable()
    {
        sliderImg.color = barColor;
    }

    private void Update()
    {
        if (changingHealth)
        {
            healthText.text = health.ToString("0") + "%";
        }
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
        StartCoroutine(ChangeHealthCor(newHealth, 0.5f));
    }

    IEnumerator ChangeHealthCor(float newHealth, float transitionDuration)
    {
        changingHealth = true;
        healthSlider.DOValue(newHealth, transitionDuration).SetEase(Ease.Linear);
        sliderImg.DOColor(damageColor, transitionDuration / 4).SetEase(Ease.OutExpo);
        DOTween.To(() => health, x => health = x, newHealth, transitionDuration).SetEase(Ease.Linear);
        yield return new WaitForSeconds(transitionDuration / 2);
        sliderImg.DOColor(barColor, transitionDuration / 4).SetEase(Ease.InExpo);
        changingHealth = false;
        health = newHealth;
        healthText.text = health.ToString("0") + "%";
    }
}
