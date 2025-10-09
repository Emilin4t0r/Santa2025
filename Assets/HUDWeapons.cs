using TMPro;
using UnityEngine;

public class HUDWeapons : MonoBehaviour
{
    public HUDWeapon[] weapons;
    TextMeshProUGUI HUDActiveWeaponText;

    private void Awake()
    {
        HUDActiveWeaponText = GameObject.Find("ACTIVEWEAPON").GetComponent<TextMeshProUGUI>();
    }

    public void SetSelectedHUDWeapon(WeaponsSelector.WeaponType weapon)
    {
        DeactivateAllHUDWeapons();
        weapons[(int)weapon].Select();
        string weaponHUDName = "";
        switch (weapon)
        {
            case WeaponsSelector.WeaponType.SingleGuns:
                weaponHUDName = "CNN 30MM";
                break;
            case WeaponsSelector.WeaponType.ChainGuns:
                weaponHUDName = "CNN 20MM";
                break;
            case WeaponsSelector.WeaponType.AirBurst:
                weaponHUDName = "CNN 100MM";
                break;
            case WeaponsSelector.WeaponType.IRMissiles:
                weaponHUDName = "MSL IR";
                break;
            case WeaponsSelector.WeaponType.RadarMissiles:
                weaponHUDName = "MSL RDR";
                break;
            case WeaponsSelector.WeaponType.SwarmMissiles:
                weaponHUDName = "MSL SWRM";
                break;
        }
        HUDActiveWeaponText.text = weaponHUDName;
    }

    void DeactivateAllHUDWeapons()
    {        
        foreach (var wpn in weapons)
        {
            wpn.Deselect();
        }
    }
}
