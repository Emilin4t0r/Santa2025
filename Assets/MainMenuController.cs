using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    GameObject baseMenu, weaponSelect;
    
    public Image title, introBlackScreen;

    private void Start()
    {
        baseMenu = GameObject.Find("Menu");
        weaponSelect = GameObject.Find("WeaponSelect");
        weaponSelect.SetActive(false);        
    }

    public void ShowTitle()
    {
        title.gameObject.SetActive(true);
        StartCoroutine(IntroScreenAndFade());
    }
    IEnumerator IntroScreenAndFade()
    {
        yield return new WaitForSeconds(3f);
        title.DOColor(new Color(1, 1, 1, 0), 2f);
        introBlackScreen.DOColor(new Color(0, 0, 0, 0), 2f);
        yield return new WaitForSeconds(2.05f);
        title.gameObject.SetActive(false);
        introBlackScreen.gameObject.SetActive(false);
    }

    public void SwitchSubMenus()
    {
        if (baseMenu.activeSelf)
        {
            baseMenu.SetActive(false);
            weaponSelect.SetActive(true);
        } else
        {
            baseMenu.SetActive(true);
            weaponSelect.SetActive(false);
        }
    }
}
