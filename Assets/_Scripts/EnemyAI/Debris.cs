using UnityEngine;

public class Debris : MonoBehaviour
{
    [Header("Force Settings")]
    public float minForce = 5f;
    public float maxForce = 15f;

    [Header("Torque Settings")]
    public float minTorque = 5f;
    public float maxTorque = 15f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Random direction
        Vector3 randomDirection = Random.onUnitSphere;

        // Random force magnitude
        float forceAmount = Random.Range(minForce, maxForce);
        Vector3 force = randomDirection * forceAmount;

        // Apply force
        rb.AddForce(force, ForceMode.Impulse);

        // Random torque
        Vector3 randomTorque = new Vector3(
            Random.Range(minTorque, maxTorque),
            Random.Range(minTorque, maxTorque),
            Random.Range(minTorque, maxTorque)
        );

        rb.AddTorque(randomTorque, ForceMode.Impulse);

        Destroy(gameObject, 10);
    }
}
