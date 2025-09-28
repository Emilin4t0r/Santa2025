using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailWiggler : MonoBehaviour
{
    Vector3 origLocalPos;
    public float frequency, magnitude;
    float nextSpotUpdateTime;

    private void Start()
    {
        origLocalPos = transform.localPosition;
    }
    void FixedUpdate()
    {
        MoveToNewSpot();
    }

    void MoveToNewSpot()
    {
        if (Time.time > nextSpotUpdateTime) {
            transform.localPosition = origLocalPos + Random.insideUnitSphere * magnitude;
            nextSpotUpdateTime = Time.time + frequency;
        }
    }
}
