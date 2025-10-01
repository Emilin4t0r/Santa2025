using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Guns : MonoBehaviour
{
    public GameObject bulletPrefab;
    public GameObject muzzleFlashPrefab;
    public float shootForce, fireRate, inaccuracy;
    public float gunAnimSpeed;
    float originalGunAnimSpeed;
    float nextTimeToFire;
    public List<Transform> guns;

    GameObject shootLoopSound;
    public Transform shootSoundParent;
    float timeToClearSounds;

    [HideInInspector] public int ammoCount;
    bool inGameScene;

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
            inGameScene = true;
            GetGunsFromChildren();
        } else
        {
            inGameScene = false;
        }
    } 
    void GetGunsFromChildren()
    {
        guns = new List<Transform>();
        Transform[] _guns = GetComponentsInChildren<Transform>();

        foreach (Transform gun in _guns)
        {
            if (gun.CompareTag("Gun"))
                guns.Add(gun);
        }
    }

    void Update()
    {
        if (!inGameScene)
            return;

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
            foreach (var gun in guns)
            {
                //Gun animation
                var gmAnim = gun.gameObject.GetComponent<Animator>();
                originalGunAnimSpeed = gmAnim.speed;
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
        foreach (var gun in guns)
        {
            // Bullet spread calculations
            Vector3 deviation3D = Random.insideUnitCircle * inaccuracy;
            Quaternion rot = Quaternion.LookRotation(Vector3.forward + deviation3D);
            Vector3 fwd = gun.transform.rotation * rot * Vector3.forward;

            //Getting muzzle transform (really bad but I'm lazy)
            Transform muzzle = gun.GetChild(1);

            // Spawn bullet
            var bullet = Instantiate(bulletPrefab, muzzle.position, muzzle.transform.rotation, null);
            bullet.GetComponent<Rigidbody>().AddForce(fwd * shootForce, ForceMode.Impulse);
            Destroy(bullet, 5);

            // Spawn muzzle flash
            int doMzf = Random.Range(0, 3);
            if (doMzf == 0)
            {
                var mzf = Instantiate(muzzleFlashPrefab, muzzle.position, muzzle.transform.rotation, muzzle.transform);
                float rand = Random.Range(1.5f, 4f);
                mzf.transform.localScale = new Vector3(rand, rand, rand);
                Destroy(mzf, 0.02f);
            }
            ammoCount--;
        }

        nextTimeToFire = Time.time + fireRate + Random.Range(0.001f, 0.02f);
    }

    void StopBurst()
    {
        foreach (var gun in guns)
        {
            //Gun animation
            var gmAnim = gun.gameObject.GetComponent<Animator>();
            gmAnim.speed = originalGunAnimSpeed;
            gmAnim.SetBool("Fire", false);
        }
        if (shootLoopSound)
        {
            SoundSpawner.EndLoop(shootLoopSound);
            SoundSpawner.SpawnSound(transform.position, transform, SoundLibrary.GetClip("shoot_tail2"));
        }
    }
}
