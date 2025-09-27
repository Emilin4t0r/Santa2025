using UnityEngine;

public class Hardpoint : MonoBehaviour
{
    public Missiles missiles;
    public Guns guns;
    public GameObject gunPrefab, missilePrefab;
    public enum SelectedWeapon { Gun, Missile }
    public SelectedWeapon weapon;

    private void Start()
    {
        GameObject wpn = null;
        switch (weapon)
        {
            case SelectedWeapon.Gun:
                wpn = Instantiate(gunPrefab, transform.position, gunPrefab.transform.rotation, transform);
                wpn.transform.rotation = transform.rotation;
                guns.guns.Add(wpn.transform.GetChild(0));
                guns.fullAmmo += 200;
                break;
            case SelectedWeapon.Missile:
                wpn = Instantiate(missilePrefab, transform.position, missilePrefab.transform.rotation, transform);
                wpn.transform.rotation = transform.rotation;
                missiles.missiles.Add(wpn.GetComponent<Missile>());
                break;
        }
    }
}
