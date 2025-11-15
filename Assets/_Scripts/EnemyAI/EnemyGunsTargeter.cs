using UnityEngine;

public class EnemyGunsTargeter : MonoBehaviour
{
    public EnemySantaMove move;

    [Header("Rotation Limits")]
    public float pitchLimit = 5f; // X
    public float yawLimit = 5f; // Y
    public float rollLimit = 5f; // Z

    void LateUpdate()
    {
        if (move.target == null) return;

        // Calculate the desired rotation that looks at the target
        Vector3 dir = move.target.position - transform.position;
        Quaternion desiredRot = Quaternion.LookRotation(dir);

        Quaternion localDesired;
        if (transform.parent != null)
            localDesired = Quaternion.Inverse(transform.parent.rotation) * desiredRot;
        else
            localDesired = desiredRot; // no parent

        // Work in Euler angles for clamping
        Vector3 euler = localDesired.eulerAngles;

        // unwrap angles (so 350 becomes -10)
        euler.x = NormalizeAngle(euler.x);
        euler.y = NormalizeAngle(euler.y);
        euler.z = NormalizeAngle(euler.z);

        // Clamp all axes
        euler.x = Mathf.Clamp(euler.x, -pitchLimit, pitchLimit);
        euler.y = Mathf.Clamp(euler.y, -yawLimit, yawLimit);
        euler.z = Mathf.Clamp(euler.z, -rollLimit, rollLimit);

        // Apply result
        transform.localRotation = Quaternion.Euler(euler);
    }

    float NormalizeAngle(float a)
    {
        if (a > 180f) a -= 360f;
        return a;
    }
}
