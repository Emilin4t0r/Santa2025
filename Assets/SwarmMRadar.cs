using UnityEngine;

public class SwarmMRadar : MonoBehaviour
{
    public float scanFrequency;
    public float scanRadius;
    float nextTimeToScan;    
    SwarmMissile missile;

    private void Start()
    {
        missile = GetComponent<SwarmMissile>();
        nextTimeToScan = Time.time + 0.75f;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            missile.target = other.transform;
        }
    }

    private void FixedUpdate()
    {
        if (Time.time > nextTimeToScan)
        {
            ScanForEnemies();
            nextTimeToScan = Time.time + scanFrequency;
        }
    }

    void ScanForEnemies()
    {
        var cols = Physics.OverlapSphere(transform.position, scanRadius);
        foreach(Collider col in cols)
        {
            if (col.CompareTag("Enemy")) {
                missile.target = col.transform;
                enabled = false;
            }
        }
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, scanRadius);
    }
}
