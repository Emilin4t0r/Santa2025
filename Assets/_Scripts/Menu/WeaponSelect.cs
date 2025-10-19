using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using DG.Tweening;

public class WeaponSelect : MonoBehaviour
{
    TMP_Dropdown dropdown;
    Transform mainCam;

    private void Start()
    {
        mainCam = Camera.main.transform;
        dropdown = GetComponent<TMP_Dropdown>();
        dropdown.onValueChanged.AddListener(SelectWeapon);
    }

    
    void SelectWeapon(int value)
    {
        mainCam.DORotate(Vector3.zero, 0.5f);
        mainCam.DOMove(Vector3.zero, 0.5f);
    }
}
