using UnityEngine;

public class EnemyTrackCollider : MonoBehaviour
{
    public bool readyToFire;
    public Transform target;

    private void OnTriggerEnter(Collider other)
    {        
        if (!other.CompareTag("AircraftTrigger"))
            return;
        if (other.transform.parent == target)
        {
            print("Enemy in range");
            readyToFire = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("AircraftTrigger"))
            return;
        if (other.transform.parent == target)
        {
            print("Enemy not in range");
            readyToFire = false;
        }
    }
}
