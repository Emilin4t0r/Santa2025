using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinematicCam : MonoBehaviour
{
    [SerializeField] public Transform follow;
    [SerializeField] public float minFOV = 40f;      // Minimum zoom (closer)
    [SerializeField] public float maxFOV = 80f;      // Maximum zoom (farther)
    [SerializeField] public float minDistance = 5f;  // Distance where FOV is at minFOV
    [SerializeField] public float maxDistance = 30f; // Distance where FOV is at maxFOV
    [SerializeField] public float smoothSpeed = 5f;  // Smooth interpolation speed

    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        if (!follow) return;

        // Make sure the camera keeps looking at the target
        transform.LookAt(follow);

        // Measure distance
        float distance = Vector3.Distance(transform.position, follow.position);

        // Map distance to FOV (using inverse lerp)
        float t = Mathf.InverseLerp(minDistance, maxDistance, distance);
        float targetFOV = Mathf.Lerp(minFOV, maxFOV, t);

        // Smoothly interpolate FOV
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, Time.deltaTime * smoothSpeed);
    }
}
