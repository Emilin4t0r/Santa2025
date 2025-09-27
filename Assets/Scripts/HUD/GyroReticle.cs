using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GyroReticle : MonoBehaviour
{
    public Transform gun;
    public float aimDistance = 100f; // Distance from the gun to project the reticle
    public float reticleLagSmoothing = 0.1f; // Smoothing factor for reticle lag

    AirplaneController ac;
    Rigidbody rbac;
    
    Camera mainCam;
    Canvas canvas;
    Vector2 reticlePosition;

    private void Start()
    {
        mainCam = Camera.main;
        canvas = GameObject.Find("HUD(Canvas)").GetComponent<Canvas>();
        ac = AirplaneController.instance;
        rbac = ac.GetComponent<Rigidbody>();
        reticlePosition = transform.localPosition;
    }

    void FixedUpdate()
    {
        // Calculate the initial velocity of the bullet
        Vector3 initialVelocity = gun.forward * Shoot.instance.shootForce;

        // Get the airplane's velocity and angular velocity
        Vector3 airplaneVelocity = rbac.linearVelocity;
        Vector3 airplaneAngularVelocity = rbac.angularVelocity;

        // Predict the bullet position at the specified distance
        Vector3 targetPosition = PredictBulletPositionAtDistance(gun.position, initialVelocity, airplaneVelocity, airplaneAngularVelocity, aimDistance);

        // Convert the world position of the target to the local position on the canvas
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(mainCam, targetPosition);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.GetComponent<RectTransform>(), screenPoint, mainCam, out Vector2 localPoint);

        // Invert the localPoint to achieve the inverse movement
        localPoint = -localPoint;

        // Smoothly update the reticle position for a lag effect
        reticlePosition = Vector2.Lerp(reticlePosition, localPoint, reticleLagSmoothing);

        // Update the reticle position
        transform.localPosition = reticlePosition;
    }

    Vector3 PredictBulletPositionAtDistance(Vector3 initialPosition, Vector3 initialVelocity, Vector3 airplaneVelocity, Vector3 airplaneAngularVelocity, float distance)
    {
        // Calculate the time it takes for the bullet to travel the specified distance
        float time = distance / initialVelocity.magnitude;

        // Predict the rotation of the airplane after the specified time
        Quaternion futureRotation = Quaternion.Euler(airplaneAngularVelocity * Mathf.Rad2Deg * time);

        // Calculate the future direction of the gun
        Vector3 futureForward = futureRotation * gun.forward;

        // Calculate the horizontal displacement considering both initial and future velocities
        Vector3 horizontalDisplacement = (initialVelocity + airplaneVelocity) * time;

        // Calculate the vertical displacement due to gravity
        float verticalDisplacement = initialVelocity.y * time - 0.5f * 9.81f * time * time;

        // Combine horizontal and vertical displacements to get the predicted position
        Vector3 predictedPosition = initialPosition + new Vector3(horizontalDisplacement.x, verticalDisplacement, horizontalDisplacement.z);

        // Adjust predicted position based on future gun direction
        predictedPosition += futureForward * distance;

        return predictedPosition;
    }
}
