using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadoutImporter : MonoBehaviour
{
    public GameObject ddol;

    static GameObject weaponsDupe;
    GameObject backupWeapons;

    private void OnEnable()
    {
        SceneManager.activeSceneChanged += OnSceneChanged;
    }
    private void OnDisable()
    {
        SceneManager.activeSceneChanged -= OnSceneChanged;
    }

    public void SaveWeaponsToDDOL()
    {
        GameObject weapons = GameObject.Find("Weapons");
        weaponsDupe = Instantiate(weapons, new Vector3(0, 1000, 0), Quaternion.identity, ddol.transform);
        weaponsDupe.name = "WeaponsDupe";
    }

    void OnSceneChanged(Scene old, Scene now)
    {
        if (now.name == "Gameplay")
            GiveWeaponsToAircraft(GameObject.Find("WeaponsParent").transform);
    }

    public void GiveWeaponsToAircraft(Transform weaponsParent)
    {        
        StartCoroutine(WeaponsBackuper());

        weaponsDupe.transform.parent = weaponsParent;
        weaponsDupe.transform.position = weaponsParent.position;
        weaponsDupe.transform.rotation = weaponsParent.rotation;
    }

    IEnumerator WeaponsBackuper()
    {
        backupWeapons = Instantiate(weaponsDupe, new Vector3(0, 1000, 0), Quaternion.identity, transform.Find("WeaponsBackup"));
        backupWeapons.SetActive(false);
        yield return null;
    }
    
    public void StartGame()
    {
        SaveWeaponsToDDOL();
        SceneManager.LoadScene("Gameplay");
    }
}
