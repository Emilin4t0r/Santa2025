using UnityEngine;

public class WeaponsBackup : MonoBehaviour
{
    public static WeaponsBackup instance;

    private void Awake()
    {
        instance = this;
    }    

    public GameObject GetSingleWeapon(Hardpoint.WeaponType wpnType)
    {
        GameObject weapon = null;
        switch (wpnType)
        {
            case Hardpoint.WeaponType.Pike_Double:
                weapon = transform.GetChild(0).transform.Find("IRMissiles").gameObject;
                break;
            case Hardpoint.WeaponType.Longbow:
                weapon = transform.GetChild(0).transform.Find("RadarMissiles").gameObject;
                break;
            case Hardpoint.WeaponType.Huracán_Pod:
                weapon = transform.GetChild(0).transform.Find("SwarmMissiles").gameObject;
                break;
            case Hardpoint.WeaponType.Hackapel:
                weapon = transform.GetChild(0).transform.Find("SingleGuns").gameObject;
                break;
            case Hardpoint.WeaponType.Landsknecht:
                weapon = transform.GetChild(0).transform.Find("ChainGuns").gameObject;
                break;
            case Hardpoint.WeaponType.Arquebus:
                weapon = transform.GetChild(0).transform.Find("AirBurst").gameObject;
                break;
        }

        return weapon;
    }
}
