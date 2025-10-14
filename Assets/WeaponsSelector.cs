using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WeaponsSelector : MonoBehaviour
{
    public Guns singleGuns, chainGuns, airBurst;
    public Missiles irMissiles, radarMissiles;
    public SwarmMissiles swarmMissiles;

    int currentWeaponIndex;    
    HUDWeapons hudWeapons;
    bool inGameScene;

    bool startFunctionsExecuted;
    int amountOfWeapons = 0;

    public enum WeaponType { SingleGuns, ChainGuns, AirBurst, IRMissiles, RadarMissiles, SwarmMissiles }

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
        else if (scroll < 0) { // Scroll down
            if (currentWeaponIndex > 0)
                currentWeaponIndex--;
            else
                currentWeaponIndex = amountOfWeapons - 1;
            SetActiveWeapon(currentWeaponIndex);
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
                break;
            case "CNN 20MM":
                chainGuns.enabled = true;
                break;
            case "CNN 100MM":
                airBurst.enabled = true;
                break;
            case "MSL IR":
                irMissiles.enabled = true;
                TargetInfo.instance.SetActiveMissilesToIR(true);
                break;
            case "MSL RDR":
                radarMissiles.enabled = true;
                TargetInfo.instance.SetActiveMissilesToIR(false);
                break;
            case "MSL SWRM":
                swarmMissiles.enabled = true;
                break;
        }
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
    }

    void SetAllWeaponsInactive()
    {
        singleGuns.enabled = false;
        chainGuns.enabled = false;
        airBurst.enabled = false;
        irMissiles.enabled = false;
        radarMissiles.enabled = false;
        swarmMissiles.enabled = false;
    }
}
