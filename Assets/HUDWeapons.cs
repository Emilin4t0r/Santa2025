using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;

public class HUDWeapons : MonoBehaviour
{
    public List<HUDWeapon> weapons;
    [HideInInspector] public List<HUDWeapon> availableWeapons;
    TextMeshProUGUI HUDActiveWeaponText;

    private void Awake()
    {
        HUDActiveWeaponText = GameObject.Find("ACTIVEWEAPON").GetComponent<TextMeshProUGUI>();
        availableWeapons = new List<HUDWeapon>(weapons);
    }

    public HUDWeapon SetSelectedHUDWeapon(int weapon)
    {
        DeactivateAllHUDWeapons();
        var hudWpn = availableWeapons[weapon];
        hudWpn.Select();
        HUDActiveWeaponText.text = hudWpn.id;
        return hudWpn;
    }

    void DeactivateAllHUDWeapons()
    {        
        foreach (var wpn in weapons)
        {
            wpn.Deselect();
        }
    }

    public void ExcludeHUDWeapon(WeaponsSelector.WeaponType weapon)
    {
        switch (weapon)
        {
            case WeaponsSelector.WeaponType.SingleGuns:
                weapons[0].gameObject.SetActive(false);
                availableWeapons.Remove(weapons[0]);
                break;
            case WeaponsSelector.WeaponType.ChainGuns:
                weapons[1].gameObject.SetActive(false);
                availableWeapons.Remove(weapons[1]);
                break;
            case WeaponsSelector.WeaponType.AirBurst:
                weapons[2].gameObject.SetActive(false);
                availableWeapons.Remove(weapons[2]);
                break;
            case WeaponsSelector.WeaponType.IRMissiles:
                weapons[3].gameObject.SetActive(false);
                availableWeapons.Remove(weapons[3]);
                break;
            case WeaponsSelector.WeaponType.RadarMissiles:
                weapons[4].gameObject.SetActive(false);
                availableWeapons.Remove(weapons[4]);
                break;
            case WeaponsSelector.WeaponType.SwarmMissiles:
                weapons[5].gameObject.SetActive(false);
                availableWeapons.Remove(weapons[5]);                
                break;
        }
    }
}
