using UnityEngine;

public class WeaponsSelector : MonoBehaviour
{
    public Guns singleGuns, chainGuns;
    public Missiles irMissiles, radarMissiles;
    public SwarmMissiles swarmMissiles;

    int currentWeaponIndex;

    public enum ActiveWeapon { SingleGuns, ChainGuns, IRMissiles, RadarMissiles, SwarmMissiles }

    private void OnEnable()
    {
        SetActiveWeapon(ActiveWeapon.SingleGuns);
    }

    private void Update()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
       
        if (scroll > 0) // Scroll up
        {
            if (currentWeaponIndex < 4)
                currentWeaponIndex++;
            else
                currentWeaponIndex = 0;
            SetActiveWeapon((ActiveWeapon)currentWeaponIndex); print("Scrolled, " + scroll);
        }
        else if (scroll < 0) { // Scroll down
            if (currentWeaponIndex > 0)
                currentWeaponIndex--;
            else
                currentWeaponIndex = 4;
            SetActiveWeapon((ActiveWeapon)currentWeaponIndex); print("Scrolled, " + scroll);
        }
        
    }

    public void SetActiveWeapon(ActiveWeapon weapon)
    {
        print("Setting current weapon to " + weapon);
        switch (weapon)
        {
            case ActiveWeapon.SingleGuns:
                SetAllWeaponsInactive();
                singleGuns.enabled = true;
                break;
            case ActiveWeapon.ChainGuns:
                SetAllWeaponsInactive();
                chainGuns.enabled = true;
                break;
            case ActiveWeapon.IRMissiles:
                SetAllWeaponsInactive();
                irMissiles.enabled = true;
                break;
            case ActiveWeapon.RadarMissiles:
                SetAllWeaponsInactive();
                radarMissiles.enabled = true;
                break;
            case ActiveWeapon.SwarmMissiles:
                SetAllWeaponsInactive();
                swarmMissiles.enabled = true;
                break;
        }
    }

    void SetAllWeaponsInactive()
    {
        singleGuns.enabled = false;
        chainGuns.enabled = false;
        irMissiles.enabled = false;
        radarMissiles.enabled = false;
        swarmMissiles.enabled = false;
    }
}
