using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RadarTracker : MonoBehaviour
{
    public GameObject target;
    Camera mainCam;
    Canvas canvas;
    AirplaneController ac;
    BracketController bc;
    Image img;

    private void Start()
    {
        mainCam = Camera.main;
        canvas = GameObject.Find("HUD(Canvas)").GetComponent<Canvas>();
        img = GetComponent<Image>();
        ac = AirplaneController.instance;
        bc = BracketController.instance;
        SoundSpawner.SpawnSound(ac.transform.position, ac.transform, SoundLibrary.GetClip("rwr_target"), 0, false);
    }

    private void FixedUpdate()
    {
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, -ac.transform.localEulerAngles.z);
        if (target)
        {
            if (bc.lockedOn)
            {
                if (bc.lockedOn.gameObject == target)
                {
                    if (img.enabled)
                        img.enabled = false;
                }
                else
                {
                    if (!img.enabled)
                        img.enabled = true;
                }
            }            
            Vector2 screenPosition = ProjectTargetPointToScreen(target.transform.position);
            UpdateReticlePosition(screenPosition);
        }
        else
        {
            Radar.instance.enemies.Remove(gameObject);
            Destroy(gameObject);
        }
    }

    Vector2 ProjectTargetPointToScreen(Vector3 point)
    {
        Vector3 screenPoint = mainCam.WorldToScreenPoint(point);
        return new Vector2(screenPoint.x, screenPoint.y);
    }

    void UpdateReticlePosition(Vector2 screenPosition)
    {
        // Convert screen position to canvas position
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.GetComponent<RectTransform>(), screenPosition, mainCam, out Vector2 canvasPosition);
        transform.localPosition = canvasPosition + new Vector2(0, -HUD.hudOffset);
    }
}
