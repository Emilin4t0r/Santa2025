using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;

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
    Transform mslLockCircle;
    public Transform radarTrackerParent;

    public float targetInfoUpdateFreq;
    float timeToUpdateTargetInfo;

    List <Transform> enemiesWithinGunsrange;
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        bc = BracketController.instance;
        ac = AirplaneController.instance;
        enemiesWithinGunsrange = new List <Transform>();
        irMissiles = GameObject.Find("IRMissiles").GetComponent<Missiles>();
        radarMissiles = GameObject.Find("RadarMissiles").GetComponent<Missiles>();
        SetActiveMissilesToIR(true);
        enemyLock.gameObject.SetActive(false);
        enemyLaunch.gameObject.SetActive(false);
        mslLock.text = "ACQUIRING";
        mslLock.gameObject.SetActive(false);
        mslLockCircle = mslLock.transform.GetChild(0);
        ResetMslLockCircle();
    }

    public void LoadHUDAfterBootup()
    {
        
    }

    public void SetActiveMissilesToIR(bool yes)
    {
        if (yes)
            activeMissiles = irMissiles;
        else
            activeMissiles = radarMissiles;
    }

    private void Update()
    {
        if (bc.lockedOn)
        {
            if (Time.time > timeToUpdateTargetInfo)
            {
                RefreshTargetInfo();
                timeToUpdateTargetInfo = Time.time + targetInfoUpdateFreq;
            }
            target.rectTransform.position = bc.GetComponent<RectTransform>().position;
        }
        else
        {
            target.text = "";
        }
    }

    private void FixedUpdate()
    {
        targetingComputerState.text = "TARGETING MODE:\n";
        targetingComputerState.text = HUD.instance.hudMode == HUD.HUDMode.AirToAir ? "AIR COMBAT" : "GROUND STRIKE";

        if (EnemiesController.enemiesAttacking.Count > 0)
        {            
            RearCamera.instance.TrackTarget(EnemiesController.enemiesAttacking[0].transform);
        }
        else
        {
            if (enemyLock.gameObject.activeSelf)
                enemyLock.gameObject.SetActive(false);
            RearCamera.instance.FreeCamera();
        }

        if (activeMissiles.seeking)
        {
            FlashMslLock("ACQUIRING");
            if (bc.lockedOn)
            {
                Transform reticle = null;
                foreach (Transform child in radarTrackerParent)
                {
                    if (child.GetComponent<RadarTracker>().target == bc.lockedOn)
                    {
                        reticle = child;
                    }
                }
                MissileLockCircleOnTarget(reticle);
            }
        }
        else if (activeMissiles.lockedOn)
        {
            if (!mslLock.gameObject.activeSelf)
                mslLock.gameObject.SetActive(true);
            mslLock.text = "LOCK";
            Transform reticle = null;
            foreach (Transform child in radarTrackerParent)
            {
                if (child.GetComponent<RadarTracker>().target == activeMissiles.lockedOn)
                {
                    reticle = child;
                }
            }
            if (reticle != null) MissileLockCircleOnTarget(reticle);
        }
        else
        {
            if (mslLock.gameObject.activeSelf)
            {
                mslLock.gameObject.SetActive(false);
                ResetMslLockCircle();
            }
        }

        if (enemiesWithinGunsrange.Count > 0)
        {
            FlashEnemyLock();
        } else
        {
            if (enemyLock.gameObject.activeSelf)
                enemyLock.gameObject.SetActive(false);
        }
    }

    void ResetMslLockCircle()
    {
        mslLockCircle.localPosition = new Vector3(0, 60, 0);
    }
    void MissileLockCircleOnTarget(Transform reticle)
    {
        Vector3 worldPos = reticle.position;
        Vector3 localTargetPos = mslLockCircle.transform.parent.InverseTransformPoint(worldPos);
        mslLockCircle.localPosition = localTargetPos;
    }

    void RefreshTargetInfo()
    {
        float targetDist = Vector3.Distance(ac.transform.position, bc.lockedOn.transform.position);
        target.text = "TARGET\n" + ((int)targetDist).ToString("D4") + " m\n";
        float spd = bc.lockedOn.GetComponent<EnemySantaMove>().currentVelocity.magnitude * 3.6f;
        target.text += ((int)spd).ToString("D4") + " km/h\n";        
    }

    void FlashMslLock(string text)
    {
        mslLock.text = text;
        t_mLock += Time.fixedDeltaTime;
        if (t_mLock > f_mLock)
        {
            mslLock.gameObject.SetActive(!mslLock.gameObject.activeSelf);
            t_mLock = 0;            
        }
    }

    GameObject rwr_lockSound;
    public void ChangeEnemiesInGunrange(Transform enemy, bool remove)
    {
        if (enemy == null) return;
        if (enemiesWithinGunsrange == null) enemiesWithinGunsrange = new List<Transform>();

        if (!remove)
        {
            if (!enemiesWithinGunsrange.Contains(enemy))
                enemiesWithinGunsrange.Add(enemy);
        }
        else
        {
            if (enemiesWithinGunsrange.Contains(enemy))
                enemiesWithinGunsrange.Remove(enemy);
        }

        if (enemiesWithinGunsrange.Count == 1 && rwr_lockSound == null)
        {
            rwr_lockSound = SoundSpawner.SpawnSoundLoop(ac.transform.position, ac.transform, SoundLibrary.GetClip("rwr_lock"), 0, false, 0.5f);
        }
        else if (enemiesWithinGunsrange.Count == 0)
        {
            SoundSpawner.EndLoop(rwr_lockSound);
            rwr_lockSound = null;
        }
    }
    public bool IsInGunrangeEnemies(Transform enemy)
    {        
        return enemiesWithinGunsrange.Contains(enemy); ;
    }

    void FlashEnemyLock()
    {
        t_eLock += Time.fixedDeltaTime;
        if (t_eLock > f_eLock)
        {            
            enemyLock.gameObject.SetActive(!enemyLock.gameObject.activeSelf);
            t_eLock = 0;
            print("enemies in gun range: " + enemiesWithinGunsrange.Count);
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
            StartCoroutine(MissileWarning(Time.time + 1));
    }
    IEnumerator MissileWarning(float t_stopFlash)
    {
        missileWarningActive = true;
        SoundSpawner.SpawnSound(ac.transform.position, ac.transform, SoundLibrary.GetClip("rwr_missile"), 0, 0);
        while (Time.time < t_stopFlash)
        {
            FlashEnemyLaunch();
            yield return null;
        }
        enemyLaunch.gameObject.SetActive(false);
        missileWarningActive = false;
    }
}
