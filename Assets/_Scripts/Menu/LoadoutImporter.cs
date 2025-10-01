using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadoutImporter : MonoBehaviour
{
    public GameObject weapons;
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
        weaponsDupe = Instantiate(weapons, ddol.transform);
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
    

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            SaveWeaponsToDDOL();
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            SceneManager.LoadScene("Gameplay Test");
        }
    }
}
