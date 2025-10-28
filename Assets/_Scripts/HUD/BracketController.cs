using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BracketController : MonoBehaviour
{
    public static BracketController instance;

    public RectTransform gtgReticle;
    public Transform radarTrackerParent;
    public Transform bracketTarget;
    public float bracketFollowSpeed;
    public GameObject lockedOn;
    public float lockTime;
    Targeter targeter;
    AirplaneController ac;
    Transform closestTemp;

    bool acquiringLock;
    BracketHealth health;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        ac = AirplaneController.instance;
        targeter = Targeter.instance;
        health = GetComponentInChildren<BracketHealth>();
        health.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            Time.timeScale = 0.1f;
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            Time.timeScale = 1;
        }

        switch (HUD.instance.hudMode)
        {
            case HUD.HUDMode.AirToGround:
                MoveBracket(gtgReticle);
                break;
            case HUD.HUDMode.AirToAir:

                if (Input.GetKeyDown(KeyCode.Mouse2))
                {
                    FindNewTarget();
                }

                if (!acquiringLock)
                {
                    if (lockedOn)
                    {
                        // Is locked target not visible anymore on the radar? (or destroyed)
                        if (!IsObjectOnRadar(lockedOn.gameObject))
                        {
                            ResetLock();
                            bracketTarget = radarTrackerParent;
                            return;
                        }
                    }
                    else
                    {
                        if (health.gameObject.activeSelf)
                            ResetLock();
                        bracketTarget = radarTrackerParent;                        
                        targeter.EnableImg(false);
                    }
                }

                if (bracketTarget)
                {
                    MoveBracket(bracketTarget);
                }
                break;
        }
    }

    void MoveBracket(Transform reticle)
    {
        Vector3 targetPos;
        if (reticle.name != "RadarTrackers")
            targetPos = reticle.localPosition + new Vector3(0, HUD.hudOffset, 0);
        else
            targetPos = reticle.localPosition;

        float step = Vector2.Distance(transform.localPosition, targetPos) * Time.deltaTime * bracketFollowSpeed;
        transform.localPosition = Vector2.MoveTowards(transform.localPosition, targetPos, step);
    }

    bool IsObjectOnRadar(GameObject target)
    {
        return Radar.instance.enemies.Contains(target);
    }

    void FindNewTarget()
    {
        acquiringLock = true;
        bracketTarget = radarTrackerParent;
        ResetLock();
        if (radarTrackerParent.childCount == 0)
        {
            acquiringLock = false;
            lockedOn = null;
            return;
        }
        // Find radar tracker closest to center of canvas
        closestTemp = radarTrackerParent.GetChild(0);
        float bestDist = Vector3.Distance(transform.position, closestTemp.transform.position);
        foreach (Transform child in radarTrackerParent)
        {
            if (child == radarTrackerParent)
                continue;
            float dist = Vector3.Distance(targeter.transform.position, child.position);
            if (dist < bestDist)
            {
                bestDist = dist;
                closestTemp = child;
            }
        }
        targeter.StartFlash(lockTime, 0.1f);
        StartCoroutine(LockAcquireWaiter(lockTime));
    }

    IEnumerator LockAcquireWaiter(float timeToGetLock)
    {
        var acqLoop = SoundSpawner.SpawnSoundLoop(transform.position, transform, SoundLibrary.GetClip("radar_acq"));
        yield return new WaitForSeconds(timeToGetLock);
        try
        {
            acquiringLock = false;
            lockedOn = closestTemp.GetComponent<RadarTracker>().target;
            bracketTarget = closestTemp;
            targeter.EnableImg(true);
            var enemyScript = closestTemp.GetComponent<RadarTracker>().target.GetComponent<EnemySantaUtils>();
            health.gameObject.SetActive(true);
            health.SetHealth(enemyScript.hitPoints);
            enemyScript.OnHit += health.ChangeHealth;
            SoundSpawner.SpawnSound(transform.position, transform, SoundLibrary.GetClip("radar_lock"), 0, false);
        }
        catch
        {
            acquiringLock = false;
            ResetLock();
            bracketTarget = radarTrackerParent;
            targeter.EnableImg(false);
        }
        SoundSpawner.EndLoop(acqLoop);        
    }

    void ResetLock()
    {
        if (lockedOn)
            lockedOn.GetComponent<EnemySantaUtils>().OnHit -= health.ChangeHealth;
        lockedOn = null;
        health.gameObject.SetActive(false);        
    }
}
