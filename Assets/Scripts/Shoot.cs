using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot : MonoBehaviour
{
    public static Shoot instance;

    public GameObject bulletPrefab;
    public GameObject muzzleFlashPrefab;
    public float shootForce, fireRate, inaccuracy;
    public float gunAnimSpeed;
    float nextTimeToFire;
    public List<Transform> gunMuzzles;

    GameObject shootLoopSound;
    Transform shootSoundParent;
    float timeToClearSounds;

    [HideInInspector]
    public int ammoCount;    
    public int fullAmmo;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        ammoCount = fullAmmo;
        shootSoundParent = transform.Find("ShootSoundParent");
    }

    void Update()
    {
        if (ammoCount <= 0)
        {
            if (shootSoundParent.childCount > 0)
            {
                StopBurst();
                ClearShootSounds();
                ammoCount = 0;
            }
            return;
        }
        if (Input.GetKey(KeyCode.Mouse0))
        {
            if (Time.time > nextTimeToFire)
            {
                Fire();
            }
            EZCameraShake.CameraShaker.Instance.ShakeOnce(0.05f, 15f, 0, 1f);
            timeToClearSounds = Time.time + 0.25f;
        } else
        {
            // Get rid of any residual sound objects
            if (shootSoundParent.childCount > 0)
            {
                if (Time.time > timeToClearSounds)
                {
                    ClearShootSounds();
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            foreach (var gm in gunMuzzles)
            {
                //Gun animation
                var gmAnim = gm.parent.gameObject.GetComponent<Animator>();
                gmAnim.speed = gunAnimSpeed;
                gmAnim.SetBool("Fire", true);
            }
            shootLoopSound = SoundSpawner.SpawnSoundLoop(transform.position, shootSoundParent, SoundLibrary.GetClip("shoot_loop2"));
        }
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            StopBurst();
        }
    }

    void ClearShootSounds()
    {
        foreach (var c in shootSoundParent.GetComponentsInChildren<Transform>())
        {
            try
            {
                if (c != shootSoundParent.transform)
                    Destroy(c.gameObject);
            }
            catch
            {
                continue;
            }
        }
    }

    void Fire()
    {
        foreach (var gm in gunMuzzles)
        {
            // Bullet spread calculations
            Vector3 deviation3D = Random.insideUnitCircle * inaccuracy;
            Quaternion rot = Quaternion.LookRotation(Vector3.forward + deviation3D);
            Vector3 fwd = gm.transform.rotation * rot * Vector3.forward;

            // Spawn bullet
            var bullet = Instantiate(bulletPrefab, gm.position, gm.transform.rotation, null);
            bullet.GetComponent<Rigidbody>().AddForce(fwd * shootForce, ForceMode.Impulse);
            Destroy(bullet, 5);

            // Spawn muzzle flash
            int doMzf = Random.Range(0, 3);
            if (doMzf == 0)
            {
                var mzf = Instantiate(muzzleFlashPrefab, gm.position, gm.transform.rotation, gm.transform);
                float rand = Random.Range(2f, 3.5f);
                mzf.transform.localScale = new Vector3(rand, rand, rand);
                Destroy(mzf, 0.02f);
            }
            ammoCount--;
        }

        nextTimeToFire = Time.time + fireRate + Random.Range(0.001f, 0.02f);
    }

    void StopBurst()
    {
        foreach (var gm in gunMuzzles)
        {
            //Gun animation
            var gmAnim = gm.parent.gameObject.GetComponent<Animator>();
            gmAnim.SetBool("Fire", false);
        }
        if (shootLoopSound)
        {
            SoundSpawner.EndLoop(shootLoopSound);
            SoundSpawner.SpawnSound(transform.position, transform, SoundLibrary.GetClip("shoot_tail2"));
        }
    }
}
