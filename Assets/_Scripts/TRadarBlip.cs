using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TRadarBlip : MonoBehaviour
{
    public Transform target;
    AirplaneController ac;

    private void Start()
    {
        ac = AirplaneController.instance;
    }

    private void FixedUpdate()
    {
        Vector3 relativePosition = target.position - ac.transform.position;
        float distanceToEnemy = relativePosition.magnitude;

        // Convert the relative position to the local space of the player
        Vector3 localRelativePosition = ac.transform.InverseTransformPoint(target.position);

        // Scale the local relative position to fit within the radar sphere
        // Adjust the radar scale to increase the movement area of the blip
        float radarRadius = transform.parent.localScale.x / 2;
        float radarScale = radarRadius / (1000f / 2.0f); // Adjust this divisor as needed

        Vector3 scaledPosition = localRelativePosition * radarScale;

        // Update the enemy blip's position within the radar sphere
        transform.localPosition = scaledPosition;

        // Calculate the local rotation of the enemy ship relative to the player ship
        Quaternion localRotation = Quaternion.Inverse(ac.transform.rotation) * target.rotation;

        // Apply the local rotation to the blip
        transform.localRotation = localRotation;
    }
}
