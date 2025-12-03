using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Countermeasures : MonoBehaviour
{
    AircraftUtils au;
    public List<Transform> boxes;
    float nextTimeToFire;
    public float fireRate;
    public int ammoPerBox;
    [HideInInspector] public int ammoCount;
    int fullAmmo;
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
        if (now.name == "Gameplay")
        {
            InitializeWeapon();
        }
        else
        {
            inGameScene = false;
        }
    }

    public void InitializeWeapon()
    {
        GetBoxesFromChildren();        
        au = AircraftUtils.instance;
        inGameScene = true;
    }

    void GetBoxesFromChildren()
    {
        boxes = new List<Transform>();
        Transform[] transforms = GetComponentsInChildren<Transform>();
        foreach (Transform tr in transforms)
        {
            if (tr.CompareTag("Countermeasure"))
            {
                boxes.Add(tr);
                ammoCount += ammoPerBox;
            }
            fullAmmo = ammoCount;
        }
    }

    private void Update()
    {
        if (!inGameScene)
            return;

        if (!au.turnedOn)
            return;

        if (SettingsToggler.gamePaused) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (ammoCount <= 0)
            {
                SoundSpawner.SpawnSound(transform.position, transform, SoundLibrary.GetClip("empty_weapon"), 0, 0);
                return;
            }

            if (Time.time > nextTimeToFire)
            {
                FireFlare();
            }            
        }
    }

    void FireFlare()
    {
        nextTimeToFire = Time.time + fireRate;
        AddAmmo(-1);
        EZCameraShake.CameraShaker.Instance.ShakeOnce(0.4f, 15, 0, 0.5f);
    }

    void AddAmmo(int ammoToAdd)
    {
        ammoCount = Mathf.Max(ammoCount + ammoToAdd, 0);
    }

    public void ReloadGuns()
    {
        ammoCount = fullAmmo;
    }
}
