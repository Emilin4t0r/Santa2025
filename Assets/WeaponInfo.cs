using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor.Searcher;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class WeaponInfoItem
{
    public string name;
    public Sprite sprite;    
    public string description;
}

public class WeaponInfo : MonoBehaviour
{
    public List<WeaponInfoItem> weaponInfos;
    public TextMeshProUGUI infoText;
    public Image infoImage;

    public void SetInfo(string weaponName)
    {
        WeaponInfoItem info = weaponInfos.FirstOrDefault(i => i.name == weaponName);

        if (info != null)
        {
            infoText.text = info.description;
            infoImage.sprite = info.sprite;
        }
    }
}
