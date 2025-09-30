using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Hardpoint : MonoBehaviour
{
    public Missiles iRMissiles;
    public Missiles radarMissiles;
    public SwarmMissiles swarmMissiles;
    public Guns singleGuns, chainGuns;
    public GameObject Hackapel, Landsknecht, Pike_Single, Pike_Double, Huracán_Small, Huracán_Pod, Arquebus, Longbow;
    public enum HardpointType { Small, Large }
    public HardpointType type;
    public enum WeaponType { Empty, Hackapel, Landsknecht, Pike_Single, Pike_Double, Huracán_Small, Huracán_Pod, Arquebus, Longbow }
    public GameObject selectedWeapon;
    public bool hideMissileFins = false;


    List<WeaponType> compatibleWeapons;

    private void Start()
    {
        if (type == HardpointType.Small)
        {
            compatibleWeapons = new List<WeaponType> {
                WeaponType.Empty,
                WeaponType.Hackapel,
                WeaponType.Pike_Single,
                WeaponType.Huracán_Small,
            };
        }
        else if (type == HardpointType.Large)
        {
            compatibleWeapons = new List<WeaponType> {
                WeaponType.Empty,
                WeaponType.Landsknecht,
                WeaponType.Pike_Double,
                WeaponType.Huracán_Pod,
                WeaponType.Arquebus,
                WeaponType.Longbow
            };
        }
    }

    public void SpawnWeapon(WeaponType wpnType)
    {
        GameObject wpn = null;
        if (selectedWeapon != null)
            Destroy(selectedWeapon.gameObject);
        switch (wpnType)
        {
            case WeaponType.Hackapel:
                wpn = Instantiate(Hackapel, transform.position, transform.rotation, singleGuns.transform);
                singleGuns.guns.Add(wpn.transform);
                singleGuns.fullAmmo += 100;
                break;
            case WeaponType.Landsknecht:
                wpn = Instantiate(Landsknecht, transform.position, transform.rotation, chainGuns.transform);
                chainGuns.guns.Add(wpn.transform);
                chainGuns.fullAmmo += 200;
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
                if (transform.localRotation.z > 0)
                {
                    wpn.transform.localEulerAngles = new Vector3(0, 0, 270);
                    wpn.transform.localScale = new Vector3(-1, 1, 1);
                }
                else
                {
                    wpn.transform.localEulerAngles = new Vector3(0, 0, 90);
                }
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
                wpn = Instantiate(Arquebus, transform.position, transform.rotation, transform);
                singleGuns.guns.Add(wpn.transform);
                singleGuns.fullAmmo += 200;
                break;
            case WeaponType.Longbow:
                wpn = Instantiate(Longbow, transform.position, transform.rotation, radarMissiles.transform);
                if (hideMissileFins)
                {
                    wpn.GetComponent<Missile>().finsToHide.SetActive(false);
                }
                break;
            default:
                break;
        }
        selectedWeapon = wpn;
    }
}
