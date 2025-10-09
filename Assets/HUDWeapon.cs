using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDWeapon : MonoBehaviour
{
    GameObject bracket;
    public TextMeshProUGUI ammoText;
    public Image wpnImg, bracketImg;

    public Color selectedColor, txtSelectedColor;
    public Color deselectedColor, txtDeselectedColor;

    private void Awake()
    {
        bracket = transform.Find("SelectedImg").gameObject;
        Deselect();
    }

    public void Deselect()
    {
        bracket.SetActive(false);
        ammoText.color = txtDeselectedColor;
        wpnImg.color = deselectedColor;
        bracketImg.color = deselectedColor;
    }
    public void Select()
    {
        bracket.SetActive(true);
        ammoText.color = txtSelectedColor;
        wpnImg.color = selectedColor;
        bracketImg.color = selectedColor;
    }
    public void SetAmmo(int ammo)
    {
        ammoText.text = ammo.ToString();
    }
}
