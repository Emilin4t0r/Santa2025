using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.GraphicsBuffer;

public class SwarmMissiles : MonoBehaviour
{
    AircraftUtils au;
    public List<SwarmMissile> missiles;
    public int hudWeaponIndex;
    HUDWeapons hudWeapons;
    HUDWeapon hud;

    public float fireRate;
    float nextTimeToFire;

    bool inGameScene;

    private void Awake()
    {
        missiles = new List<SwarmMissile>();
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
        } else
        {
            inGameScene = false;
        }
    }

    public void InitializeWeapon()
    {
        GetMissilesFromChildren();
        hudWeapons = GameObject.Find("HUDWeapons").GetComponent<HUDWeapons>();
        hud = hudWeapons.weapons[hudWeaponIndex].GetComponent<HUDWeapon>();
        hud.SetAmmo(missiles.Count);
        au = AircraftUtils.instance;
        inGameScene = true;
    }

    void GetMissilesFromChildren()
    {
        missiles = new List<SwarmMissile>();
        SwarmMissile[] _missiles = GetComponentsInChildren<SwarmMissile>();

        foreach (SwarmMissile msl in _missiles)
        {
            missiles.Add(msl);
        }
    }

    private void Update()
    {
        if (!inGameScene)
            return;

        if (!au.turnedOn)
            return;

        if (SettingsToggler.gamePaused) return;

        if (missiles.Count <= 0)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
                SoundSpawner.SpawnSound(transform.position, transform, SoundLibrary.GetClip("empty_weapon"), 0, 0);
            return;
        }

        if (Input.GetKey(KeyCode.Mouse0))
        {            
            if (Time.time > nextTimeToFire)
            {
                FireMissile();
                nextTimeToFire = Time.time + fireRate;
            }
        }
    }

    void FireMissile()
    {        
        SwarmMissile msl = missiles[0];        
        msl.enabled = true;
        msl.GetComponent<SwarmMRadar>().enabled = true;
        msl.transform.parent = null;
        missiles.Remove(msl);
        hud.SetAmmo(missiles.Count);
        EZCameraShake.CameraShaker.Instance.ShakeOnce(0.4f, 15, 0, 0.5f);
    }
}
