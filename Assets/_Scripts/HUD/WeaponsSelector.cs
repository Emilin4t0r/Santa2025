using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WeaponsSelector : MonoBehaviour
{
    public Guns singleGuns, chainGuns, airBurst;
    public Missiles irMissiles, radarMissiles;
    public SwarmMissiles swarmMissiles;

    int currentWeaponIndex;
    [HideInInspector] public string currentWeaponName;
    HUDWeapons hudWeapons;
    bool inGameScene;

    bool startFunctionsExecuted;
    int amountOfWeapons = 0;

    public enum WeaponType { SingleGuns, ChainGuns, AirBurst, IRMissiles, RadarMissiles, SwarmMissiles }

    public event Action OnWeaponChanged;

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
            inGameScene = true;
        else
            inGameScene = false;

        if (inGameScene)
        {
            hudWeapons = GameObject.Find("HUDWeapons").GetComponent<HUDWeapons>();
        }
    }

    private void LateUpdate()
    {
        if (inGameScene && !startFunctionsExecuted)
        {
            ExcludeEmptyWeapons();
            SetActiveWeapon(0);
            startFunctionsExecuted = true;
        }
    }

    private void Update()
    {
        if (!inGameScene)
            return;

        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll > 0) // Scroll up
        {
            if (currentWeaponIndex < amountOfWeapons - 1)
                currentWeaponIndex++;
            else
                currentWeaponIndex = 0;
            SetActiveWeapon(currentWeaponIndex);
        }
        else if (scroll < 0)
        { // Scroll down
            if (currentWeaponIndex > 0)
                currentWeaponIndex--;
            else
                currentWeaponIndex = amountOfWeapons - 1;
            SetActiveWeapon(currentWeaponIndex);
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (hudWeapons.availableWeapons.Count > 0)
                SetActiveWeapon(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (hudWeapons.availableWeapons.Count > 1)
                SetActiveWeapon(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (hudWeapons.availableWeapons.Count > 2)
                SetActiveWeapon(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            if (hudWeapons.availableWeapons.Count > 3)
                SetActiveWeapon(3);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            if (hudWeapons.availableWeapons.Count > 4)
                SetActiveWeapon(4);
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            if (hudWeapons.availableWeapons.Count > 5)
                SetActiveWeapon(5);
        }

    }

    public void SetActiveWeapon(int weaponIndex)
    {
        SetAllWeaponsInactive();
        var hudWpn = hudWeapons.SetSelectedHUDWeapon(weaponIndex);
        switch (hudWpn.id)
        {
            case "CNN 30MM":
                singleGuns.enabled = true;
                currentWeaponName = "30MM";
                LeadReticle.instance.SetActiveGuns(singleGuns);
                break;
            case "CNN 20MM":
                chainGuns.enabled = true;
                currentWeaponName = "20MM";
                LeadReticle.instance.SetActiveGuns(chainGuns);
                break;
            case "CNN 100MM":
                airBurst.enabled = true;
                currentWeaponName = "100MM";
                LeadReticle.instance.SetActiveGuns(airBurst);
                break;
            case "MSL IR":
                irMissiles.enabled = true;
                currentWeaponName = "IR";
                TargetInfo.instance.SetActiveMissilesToIR(true);
                break;
            case "MSL RDR":
                radarMissiles.enabled = true;
                currentWeaponName = "RADAR";
                TargetInfo.instance.SetActiveMissilesToIR(false);
                break;
            case "MSL SWRM":
                currentWeaponName = "SWARM";
                swarmMissiles.enabled = true;
                break;
        }
        SoundSpawner.SpawnSound(transform.position, transform, SoundLibrary.GetClip("switch_weapon"), 0, 0.025f);
    }

    void ExcludeEmptyWeapons()
    {
        amountOfWeapons = 6;
        if (singleGuns.guns.Count == 0)
        {
            singleGuns.enabled = false;
            hudWeapons.ExcludeHUDWeapon(WeaponType.SingleGuns);
            amountOfWeapons--;
        }
        if (chainGuns.guns.Count == 0)
        {
            chainGuns.enabled = false;
            hudWeapons.ExcludeHUDWeapon(WeaponType.ChainGuns);
            amountOfWeapons--;
        }
        if (airBurst.guns.Count == 0)
        {
            airBurst.enabled = false;
            hudWeapons.ExcludeHUDWeapon(WeaponType.AirBurst);
            amountOfWeapons--;
        }
        if (irMissiles.missiles.Count == 0)
        {
            irMissiles.enabled = false;
            hudWeapons.ExcludeHUDWeapon(WeaponType.IRMissiles);
            amountOfWeapons--;
        }
        if (radarMissiles.missiles.Count == 0)
        {
            radarMissiles.enabled = false;
            hudWeapons.ExcludeHUDWeapon(WeaponType.RadarMissiles);
            amountOfWeapons--;
        }
        if (swarmMissiles.missiles.Count == 0)
        {
            swarmMissiles.enabled = false;
            hudWeapons.ExcludeHUDWeapon(WeaponType.SwarmMissiles);
            amountOfWeapons--;
        }

        for (int i = 0; i < hudWeapons.availableWeapons.Count; ++i)
        {
            hudWeapons.availableWeapons[i].SetHotkey(i + 1);
            hudWeapons.availableWeapons[i].transform.localPosition = hudWeapons.hudSlots[i].transform.localPosition;
            if (i > 2) // Opposite side hotkey number placement
            {
                Transform hkeyTrsf = hudWeapons.availableWeapons[i].hotkeyText.transform;
                hkeyTrsf.localPosition = new Vector3(30, hkeyTrsf.localPosition.y, hkeyTrsf.localPosition.z);
            }
        }
    }

    void SetAllWeaponsInactive()
    {
        singleGuns.enabled = false;
        chainGuns.enabled = false;
        airBurst.enabled = false;
        irMissiles.DeactivateWeapon();
        radarMissiles.DeactivateWeapon();
        swarmMissiles.enabled = false;
    }

    public bool IsCurrentWeaponGun()
    {
        if (currentWeaponName == "20MM" || currentWeaponName == "30MM" || currentWeaponName == "100MM")
            return true;
        else
            return false;
    }
}
