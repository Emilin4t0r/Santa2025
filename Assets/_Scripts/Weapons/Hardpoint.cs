using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Hardpoint : MonoBehaviour
{
    public Missiles iRMissiles;
    public Missiles radarMissiles;
    public SwarmMissiles swarmMissiles;
    public Guns singleGuns, chainGuns, abGuns;
    public GameObject countermeasuresParent;
    public GameObject Hackapel, Landsknecht, Pike_Single, Pike_Double, Huracán_Small, Huracán_Pod, Arquebus, Longbow, Countermeasures;
    public enum HardpointType { Small, Large }
    public HardpointType type;
    public enum WeaponType { Empty, Hackapel, Landsknecht, Pike_Single, Pike_Double, Huracán_Small, Huracán_Pod, Arquebus, Longbow, Countermeasures}
    public GameObject selectedWeapon;
    public bool hideMissileFins = false;

    public bool ClearWeapon()
    {
        if (selectedWeapon != null)
        {
            Destroy(selectedWeapon.gameObject);
            return true;
        }
        return false;
    }

    public void SpawnWeapon(WeaponType wpnType)
    {
        GameObject wpn = null;
        ClearWeapon();
        switch (wpnType)
        {
            case WeaponType.Hackapel:
                wpn = Instantiate(Hackapel, transform.position, transform.rotation, singleGuns.transform);
                break;
            case WeaponType.Landsknecht:
                wpn = Instantiate(Landsknecht, transform.position, transform.rotation, chainGuns.transform);
                break;
            case WeaponType.Pike_Single:
                wpn = Instantiate(Pike_Single, transform.position, transform.rotation, iRMissiles.transform);
                if (hideMissileFins)
                {
                    wpn.GetComponent<Missile>().finsToHide.SetActive(false);
                }
                break;
            case WeaponType.Pike_Double:
                wpn = Instantiate(Pike_Double, transform.position, Pike_Double.transform.rotation, transform);               
                wpn.transform.localEulerAngles = new Vector3(0, 0, 90);                               
                wpn.transform.parent = iRMissiles.transform;
                if (hideMissileFins)
                {
                    foreach (var m in wpn.GetComponent<IRMissileAssigner>().missiles)
                    {
                        m.finsToHide.SetActive(false);
                    }
                }
                break;
            case WeaponType.Huracán_Small:
                wpn = Instantiate(Huracán_Small, transform.position, transform.rotation, transform);                
                if (transform.localRotation.z > 0)
                {
                    wpn.transform.localScale = new Vector3(-1, 1, 1);
                }                
                wpn.transform.parent = swarmMissiles.transform;
                if (hideMissileFins)
                {
                    foreach(var m in wpn.GetComponent<SwarmMissileAssigner>().missiles)
                    {
                        m.finsToHide.SetActive(false);
                    }
                }
                break;
            case WeaponType.Huracán_Pod:
                wpn = Instantiate(Huracán_Pod, transform.position, transform.rotation, swarmMissiles.transform);
                if (hideMissileFins)
                {
                    foreach (var m in wpn.GetComponent<SwarmMissileAssigner>().missiles)
                    {
                        m.finsToHide.SetActive(false);
                    }
                }
                break;
            case WeaponType.Arquebus:
                wpn = Instantiate(Arquebus, transform.position, transform.rotation, abGuns.transform);
                break;
            case WeaponType.Longbow:
                wpn = Instantiate(Longbow, transform.position, transform.rotation, radarMissiles.transform);
                if (hideMissileFins)
                {
                    wpn.GetComponent<Missile>().finsToHide.SetActive(false);
                }
                break;
            case WeaponType.Countermeasures:
                wpn = Instantiate(Countermeasures, transform.position, transform.rotation, countermeasuresParent.transform);
                break;
            default:
                break;
        }
        selectedWeapon = wpn;
    }
}
