using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DropdownItem : MonoBehaviour
{
    public TextMeshProUGUI label;
    public WeaponInfo weaponInfo;

    public void OnHoverEnter()
    {
        weaponInfo.SetInfo(label.text);
    }
}
