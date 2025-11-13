using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RadarTracker : MonoBehaviour
{
    public GameObject target;
    public float targetFollowSpeed;
    Camera mainCam;
    Canvas canvas;
    AirplaneController ac;
    BracketController bc;
    Image img;
    TrackerHealth health;

    private void Start()
    {
        mainCam = Camera.main;
        canvas = GameObject.Find("HUD(Canvas)").GetComponent<Canvas>();
        img = GetComponent<Image>();
        ac = AirplaneController.instance;
        bc = BracketController.instance;
        health = GetComponentInChildren<TrackerHealth>();
        SetHealth(target.GetComponent<EnemySantaUtils>().hitPoints);
        SoundSpawner.SpawnSound(ac.transform.position, ac.transform, SoundLibrary.GetClip("threat_flash"), 0, 0, 0.5f);        
    }

    private void FixedUpdate()
    {
        if (target)
        {
            if (bc.lockedOn)
            {
                if (bc.lockedOn.gameObject == target)
                {
                    if (img.enabled)
                    {
                        img.enabled = false;
                        health.gameObject.SetActive(false);
                    }
                }
                else
                {
                    if (!img.enabled)
                    {
                        img.enabled = true;
                        health.gameObject.SetActive(true);
                    }
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

    public void SetHealth(float amount)
    {
        if (health.gameObject.activeSelf)
            health.SetHealth(amount);
    }
    public void ChangeHealth(float newHealth)
    {
        if (health.gameObject.activeSelf)
            health.ChangeHealth(newHealth, 0.7f);
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
        Vector2 targetPos = canvasPosition + new Vector2(0, -HUD.hudOffset);
        float step = Vector2.Distance(transform.localPosition, targetPos) * Time.deltaTime * targetFollowSpeed;
        transform.localPosition = Vector2.MoveTowards(transform.localPosition, targetPos, step);
    }
}
