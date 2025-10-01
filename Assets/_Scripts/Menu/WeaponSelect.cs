using UnityEngine;
using TMPro;

public class WeaponSelect : MonoBehaviour
{
    public Hardpoint hardpoint;
    public Hardpoint.HardpointType hardpointType;    
    TMP_Dropdown dropdown;    

    private void Start()
    {
        dropdown = GetComponent<TMP_Dropdown>();
        dropdown.onValueChanged.AddListener(SelectWeapon);
    }

    void SelectWeapon(int value)
    {
        if (hardpointType == Hardpoint.HardpointType.Small)
        {
            switch (dropdown.value)
            {
                case 1:
                    hardpoint.SpawnWeapon(Hardpoint.WeaponType.Hackapel);
                    break;
                case 2:
                    hardpoint.SpawnWeapon(Hardpoint.WeaponType.Pike_Single);
                    break;
                case 3:
                    hardpoint.SpawnWeapon(Hardpoint.WeaponType.Huracán_Small);
                    break;
                default:
                    hardpoint.SpawnWeapon(Hardpoint.WeaponType.Empty);
                    break;

            }
        } else
        {
            switch (dropdown.value)
            {
                case 1:
                    hardpoint.SpawnWeapon(Hardpoint.WeaponType.Landsknecht);
                    break;
                case 2:
                    hardpoint.SpawnWeapon(Hardpoint.WeaponType.Pike_Double);
                    break;
                case 3:
                    hardpoint.SpawnWeapon(Hardpoint.WeaponType.Huracán_Pod);
                    break;
                case 4:
                    hardpoint.SpawnWeapon(Hardpoint.WeaponType.Arquebus);
                    break;
                case 5:
                    hardpoint.SpawnWeapon(Hardpoint.WeaponType.Longbow);
                    break;
                default:
                    hardpoint.SpawnWeapon(Hardpoint.WeaponType.Empty);
                    break;
            }
        }
    }
}
