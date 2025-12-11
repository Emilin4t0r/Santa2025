using DG.Tweening;
using TMPro;
using UnityEngine;

public class GraphicsDropdown : MonoBehaviour
{
    TMP_Dropdown dropdown;
    GraphicsSettings gSettings;

    private void Start()
    {
        gSettings = GameObject.Find("Settings").GetComponent<GraphicsSettings>();

        dropdown = GetComponent<TMP_Dropdown>();
        dropdown.onValueChanged.AddListener(SelectGPreset);
    }


    void SelectGPreset(int value)
    {
        gSettings.ApplyPreset(value);
    }
}
