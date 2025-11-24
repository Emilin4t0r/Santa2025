using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.GraphicsBuffer;

public class Missiles : MonoBehaviour
{
    AircraftUtils au;
    public List<Missile> missiles;
    public float timeToLock;
    [HideInInspector]
    public GameObject lockedOn;
    BracketController bc;
    public bool seeking;
    float lockTimer;

    GameObject acqSound, lockSound;
    public int hudWeaponIndex;
    HUDWeapons hudWeapons;
    HUDWeapon hud;
    public bool requireRadarLock = true;

    // References for seeking
    Targeter targeter;
    Transform radarTrackerParent;

    public GameObject missilePrefab;

    private void Awake()
    {
        missiles = new List<Missile>();
    }

    private void OnEnable()
    {
        SceneManager.activeSceneChanged += OnSceneChanged;
    }
    private void OnDisable()
    {
        SceneManager.activeSceneChanged -= OnSceneChanged;
    }
    void OnSceneChanged(Scene old, Scene now)
    {
        if (now.name == "Gameplay")
        {
            InitializeWeapon();
        }
    }

    public void InitializeWeapon()
    {
        seeking = false;
        bc = BracketController.instance;
        hudWeapons = GameObject.Find("HUDWeapons").GetComponent<HUDWeapons>();
        hud = hudWeapons.weapons[hudWeaponIndex].GetComponent<HUDWeapon>();
        GetMissilesFromChildren();
        au = AircraftUtils.instance;
        radarTrackerParent = GameObject.Find("RadarTrackers").transform;
        targeter = Targeter.instance;
    }

    void GetMissilesFromChildren()
    {
        missiles = new List<Missile>();
        Missile[] _missilesInChildren = GetComponentsInChildren<Missile>(true);

        foreach (Missile msl in _missilesInChildren)
        {
            missiles.Add(msl);
        }
        
        hud.SetAmmo(missiles.Count);
    }

    private void Update()
    {
        if (missiles.Count <= 0 || !au.turnedOn)
            return;
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (lockedOn)
            {
                FireMissile();
            }
            else
            {
                if (seeking)
                    return;
                if (requireRadarLock)
                {
                    SeekRadarLock();
                }
                else
                {
                    SeekIRLock();
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (requireRadarLock)
        {
            if (seeking)
            {
                if (bc.lockedOn != null && lockedOn == null)
                {
                    lockTimer += Time.fixedDeltaTime;
                    if (lockTimer > timeToLock)
                    {
                        lockedOn = bc.lockedOn;
                        seeking = false;
                    }
                }
                if (!bc.lockedOn && seeking)
                    seeking = false;
            }
            if (lockedOn)
            {
                if (bc.lockedOn != lockedOn)
                {
                    lockedOn = null;
                }
            }
        }
        else
        {
            if (seeking)
            {
                if (lockedOn == null)
                {
                    lockTimer += Time.fixedDeltaTime;
                    if (lockTimer > timeToLock)
                    {
                        if (Radar.instance.enemies.Count > 0)
                        {
                            if (bc.lockedOn)
                            {
                                lockedOn = bc.lockedOn;
                            }
                            else
                            {
                                // Get radar blip closest to canvas center
                                RectTransform origin = targeter.GetComponent<RectTransform>();
                                lockedOn = Helpers.GetClosestRadarBlip(origin, radarTrackerParent).GetComponent<RadarTracker>().target;
                            }
                            print("Locking finished, locked on: " + lockedOn?.name);

                            var tts = TooltipSpawner.instance;
                            tts.ShowTooltip(tts.tt_firemsl);

                            seeking = false;
                        }
                        else
                        {
                            seeking = false;
                        }
                    }
                }
            }
            if (lockedOn)
            {
                if (!Radar.instance.enemies.Contains(lockedOn))
                {
                    lockedOn = null;
                }
            }
        }

        if (seeking)
        {
            if (!acqSound)
                acqSound = SoundSpawner.SpawnSoundLoop(transform.position, transform, SoundLibrary.GetClip("missile_acq"));
            if (lockSound)
                SoundSpawner.EndLoop(lockSound);
        }
        else if (lockedOn)
        {
            if (!lockSound)
                lockSound = SoundSpawner.SpawnSoundLoop(transform.position, transform, SoundLibrary.GetClip("missile_lock"));
            if (acqSound)
                SoundSpawner.EndLoop(acqSound);
        }
        else
        {
            if (lockSound)
                SoundSpawner.EndLoop(lockSound);
            if (acqSound)
                SoundSpawner.EndLoop(acqSound);
        }
    }

    void SeekRadarLock()
    {
        if (!bc.lockedOn)
            return;
        StartSeek();
    }
    void SeekIRLock()
    {
        if (Radar.instance.enemies.Count == 0)
            return;
        StartSeek();
    }
    void StartSeek()
    {
        seeking = true;
        lockedOn = null;
        lockTimer = 0;
    }

    void FireMissile()
    {
        Missile msl = missiles[0];             
        msl.enabled = true;
        msl.target = lockedOn.transform;
        lockedOn = null;
        msl.transform.parent = null;
        missiles.Remove(msl);
        hud.SetAmmo(missiles.Count);
        EZCameraShake.CameraShaker.Instance.ShakeOnce(0.75f, 15, 0, 0.5f);
    }

    public void DeactivateWeapon()
    {
        lockedOn = null;
        seeking = false;
        SoundSpawner.EndLoop(lockSound);
        SoundSpawner.EndLoop(acqSound);
        enabled = false;
    }
}
