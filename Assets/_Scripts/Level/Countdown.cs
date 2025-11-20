using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Countdown : MonoBehaviour
{
    public Transform[] numbers;
    public GameObject[] explosions;
    public float startDelay, timeBetweenNumbers;

    void Start()
    {
        StartCoroutine(DoCountdown());        
    }


    IEnumerator DoCountdown()
    {
        yield return new WaitForSeconds(startDelay);
        SoundSpawner.SpawnSound(transform.position, transform, SoundLibrary.GetClip("countdown"), 0, 0);
        yield return new WaitForSeconds(0.25f); // give time for soundclip's "Three"
        DisplayNumber(numbers[2]);
        explosions[0].SetActive(true);
        Helpers.ExplosionSound(transform.position);
        yield return new WaitForSeconds(timeBetweenNumbers);
        DisplayNumber(numbers[1]);
        explosions[1].SetActive(true);
        Helpers.ExplosionSound(transform.position);
        yield return new WaitForSeconds(timeBetweenNumbers);
        DisplayNumber(numbers[0]);
        explosions[2].SetActive(true);
        yield return new WaitForSeconds(timeBetweenNumbers);
        explosions[3].SetActive(true);
        foreach (var e in explosions)
        {
            Destroy(e.gameObject, 7);
        }
    }

    void DisplayNumber(Transform num)
    {                
        StartCoroutine(NumberEffects(num));
    }
    IEnumerator NumberEffects(Transform num)
    {
        float halfDuration = timeBetweenNumbers / 2;
        num.gameObject.SetActive(true);
        num.localScale = Vector3.zero;
        num.DOScale(1, 1).SetEase(Ease.OutExpo);
        num.DOLocalRotate(new Vector3(0, -185, 0), timeBetweenNumbers);
        yield return new WaitForSeconds(timeBetweenNumbers);
        num.gameObject.SetActive(false);
    }
}
