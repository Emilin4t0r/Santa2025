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
            SetActiveWeapon(WeaponType.SingleGuns);           
        }
    }

    private void Update()
    {
        if (!inGameScene)
            return;

        float scroll = Input.GetAxis("Mouse ScrollWheel");
       
        if (scroll > 0) // Scroll up
        {
            if (currentWeaponIndex < 5)
                currentWeaponIndex++;
            else
                currentWeaponIndex = 0;
            SetActiveWeapon((WeaponType)currentWeaponIndex);
        }
        else if (scroll < 0) { // Scroll down
            if (currentWeaponIndex > 0)
                currentWeaponIndex--;
            else
                currentWeaponIndex = 5;
            SetActiveWeapon((WeaponType)currentWeaponIndex);
        }
        
    }

    public void SetActiveWeapon(WeaponType weapon)
    {
        print("Setting current weapon to " + weapon);
        SetAllWeaponsInactive();
        switch (weapon)
        {
            case WeaponType.SingleGuns:
                singleGuns.enabled = true;
                break;
            case WeaponType.ChainGuns:
                chainGuns.enabled = true;
                break;
            case WeaponType.AirBurst:
                airBurst.enabled = true;
                break;
            case WeaponType.IRMissiles:
                irMissiles.enabled = true;
                break;
            case WeaponType.RadarMissiles:
                radarMissiles.enabled = true;
                break;
            case WeaponType.SwarmMissiles:                
                swarmMissiles.enabled = true;
                break;
        }        
        hudWeapons.SetSelectedHUDWeapon(weapon);
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
