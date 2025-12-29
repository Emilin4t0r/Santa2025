using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GyroReticle : MonoBehaviour
{
    public float aimDistance = 100f; // how far ahead the reticle is projected
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
        Vector3 velocity = rbac.linearVelocity;
        float speed = velocity.magnitude;
        if (speed < 0.05f)
            return; // don't move reticle when stationary

        Vector3 velocityDir = velocity / speed;
        Vector3 targetWorldPos = ac.transform.position + velocityDir * aimDistance;

        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(mainCam, targetWorldPos);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.GetComponent<RectTransform>(), screenPoint, mainCam, out Vector2 localPoint);

        // invert for HUD-style movement
        //localPoint = -localPoint;

        reticlePosition = Vector2.Lerp(reticlePosition, localPoint, reticleLagSmoothing);
        Vector2 offset = new Vector2(0, HUD.hudOffset);
        transform.localPosition = reticlePosition + offset;
    }
}
