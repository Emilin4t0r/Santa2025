using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TargetInfo : MonoBehaviour
{
    public static TargetInfo instance;

    BracketController bc;
    AirplaneController ac;
    Missiles irMissiles, radarMissiles;
    Missiles activeMissiles;

    public TextMeshProUGUI target, targetingComputerState, enemyLock, enemyLaunch, mslLock;
    float t_eLock, t_mLock, t_eLaunch;
    public float f_eLock, f_mLock, f_eLaunch;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        bc = BracketController.instance;
        ac = AirplaneController.instance;
        irMissiles = GameObject.Find("IRMissiles").GetComponent<Missiles>();
        radarMissiles = GameObject.Find("RadarMissiles").GetComponent<Missiles>();
        activeMissiles = irMissiles;
        enemyLock.gameObject.SetActive(false);
        enemyLaunch.gameObject.SetActive(false);
        mslLock.text = "ACQUIRING";
        mslLock.enabled = false;
    }
    private void FixedUpdate()
    {
        if (bc.lockedOn)
        {
            float targetDist = Vector3.Distance(ac.transform.position, bc.lockedOn.transform.position);
            target.text = "TARGET\n" + ((int)targetDist).ToString("D4") + " m\n";
            float spd = bc.lockedOn.GetComponent<EnemySanta>().currentVelocity.magnitude * 3.6f;
            target.text += ((int)spd).ToString("D4") + " km/h\n";
        } else
        {
            target.text = "";
        }

        targetingComputerState.text = "TARGETING MODE:\n";
        targetingComputerState.text = HUD.instance.hudMode == HUD.HUDMode.AirToAir ? "AIR COMBAT" : "GROUND STRIKE";

        if (EnemiesController.enemiesAttacking.Count > 0)
        {
            FlashEnemyLock();
            RearCamera.instance.TrackTarget(EnemiesController.enemiesAttacking[0].transform);
        } else
        {
            if (enemyLock.gameObject.activeSelf)
                enemyLock.gameObject.SetActive(false);
            RearCamera.instance.FreeCamera();
        }

        if (activeMissiles.seeking)
        {
            FlashMslLock("ACQUIRING");
        }
        else if (activeMissiles.lockedOn) 
        {
            if (!mslLock.enabled)
                mslLock.enabled = true;
            mslLock.text = "LOCK";
        } else
        {
            if (mslLock.enabled)
                mslLock.enabled = false;
        }
    }

    void FlashMslLock(string text)
    {
        mslLock.text = text;
        t_mLock += Time.fixedDeltaTime;
        if (t_mLock > f_mLock)
        {
            mslLock.enabled = !mslLock.enabled;
            t_mLock = 0;
        }
    }

    void FlashEnemyLock()
    {
        t_eLock += Time.fixedDeltaTime;
        if (t_eLock > f_eLock)
        {
            enemyLock.gameObject.SetActive(!enemyLock.gameObject.activeSelf);
            t_eLock = 0;
        }
    }

    void FlashEnemyLaunch()
    {
        t_eLaunch += Time.fixedDeltaTime;
        if (t_eLaunch > f_eLaunch)
        {
            enemyLaunch.gameObject.SetActive(!enemyLaunch.gameObject.activeSelf);
            t_eLaunch = 0;
        }
    }

    bool missileWarningActive;
    public void TriggerMissileWarning()
    {
        if (!missileWarningActive)
        StartCoroutine(MissileWarning(Time.time + 1.5f));
    }
    IEnumerator MissileWarning(float t_stopFlash)
    {
        missileWarningActive = true;
        while (Time.time < t_stopFlash) {
            FlashEnemyLaunch();
            yield return null;
        }
        enemyLaunch.gameObject.SetActive(false);
        missileWarningActive = false;
    }
}
