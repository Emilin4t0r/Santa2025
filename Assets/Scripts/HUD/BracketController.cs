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
    Targeter targeter;
    AirplaneController ac;
    Transform closestTemp;

    bool acquiringLock;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        ac = AirplaneController.instance;
        targeter = Targeter.instance;
    }

    private void Update()
    {
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
                        // Is locked target not visible anymore on the radar?
                        if (!IsObjectOnRadar(lockedOn.gameObject))
                        {
                            lockedOn = null;
                            bracketTarget = radarTrackerParent;
                            return;
                        }
                    }
                    else
                    {
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
        targetPos = reticle.localPosition + new Vector3(0, HUD.hudOffset, 0);

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
        lockedOn = null;
        if (radarTrackerParent.childCount == 0)
        {
            acquiringLock = false;
            lockedOn = null;
            return;
        }
        // Find radar tracker closest to center of canvas
        closestTemp = radarTrackerParent.GetChild(0);
        float bestDist = Vector3.Distance(transform.position, radarTrackerParent.GetChild(0).transform.position);
        foreach (var r in radarTrackerParent.GetComponentsInChildren<Transform>())
        {
            if (r == radarTrackerParent)
                continue;
            float dist = Vector3.Distance(targeter.transform.position, r.position);
            if (dist < bestDist)
            {
                bestDist = dist;
                closestTemp = r;
            }
        }
        targeter.StartFlash(1.5f, 0.1f);
        StartCoroutine(LockAcquireWaiter(1.5f));
    }

    IEnumerator LockAcquireWaiter(float timeToGetLock)
    {
        yield return new WaitForSeconds(timeToGetLock);
        try
        {
            acquiringLock = false;
            lockedOn = closestTemp.GetComponent<RadarTracker>().target;
            bracketTarget = closestTemp;
            targeter.EnableImg(true);
        }
        catch
        {
            acquiringLock = false;
            lockedOn = null;
            bracketTarget = radarTrackerParent;
            targeter.EnableImg(false);
        }
    }
}
