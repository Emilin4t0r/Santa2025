using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Hardpoint : MonoBehaviour
{
    public Missiles missiles;
    public Guns singleGuns, chainGuns;
    public GameObject Hackapel, Landsknecht, Pike_Single, Pike_Double, Hurac�n_Small, Hurac�n_Pod, Arquebus, Longbow;
    public enum HardpointType { Small, Large }
    public HardpointType type;
    public enum WeaponType { Empty, Hackapel, Landsknecht, Pike_Single, Pike_Double, Hurac�n_Small, Hurac�n_Pod, Arquebus, Longbow }
    public WeaponType selectedWeapon;

    List<WeaponType> compatibleWeapons;

    private void Start()
    {
        if (type == HardpointType.Small)
        {
            compatibleWeapons = new List<WeaponType> {
                WeaponType.Empty,
                WeaponType.Hackapel,
                WeaponType.Pike_Single,
                WeaponType.Hurac�n_Small,
            };
        }
        else if (type == HardpointType.Large)
        {
            compatibleWeapons = new List<WeaponType> {
                WeaponType.Empty,
                WeaponType.Landsknecht,
                WeaponType.Pike_Double,
                WeaponType.Hurac�n_Pod,
                WeaponType.Arquebus,
                WeaponType.Longbow
            };
        }
    }

    public void SpawnWeapon(WeaponType wpnType)
    {
        selectedWeapon = wpnType;
        GameObject wpn = null;
        if (transform.childCount > 0)
            Destroy(transform.GetChild(0).gameObject);
        switch (selectedWeapon)
        {
            case WeaponType.Hackapel:
                wpn = Instantiate(Hackapel, transform.position, transform.rotation, transform);
                singleGuns.guns.Add(wpn.transform);
                singleGuns.fullAmmo += 100;
                break;
            case WeaponType.Landsknecht:
                wpn = Instantiate(Landsknecht, transform.position, transform.rotation, transform);
                chainGuns.guns.Add(wpn.transform);
                chainGuns.fullAmmo += 200;
                break;
            case WeaponType.Pike_Single:
                wpn = Instantiate(Pike_Single, transform.position, transform.rotation, transform);
                missiles.missiles.Add(wpn.GetComponent<Missile>());
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
                missiles.missiles.Add(wpn.GetComponent<DoublePike>().missiles[0]);
                missiles.missiles.Add(wpn.GetComponent<DoublePike>().missiles[1]);
                break;
            case WeaponType.Hurac�n_Small:
                wpn = Instantiate(Hurac�n_Small, transform.position, transform.rotation, transform);
                missiles.missiles.Add(wpn.GetComponent<Missile>());
                break;
            case WeaponType.Hurac�n_Pod:
                wpn = Instantiate(Hurac�n_Pod, transform.position, transform.rotation, transform);
                missiles.missiles.Add(wpn.GetComponent<Missile>());
                break;
            case WeaponType.Arquebus:
                wpn = Instantiate(Arquebus, transform.position, transform.rotation, transform);
                singleGuns.guns.Add(wpn.transform);
                singleGuns.fullAmmo += 200;
                break;
            case WeaponType.Longbow:
                wpn = Instantiate(Longbow, transform.position, transform.rotation, transform);
                missiles.missiles.Add(wpn.GetComponent<Missile>());
                break;
            default:
                break;
        }
    }
}
