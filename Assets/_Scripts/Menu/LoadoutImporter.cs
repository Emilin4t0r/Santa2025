using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadoutImporter : MonoBehaviour
{
    public GameObject ddol;

    static GameObject weaponsDupe;

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
    }

    void OnSceneChanged(Scene old, Scene now)
    {
        if (now.name == "Gameplay Test")
            GiveWeaponsToAircraft(GameObject.Find("WeaponsParent").transform);
    }

    public void GiveWeaponsToAircraft(Transform weaponsParent)
    {
        weaponsDupe.transform.parent = weaponsParent;
        weaponsDupe.transform.position = weaponsParent.position;
        weaponsDupe.transform.rotation = weaponsParent.rotation;
    }
    
    public void StartGame()
    {
        SaveWeaponsToDDOL();
        SceneManager.LoadScene("Gameplay Test");
    }
}
