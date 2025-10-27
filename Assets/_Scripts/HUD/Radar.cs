using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Radar : MonoBehaviour
{
    public static Radar instance;
    public List<GameObject> enemies;
    public GameObject radarTrackerUIPrefab;
    public GameObject radarTrackersParentUI;
    Transform airplane;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        enemies = new List<GameObject>();
        airplane = AirplaneController.instance.transform;
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
                var trackerScript = tracker.GetComponent<RadarTracker>();
                trackerScript.target = other.gameObject;
                var enemyScript = other.GetComponent<EnemySantaUtils>();
                enemyScript.OnHit += trackerScript.ChangeHealth;
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
                    var tracker = radarTrackersParentUI.transform.Find("Tracker" + other.gameObject.name).gameObject;
                    other.GetComponent<EnemySantaUtils>().OnHit -= tracker.GetComponent<RadarTracker>().ChangeHealth;
                    Destroy(tracker);
                    // Play sound for losing track
                }
            }
        }
    }
}
