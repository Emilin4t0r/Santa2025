using UnityEngine;

public class EnemyGunsRange : MonoBehaviour
{
    public bool readyToFire;
    public EnemySantaMove moveScript;
    public EnemySantaUtils utils;

    private void OnTriggerEnter(Collider other)
    {
        if (!moveScript.target)
            return;

        if (other.transform.parent == moveScript.target)
        {
            utils.nextShootTime = Time.time + Random.Range(0.2f, 2f);
            readyToFire = true;
        }
        if (other.CompareTag("AircraftTrigger"))
        {
            TargetInfo.instance.AddEnemiesInGunrange(1);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (!moveScript.target)
            return;

        if (other.transform.parent == moveScript.target)
        {
            readyToFire = false;            
        }
        if (other.CompareTag("AircraftTrigger"))
        {            
            TargetInfo.instance.AddEnemiesInGunrange(-1);
        }
    }
}
