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

    List<Transform> enemiesWithinGunsrange;
    bool enemyLockFlasherRunning = false;
    Coroutine enemyLockCoroutine = null;

    AircraftUtils au;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        bc = BracketController.instance;
        ac = AirplaneController.instance;
        enemiesWithinGunsrange = new List<Transform>();
        irMissiles = GameObject.Find("WeaponsDupe").transform.Find("IRMissiles").GetComponent<Missiles>();
        radarMissiles = GameObject.Find("WeaponsDupe").transform.Find("RadarMissiles").GetComponent<Missiles>();
        SetActiveMissilesToIR(true);
        enemyLock.gameObject.SetActive(false);
        enemyLaunch.gameObject.SetActive(false);
        mslLock.text = "ACQUIRING";
        mslLock.gameObject.SetActive(false);
        mslLockCircle = mslLock.transform.GetChild(0);
        au = AircraftUtils.instance;
        ResetMslLockCircle();
    }

    public void GetNewIRMsl()
    {
        irMissiles =  GameObject.Find("WeaponsDupe").transform.Find("IRMissiles(Clone)").GetComponent<Missiles>();
        SetActiveMissilesToIR(true);
    }
    public void GetNewRadarMsl()
    {
        radarMissiles = GameObject.Find("WeaponsDupe").transform.Find("RadarMissiles(Clone)").GetComponent<Missiles>();
        SetActiveMissilesToIR(false);
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

        if (au.turnedOn)
        {
            CheckForEnemyForRearCam();
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
    }

    void CheckForEnemyForRearCam()
    {
        if (EnemiesController.enemiesAttacking.Count > 0)
        {
            if (RearCamera.instance.trackTarget == null)
                RearCamera.instance.StartTrack(EnemiesController.enemiesAttacking[0].transform);
        }
        else
        {
            if (RearCamera.instance.trackTarget != null)
                RearCamera.instance.FreeCamera();
        }
        TargetDirectionIndicator.instance.CheckForThreats();
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

        if (enemiesWithinGunsrange.Count == 1)
        {
            // Spawn sound
            if (rwr_lockSound == null)
                rwr_lockSound = SoundSpawner.SpawnSoundLoop(ac.transform.position, ac.transform, SoundLibrary.GetClip("rwr_lock"), 0, false, 0.4f);

            // Start lock flash
            if (!enemyLockFlasherRunning)
            {
                enemyLockCoroutine = StartCoroutine(EnemyLockFlashLoop());
                enemyLockFlasherRunning = true;
            }
        }
        if (enemiesWithinGunsrange.Count == 0)
        {
            // End sound
            SoundSpawner.EndLoop(rwr_lockSound);
            rwr_lockSound = null;

            // Stop lock flash
            if (enemyLockFlasherRunning && enemyLockCoroutine != null)
            {
                StopCoroutine(enemyLockCoroutine);
                enemyLockCoroutine = null;
                enemyLockFlasherRunning = false;
            }

            // Make sure lock warning text is off
            if (enemyLock.gameObject.activeSelf)
            {
                enemyLock.gameObject.SetActive(false);
            }
        }
    }

    IEnumerator EnemyLockFlashLoop()
    {
        // Ensure text is initially visible
        enemyLock.gameObject.SetActive(true);

        // Small wait so we don't constantly turn it on and off for like 1 frame
        yield return new WaitForSeconds(0.25f);

        while (enemiesWithinGunsrange.Count > 0)
        {
            // On for f_eLock seconds
            enemyLock.gameObject.SetActive(true);
            yield return new WaitForSeconds(f_eLock);


            // Off for f_eLock seconds
            enemyLock.gameObject.SetActive(false);
            yield return new WaitForSeconds(f_eLock);
        }
        // Ensure off when we exit
        enemyLock.gameObject.SetActive(false);
        enemyLockFlasherRunning = false;
        enemyLockCoroutine = null;
    }

    void FlashEnemyFire()
    {
        t_eLaunch += Time.fixedDeltaTime;
        if (t_eLaunch > f_eLaunch)
        {
            enemyLaunch.gameObject.SetActive(!enemyLaunch.gameObject.activeSelf);
            t_eLaunch = 0;
        }
    }

    bool enemyFireWarningActive;
    public void TriggerEnemyFireWarning(Transform enemy)
    {
        if (!enemyFireWarningActive && enemyLockFlasherRunning)
        {
            StartCoroutine(EnemyFireWarning(Time.time + 1));
            if (enemy != EnemiesController.enemiesAttacking[0].transform) // If enemy isn't the one already being tracked:
                RearCamera.instance.StartTrack(enemy);
        }
    }
    IEnumerator EnemyFireWarning(float t_stopFlash)
    {
        enemyFireWarningActive = true;
        SoundSpawner.SpawnSound(ac.transform.position, ac.transform, SoundLibrary.GetClip("rwr_missile"), 0, 0);
        while (Time.time < t_stopFlash)
        {
            FlashEnemyFire();
            yield return null;
        }
        enemyLaunch.gameObject.SetActive(false);
        enemyFireWarningActive = false;
        CheckForEnemyForRearCam();
    }
}
