using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunToGroundReticle : MonoBehaviour
{
    public Transform gunTransform;
    float bulletSpeed = 1f; // Speed of the bullet
    public LayerMask groundLayerMask; // Layer mask for the ground
    RectTransform canvasRect;
    public RectTransform reticleTransform; // Reference to the reticle RectTransform

    private void Start()
    {
        bulletSpeed = Guns.instance.shootForce;
        canvasRect = GameObject.Find("HUD(Canvas)").GetComponent<RectTransform>();
        reticleTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        // Calculate initial velocity of the bullet
        Vector3 initialVelocity = gunTransform.forward * bulletSpeed;

        // Calculate time of flight
        float time = CalculateTimeOfFlight(initialVelocity.y);

        // Calculate predicted hit point
        Vector3 predictedHitPoint = CalculatePredictedHitPoint(initialVelocity, time);

        // Project predicted hit point onto the canvas
        Vector2 canvasPos = ProjectToCanvas(predictedHitPoint);

        // Update reticle position on the canvas
        UpdateReticlePosition(canvasPos);
    }

    float CalculateTimeOfFlight(float verticalInitialVelocity)
    {
        return (2 * verticalInitialVelocity) / Physics.gravity.y;
    }

    Vector3 CalculatePredictedHitPoint(Vector3 initialVelocity, float time)
    {
        // Assuming no air resistance, calculate predicted hit point using kinematic equations
        Vector3 predictedHitPoint = gunTransform.position + initialVelocity * time;

        // Raycast downwards to find the actual ground position
        RaycastHit hit;
        if (Physics.Raycast(predictedHitPoint, Vector3.down, out hit, Mathf.Infinity, groundLayerMask))
        {
            return hit.point;
        }

        // If no ground hit, return the predicted hit point
        return predictedHitPoint;
    }

    Vector2 ProjectToCanvas(Vector3 worldPosition)
    {
        // Convert world position to viewport space
        Vector3 viewportPos = Camera.main.WorldToViewportPoint(worldPosition);

        // Convert viewport position to canvas position
        Vector2 canvasPos = new Vector2(
            viewportPos.x * canvasRect.sizeDelta.x,
            viewportPos.y * canvasRect.sizeDelta.y
        );

        return canvasPos;
    }

    void UpdateReticlePosition(Vector2 canvasPos)
    {
        // Update reticle position on the canvas
        reticleTransform.anchoredPosition = canvasPos;
    }
}
