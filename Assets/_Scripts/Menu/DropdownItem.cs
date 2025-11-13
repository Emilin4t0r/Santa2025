using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Hardpoint;

public class DropdownItem : MonoBehaviour
{
    public TextMeshProUGUI label;
    public WeaponInfo weaponInfo;
    Transform mainCam;
    public Vector3 camCloseupPos, camCloseupRot;

    public Hardpoint hardpoint;
    TextMeshProUGUI itemLabel;

    private void Start()
    {
        mainCam = Camera.main.transform;
        itemLabel = transform.Find("Item Label").GetComponent<TextMeshProUGUI>();
    }

    public void OnHoverEnter()
    {
        string wpn = itemLabel.text;
        switch (wpn)
        {
            case "HACKAPEL":
                hardpoint.SpawnWeapon(WeaponType.Hackapel);
                break;
            case "PIKE":
                hardpoint.SpawnWeapon(WeaponType.Pike_Single);
                break;
            case "HURACÁN-S":
                hardpoint.SpawnWeapon(WeaponType.Huracán_Small);
                break;
            case "LANDSKNECHT":
                hardpoint.SpawnWeapon(WeaponType.Landsknecht);
                break;
            case "PIKE X2":
                hardpoint.SpawnWeapon(WeaponType.Pike_Double);
                break;
            case "HURACÁN-L":
                hardpoint.SpawnWeapon(WeaponType.Huracán_Pod);
                break;
            case "ARQUEBUS":
                hardpoint.SpawnWeapon(WeaponType.Arquebus);
                break;
            case "LONGBOW":
                hardpoint.SpawnWeapon(WeaponType.Longbow);
                break;
            default:
                hardpoint.SpawnWeapon(WeaponType.Empty);
                break;
        }

        weaponInfo.SetInfo(label.text);
        mainCam.DORotate(camCloseupRot, 0.5f);
        mainCam.DOMove(camCloseupPos, 0.5f);
        MenuTurntable.instance.transform.DORotate(new Vector3(0, 0, 0), 0.5f);        
    }
    public void OnHoverExit()
    {
        bool hadSelectedWeapon = hardpoint.ClearWeapon();
        if (hadSelectedWeapon)
            weaponInfo.SetInfo("EMPTY");
    }
}
