using System.Collections;
using UnityEngine;

public class WeaponsParent : MonoBehaviour
{
    public static WeaponsParent instance;
    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            ReplaceWeaponGameobject(Hardpoint.WeaponType.Pike_Double);
        }
    }

    public void ReplaceWeaponGameobject(Hardpoint.WeaponType wpnType)
    {
        Transform weaponToReplace = null;
        Transform weaponsObject = transform.GetChild(0);
        GameObject newWpn = null;
        switch (wpnType)
        {
            case Hardpoint.WeaponType.Pike_Double:
                weaponToReplace = Helpers.FindChildByPartialName(weaponsObject, "IRMissiles");
                newWpn = Instantiate(WeaponsBackup.instance.GetSingleWeapon(wpnType), weaponToReplace.position, weaponToReplace.rotation, weaponToReplace.parent);
                weaponsObject.GetComponent<WeaponsSelector>().irMissiles = newWpn.GetComponent<Missiles>();
                Destroy(weaponToReplace.gameObject);
                newWpn.GetComponent<Missiles>().InitializeWeapon();
                StartCoroutine(MissileCircleReset(isIR: true));
                break;
            case Hardpoint.WeaponType.Longbow:
                weaponToReplace = Helpers.FindChildByPartialName(weaponsObject, "RadarMissiles");
                newWpn = Instantiate(WeaponsBackup.instance.GetSingleWeapon(wpnType), weaponToReplace.position, weaponToReplace.rotation, weaponToReplace.parent);
                weaponsObject.GetComponent<WeaponsSelector>().radarMissiles = newWpn.GetComponent<Missiles>();
                Destroy(weaponToReplace.gameObject);
                newWpn.GetComponent<Missiles>().InitializeWeapon();                
                StartCoroutine(MissileCircleReset(isIR: false));
                break;
            case Hardpoint.WeaponType.Huracán_Pod:
                weaponToReplace = Helpers.FindChildByPartialName(weaponsObject, "SwarmMissiles");
                newWpn = Instantiate(WeaponsBackup.instance.GetSingleWeapon(wpnType), weaponToReplace.position, weaponToReplace.rotation, weaponToReplace.parent);
                weaponsObject.GetComponent<WeaponsSelector>().swarmMissiles = newWpn.GetComponent<SwarmMissiles>();
                Destroy(weaponToReplace.gameObject);
                break;
            case Hardpoint.WeaponType.Hackapel:
                weaponToReplace = Helpers.FindChildByPartialName(weaponsObject, "SingleGuns");
                newWpn = Instantiate(WeaponsBackup.instance.GetSingleWeapon(wpnType), weaponToReplace.position, weaponToReplace.rotation, weaponToReplace.parent);
                weaponsObject.GetComponent<WeaponsSelector>().singleGuns = newWpn.GetComponent<Guns>();
                Destroy(weaponToReplace.gameObject);
                break;
            case Hardpoint.WeaponType.Landsknecht:
                weaponToReplace = Helpers.FindChildByPartialName(weaponsObject, "ChainGuns");
                newWpn = Instantiate(WeaponsBackup.instance.GetSingleWeapon(wpnType), weaponToReplace.position, weaponToReplace.rotation, weaponToReplace.parent);
                weaponsObject.GetComponent<WeaponsSelector>().chainGuns = newWpn.GetComponent<Guns>();
                Destroy(weaponToReplace.gameObject);
                break;
            case Hardpoint.WeaponType.Arquebus:
                weaponToReplace = Helpers.FindChildByPartialName(weaponsObject, "AirBurst");
                newWpn = Instantiate(WeaponsBackup.instance.GetSingleWeapon(wpnType), weaponToReplace.position, weaponToReplace.rotation, weaponToReplace.parent);
                weaponsObject.GetComponent<WeaponsSelector>().airBurst = newWpn.GetComponent<Guns>();
                Destroy(weaponToReplace.gameObject);
                break;
        }                
    }

    IEnumerator MissileCircleReset(bool isIR)
    {
        yield return new WaitForEndOfFrame();
        if (isIR)
            TargetInfo.instance.GetNewIRMsl();
        else
            TargetInfo.instance.GetNewRadarMsl();
    }
}
