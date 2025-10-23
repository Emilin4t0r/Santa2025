using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Guns : MonoBehaviour
{
    public GameObject bulletPrefab;
    public float shootForce, fireRate, inaccuracy;
    public float gunAnimSpeed;
    float originalGunAnimSpeed;
    float nextTimeToFire;
    public List<Transform> guns;
    public Vector2 camShakeValues;

    GameObject shootLoopSound;
    public Transform shootSoundParent;
    float timeToClearSounds;
    public float origMzfScale;

    public int ammoPerGun;
    [HideInInspector] public int ammoCount;
    bool inGameScene;
    public int hudWeaponIndex;
    HUDWeapons hudWeapons;
    HUDWeapon hud;

    public string caliberForAudio;

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
            inGameScene = true;
        else
            inGameScene = false;

        if (inGameScene)
        {
            GetGunsFromChildren();
            hudWeapons = GameObject.Find("HUDWeapons").GetComponent<HUDWeapons>();
            hud = hudWeapons.weapons[hudWeaponIndex].GetComponent<HUDWeapon>();
            hud.SetAmmo(ammoCount);
        }
    }

    void GetGunsFromChildren()
    {
        guns = new List<Transform>();
        Transform[] transforms = GetComponentsInChildren<Transform>();
        foreach (Transform tr in transforms)
        {
            if (tr.CompareTag("Gun"))
            {
                guns.Add(tr);
                ammoCount += ammoPerGun;
            }
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
                hud.SetAmmo(ammoCount);
            }
            return;
        }
        if (Input.GetKey(KeyCode.Mouse0))
        {
            if (Time.time > nextTimeToFire)
            {
                Fire();                
            }
            EZCameraShake.CameraShaker.Instance.ShakeOnce(camShakeValues.x, camShakeValues.y, 0, 0.75f);
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
                var gAnim = gun.gameObject.GetComponent<Animator>();
                originalGunAnimSpeed = gAnim.speed;
                gAnim.speed = gunAnimSpeed;
                gAnim.SetBool("Fire", true);
            }
            if (SoundLibrary.GetClip(caliberForAudio + "_start") != null)
                SoundSpawner.SpawnSound(transform.position, shootSoundParent, SoundLibrary.GetClip(caliberForAudio + "_start"));
            shootLoopSound = SoundSpawner.SpawnSoundLoop(transform.position, shootSoundParent, SoundLibrary.GetClip(caliberForAudio + "_loop"));
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
                GameObject mfLight = muzzle.GetChild(0).gameObject;                
                float flashTime = 0.01f;
                StartCoroutine(FlashMuzzleLight(mfLight, flashTime));
            }
            ammoCount--;
            hud.SetAmmo(ammoCount);
        }

        nextTimeToFire = Time.time + fireRate;
    }

    IEnumerator FlashMuzzleLight(GameObject light, float upTime)
    {
        light.SetActive(true);
        GameObject mzf = light.transform.GetChild(0).gameObject;
        float rand = Random.Range(1f, 3f);        
        mzf.transform.localScale = new Vector3(origMzfScale, origMzfScale, origMzfScale) * rand;
        yield return new WaitForSeconds(upTime);
        mzf.transform.localScale = new Vector3(origMzfScale, origMzfScale, origMzfScale);
        light.SetActive(false);
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
            SoundSpawner.SpawnSound(transform.position, transform, SoundLibrary.GetClip(caliberForAudio + "_tail"), 0, false);
        }
    }
}
