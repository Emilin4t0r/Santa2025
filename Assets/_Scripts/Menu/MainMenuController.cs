using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    GameObject baseMenu, weaponSelect;
    
    public Image title, introBlackScreen;

    public GameObject settingsMenu, tutorial, noWeaponsWarning;
    public FadeBlack tutorialBlack;

    private void Start()
    {
        baseMenu = GameObject.Find("Menu");
        weaponSelect = GameObject.Find("WeaponSelect");
        weaponSelect.SetActive(false);        
    }

    public void ShowTitle(bool skip)
    {
        if (skip)
        {
            title.gameObject.SetActive(false);
            introBlackScreen.gameObject.SetActive(false);
        }
        else
        {
            title.gameObject.SetActive(true);
            StartCoroutine(IntroScreenAndFade());            
        }
    }
    IEnumerator IntroScreenAndFade()
    {
        yield return new WaitForSeconds(3f);
        title.DOColor(new Color(1, 1, 1, 0), 2f).SetEase(Ease.Linear);        
        yield return new WaitForSeconds(2.05f);
        title.gameObject.SetActive(false);
        introBlackScreen.DOColor(new Color(0, 0, 0, 0), 1f).SetEase(Ease.Linear);
        MenuTurntable.instance.spin = true;
        yield return new WaitForSeconds(1.05f);        
        introBlackScreen.gameObject.SetActive(false);
        Cursor.visible = true;
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

    public void ToggleSettingsMenu()
    {
        settingsMenu.SetActive(!settingsMenu.activeSelf);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void OpenTutorial()
    {
        // Check if has at least one offensive weapon:
        // Check weapons parent -> get all weapon parents, check for children (don't include sound parent)
        var wpnParent = GameObject.Find("Weapons");
        int offensiveWeapons = 0;
        foreach(Transform wpn in wpnParent.transform)
        {
            if (wpn.transform.name == "CountermeasuresParent" || wpn.transform.name == "Hardpoints")
                continue;

            foreach(Transform child in wpn.transform)
            {
                if (child.name != "ShootSoundParent")
                {
                    offensiveWeapons++;
                    print("found offensive weapon " + child.name);
                }
            }
        }

        if (offensiveWeapons > 0)
            StartCoroutine(TutorialOpener());
        else
            StartCoroutine(ShowNoWeaponsWarning());
    }
    IEnumerator ShowNoWeaponsWarning()
    {
        noWeaponsWarning.SetActive(true);
        yield return new WaitForSeconds(3);
        noWeaponsWarning.SetActive(false);
    }
    IEnumerator TutorialOpener()
    {
        tutorialBlack.DoFade();
        yield return new WaitForSeconds(1);
        tutorial.SetActive(true);
    }

    public void StartGame()
    {
        StartCoroutine(GameStarter());
    }
    IEnumerator GameStarter()
    {
        yield return new WaitForSeconds(1);
        GameObject.Find("LoadoutImporter").GetComponent<LoadoutImporter>().StartGame();
    }
}
