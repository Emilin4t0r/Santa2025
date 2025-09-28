using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Radar : MonoBehaviour
{
    public static Radar instance;
    public List<GameObject> enemies;
    public GameObject radarTrackerUIPrefab;
    public GameObject radarTrackersParentUI;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        enemies = new List<GameObject>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            if (!enemies.Contains(other.gameObject))
            {
                enemies.Add(other.gameObject);
                var tracker = Instantiate(radarTrackerUIPrefab, radarTrackersParentUI.transform);
                tracker.transform.name = "Tracker" + other.gameObject.name;
                tracker.GetComponent<RadarTracker>().target = other.gameObject;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            if (enemies.Contains(other.gameObject))
            {
                enemies.Remove(other.gameObject);
                if (radarTrackersParentUI.transform.Find("Tracker" + other.gameObject.name)) {
                    Destroy(radarTrackersParentUI.transform.Find("Tracker" + other.gameObject.name).gameObject);
                }
            }
        }
    }
}
