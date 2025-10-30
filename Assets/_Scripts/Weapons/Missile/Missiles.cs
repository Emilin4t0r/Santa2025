using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class Missiles : MonoBehaviour
{
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
        if (now.name == "Gameplay Test")
        {
            seeking = false;
            bc = BracketController.instance;
            GetMissilesFromChildren();
            hudWeapons = GameObject.Find("HUDWeapons").GetComponent<HUDWeapons>();
            hud = hudWeapons.weapons[hudWeaponIndex].GetComponent<HUDWeapon>();
            hud.SetAmmo(missiles.Count);
        }
    }

    void GetMissilesFromChildren()
    {
        missiles = new List<Missile>();
        Missile[] _missiles = GetComponentsInChildren<Missile>();

        foreach (Missile msl in _missiles)
        {
            missiles.Add(msl);
        }
    }

    private void Update()
    {
        if (missiles.Count <= 0)
            return;
        if (Input.GetKeyDown(KeyCode.Space))
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
                                lockedOn = bc.lockedOn;
                            else
                                lockedOn = Radar.instance.enemies.FirstOrDefault();
                            print("Locking finished, locked on: " + lockedOn?.name);
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
