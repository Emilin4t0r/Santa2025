using UnityEngine;

public class EnemyGunsRange : MonoBehaviour
{
    public bool readyToFire;
    public EnemySantaMove moveScript;
    public EnemySantaUtils utils;

    private void OnTriggerEnter(Collider other)
    {
        if (other == null || moveScript.target == null) return;
        
        if (moveScript.target != null && other.transform.parent == moveScript.target)
        {
            // schedule first shot and set ready flag
            utils.nextShootTime = Time.time + Random.Range(0.2f, 2f);
            readyToFire = true;
        }

        if (!other.CompareTag("AircraftTrigger")) return;

        // tell HUD that *this enemy* is now in gunrange
        if (TargetInfo.instance != null)
            TargetInfo.instance.ChangeEnemiesInGunrange(moveScript.transform, false);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other == null) return;
        
        if (moveScript.target != null && other.transform.parent == moveScript.target)
        {
            readyToFire = false;
        }

        if (!other.CompareTag("AircraftTrigger")) return;

        // tell HUD to remove this enemy from the list
        if (TargetInfo.instance != null)
            TargetInfo.instance.ChangeEnemiesInGunrange(moveScript.transform, true);
    }
}
