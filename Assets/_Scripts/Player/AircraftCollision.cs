using UnityEngine;

public class AircraftCollision : MonoBehaviour
{
    public WheelCollider[] wheels;
    public static bool isGrounded;

    private void FixedUpdate()
    {
        CheckGrounded();
    }

    void CheckGrounded()
    {
        foreach (var w in wheels)
        {
            if (w.isGrounded)
            {
                isGrounded = true;
                return;
            }
        }
        isGrounded = false;
    }
}
