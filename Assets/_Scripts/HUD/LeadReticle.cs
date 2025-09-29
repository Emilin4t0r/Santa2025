using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeadReticle : MonoBehaviour
{
    Transform target; // Reference to the moving target
    public Transform gun; // Reference to the aircraft's gun
    Image img;
    Canvas canvas;

    Guns activeGuns;
    BracketController bc;

    private void Start()
    {
        canvas = GameObject.Find("HUD(Canvas)").GetComponent<Canvas>();
        bc = BracketController.instance;
        img = GetComponent<Image>();
        activeGuns = GameObject.Find("ChainGuns").GetComponent<Guns>();
    }

    private void Update()
    {
        if (!target)
        {
            if (bc.lockedOn)
            {
                target = bc.lockedOn.transform;
                img.enabled = true;
            }
            else
            {
                img.enabled = false;
                return;
            }
        }
        else
        {
            if (!bc.lockedOn)
            {
                target = null;
                img.enabled = false;
                return;
            }
        }

        // Calculate the predicted position of the target
        Vector3 predictedPosition = PredictTargetPosition(target.position, target.GetComponent<EnemySanta>().currentVelocity, activeGuns.transform.position, activeGuns.transform.forward, activeGuns.shootForce);

        // Convert the predicted position to canvas space
        Vector2 canvasPosition;
        if (ConvertToCanvasSpace(predictedPosition, out canvasPosition))
        {
            // Update the position of the lead reticle on the canvas
            Vector2 targetPosition = canvasPosition + new Vector2(0, -HUD.hudOffset);
            transform.localPosition = Vector2.Lerp(transform.localPosition, targetPosition, Time.deltaTime * 10f);
        }
    }

    Vector3 PredictTargetPosition(Vector3 targetPosition, Vector3 targetVelocity, Vector3 gunPosition, Vector3 gunDirection, float shootForce)
    {
        // Calculate the relative position of the target
        Vector3 relativePosition = targetPosition - gunPosition;

        // Calculate the time it takes for a projectile to reach the target
        float timeToTarget = relativePosition.magnitude / shootForce;

        // Predict the future position of the target based on its velocity and time to target
        Vector3 predictedPosition = targetPosition + targetVelocity * timeToTarget;

        // Return the predicted position
        return predictedPosition;
    }

    bool ConvertToCanvasSpace(Vector3 worldPosition, out Vector2 canvasPosition)
    {
        canvasPosition = Vector2.zero;

        // Convert world position to screen position
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);

        // Check if the position is within the camera's view
        if (screenPosition.z > 0)
        {
            // Convert screen position to canvas local position
            RectTransform canvasRect = canvas.GetComponent<RectTransform>();
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPosition, Camera.main, out canvasPosition))
            {
                return true;
            }
        }
        return false;

    }
}
