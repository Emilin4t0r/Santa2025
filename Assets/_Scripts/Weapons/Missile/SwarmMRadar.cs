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
        Vector3 sphereCenter = transform.position + transform.forward * scanRadius;
        var cols = Physics.OverlapSphere(sphereCenter, scanRadius);
        foreach(Collider col in cols)
        {
            if (col.CompareTag("Enemy")) {
                missile.target = col.transform;
                missile.targetSet = true;
                enabled = false;
            }
        }
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Vector3 sphereCenter = transform.position + transform.forward * scanRadius;
        Gizmos.DrawWireSphere(sphereCenter, scanRadius);
    }
}
