using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileShooter : MonoBehaviour
{
    public static MissileShooter instance;

    [HideInInspector]
    public List<Missile> missiles;
    public float timeToLock;
    [HideInInspector]
    public GameObject lockedOn;
    BracketController bc;
    public bool seeking;
    float lockTimer;

    GameObject acqSound, lockSound;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        seeking = false;
        bc = BracketController.instance;
        missiles = new List<Missile>();
        CheckMslAmt();
    }

    private void Update()
    {
        if (missiles.Count <= 0)
            return;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (lockedOn)
                FireMissile();
            else
                SeekLock();
        }
    }

    private void FixedUpdate()
    {
        if (seeking)
        {
            if (bc.lockedOn && !lockedOn)
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
        } else
        {
            if (lockSound)
                SoundSpawner.EndLoop(lockSound);
            if (acqSound)
                SoundSpawner.EndLoop(acqSound);
        }

    }

    void CheckMslAmt()
    {
        missiles = new List<Missile>();
        foreach (var m in transform.GetComponentsInChildren<Missile>())
        {
            missiles.Add(m);
        }
    }

    void SeekLock()
    {
        if (!bc.lockedOn)
            return;
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
        CheckMslAmt();
    }
}
