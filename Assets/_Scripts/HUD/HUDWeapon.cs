using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDWeapon : MonoBehaviour
{
    public string id;
    GameObject bracket;
    public TextMeshProUGUI ammoText, hotkeyText;
    public Image wpnImg, bracketImg;

    public Color selectedColor, txtSelectedColor;
    public Color deselectedColor, txtDeselectedColor;
    public bool outOfAmmo;

    private void Awake()
    {
        bracket = transform.Find("SelectedImg").gameObject;
        Deselect();
    }

    public void Deselect()
    {
        bracket.SetActive(false);
        DimColors(true);
    }
    public void Select()
    {
        bracket.SetActive(true);
        if (!outOfAmmo)
            DimColors(false);
    }
    public void DimColors(bool yes)
    {
        if (yes)
        {
            ammoText.color = txtDeselectedColor;
            wpnImg.color = deselectedColor;
            
        }
        else
        {
            ammoText.color = txtSelectedColor;
            wpnImg.color = selectedColor;
        }
    }
    public void SetAmmo(int ammo)
    {
        ammoText.text = ammo.ToString();
        if (ammo <= 0)
        {
            DimColors(true);
            outOfAmmo = true;
        } else
        {
            if (outOfAmmo)
            {
                outOfAmmo = false;
                DimColors(false);
            }
        }
    }
    public void SetHotkey(int hotkey)
    {
        hotkeyText.text = hotkey.ToString();
    }
}
