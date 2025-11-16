using UnityEngine;

public class EnemyGunsTargeter : MonoBehaviour
{
    public EnemySantaMove move;

    [Header("Rotation Limits")]
    public float pitchLimit = 5f; // X
    public float yawLimit = 5f;   // Y
    public float rollLimit = 5f;  // Z

    [Header("Leading / Projectile")]
    [Tooltip("Speed of the projectile (units/sec). If <= 0, no leading is applied.")]
    public float projectileSpeed = 40f;

    [Tooltip("If true, will try to read velocity from target's Rigidbody (preferred). Otherwise velocity is estimated.")]
    public bool useRigidbodyIfAvailable = true;

    [Tooltip("Optional Rigidbody attached to the shooter (for moving shooter lead). Leave null if shooter is stationary.")]
    public Rigidbody shooterRigidbody;

    // for velocity estimation if target has no Rigidbody
    Vector3 lastTargetPos;
    float lastSampleTime;

    void Start()
    {
        lastSampleTime = Time.time;
        if (move != null && move.target != null)
            lastTargetPos = move.target.position;
    }

    void LateUpdate()
    {
        if (move == null || move.target == null) return;

        Vector3 shooterPos = transform.position;
        Vector3 targetPos = move.target.position;

        // 1) Get target velocity
        Vector3 targetVel = Vector3.zero;
        if (useRigidbodyIfAvailable)
        {
            var rb = move.target.GetComponent<Rigidbody>();
            if (rb != null)
            {
                targetVel = rb.linearVelocity;
            }
            else
            {
                targetVel = EstimateTargetVelocity(targetPos);
            }
        }
        else
        {
            targetVel = EstimateTargetVelocity(targetPos);
        }

        // 2) Optionally get shooter's velocity
        Vector3 shooterVel = shooterRigidbody != null ? shooterRigidbody.linearVelocity : Vector3.zero;

        // 3) Compute intercept point
        Vector3 interceptPoint;
        if (projectileSpeed > 0f && TryComputeInterceptPoint(shooterPos, shooterVel, targetPos, targetVel, projectileSpeed, out interceptPoint))
        {
            // aim at intercept point
            AimAtPoint(interceptPoint);
        }
        else
        {
            // fallback to direct aim (no valid lead)
            AimAtPoint(targetPos);
        }

        // update sampled pos for next frame
        lastTargetPos = targetPos;
        lastSampleTime = Time.time;
    }

    Vector3 EstimateTargetVelocity(Vector3 currentPos)
    {
        float now = Time.time;
        float dt = now - lastSampleTime;
        if (dt <= Mathf.Epsilon) return Vector3.zero;
        return (currentPos - lastTargetPos) / dt;
    }

    bool TryComputeInterceptPoint(Vector3 shooterPos, Vector3 shooterVel, Vector3 targetPos, Vector3 targetVel, float projectileSpeed, out Vector3 interceptPoint)
    {
        // We want t >= 0 such that:
        // | (targetPos + targetVel * t) - (shooterPos + shooterVel * t) | = projectileSpeed * t
        // Let r = targetPos - shooterPos, v = targetVel - shooterVel
        // Solve |r + v t|^2 = (s t)^2 => (v·v - s^2) t^2 + 2 r·v t + r·r = 0

        interceptPoint = targetPos;
        Vector3 r = targetPos - shooterPos;
        Vector3 v = targetVel - shooterVel;
        float s = projectileSpeed;

        float a = Vector3.Dot(v, v) - s * s;
        float b = 2f * Vector3.Dot(r, v);
        float c = Vector3.Dot(r, r);
        float t = 0;
        // If a is approximately 0, the equation becomes linear: b t + c = 0
        if (Mathf.Abs(a) < 1e-6f)
        {
            if (Mathf.Abs(b) < 1e-6f)
            {
                // no relative motion; if already at shooter, t = 0 else no solution
                if (c <= 1e-6f)
                {
                    interceptPoint = targetPos;
                    return true;
                }
                return false;
            }
            t = -c / b;
            if (t > 0f)
            {
                interceptPoint = targetPos + targetVel * t;
                return true;
            }
            return false;
        }

        float disc = b * b - 4f * a * c;
        if (disc < 0f) return false; // no real roots -> no intercept

        float sqrtD = Mathf.Sqrt(disc);
        float t1 = (-b + sqrtD) / (2f * a);
        float t2 = (-b - sqrtD) / (2f * a);

        // choose smallest positive t
        t = float.MaxValue;
        if (t1 > 0f) t = Mathf.Min(t, t1);
        if (t2 > 0f) t = Mathf.Min(t, t2);

        if (t == float.MaxValue) return false;

        interceptPoint = targetPos + targetVel * t;
        return true;
    }

    void AimAtPoint(Vector3 worldPoint)
    {
        // Calculate desired rotation that looks at worldPoint
        Vector3 dir = worldPoint - transform.position;
        if (dir.sqrMagnitude < 1e-6f) return; // avoid NaNs

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
