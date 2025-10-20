using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SwarmMissiles : MonoBehaviour
{
    public List<SwarmMissile> missiles;
    public int hudWeaponIndex;
    HUDWeapons hudWeapons;
    HUDWeapon hud;

    public float fireRate;
    float nextTimeToFire;

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
        if (now.name == "Gameplay Test")
        {
            GetMissilesFromChildren();
            hudWeapons = GameObject.Find("HUDWeapons").GetComponent<HUDWeapons>();
            hud = hudWeapons.weapons[hudWeaponIndex].GetComponent<HUDWeapon>();
            hud.SetAmmo(missiles.Count);
        }
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
        if (missiles.Count <= 0)
            return;
        if (Input.GetKey(KeyCode.Space))
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
    }
}
