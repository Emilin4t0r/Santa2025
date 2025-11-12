using Unity.Burst.Intrinsics;
using UnityEngine;

public class SwarmMissile : MonoBehaviour
{
    [HideInInspector] public float jetSpdOnLaunch;
    public float thrust;
    public float acceleration;
    public float maxSpeed;
    public float turnSpeed;
    public float lifeTime = 5;
    public float visualRotationSpeed;
    public float deviationMagnitude;
    public float deviationFrequency;
    public Vector2 damageRange;
    public float damageRadius;
    public float armingDelay = 0.7f;
    float armingTimer;
    public GameObject explosion;
    GameObject rotator;
    public Transform target;
    TrailRenderer trail;
    SphereCollider sc;
    Rigidbody rb;
    GameObject pointLight;

    public GameObject finsToHide;
    public float explosionEffectSize = 0.5f;

    float blowUpTimer;
    float nextTimeToDeviate;
    public bool targetSet;
    SwarmMRadar swarmRadar;

    private void Start()
    {
        SoundSpawner.SpawnSound(transform.position, transform, SoundLibrary.GetClip("missile_launch"));
        rotator = transform.Find("MissileRotator").gameObject;
        trail = transform.GetComponentInChildren<TrailRenderer>();
        sc = GetComponent<SphereCollider>();
        rb = GetComponent<Rigidbody>();
        pointLight = transform.Find("Point Light").gameObject;
        trail.enabled = true;
        sc.enabled = true;
        rb.isKinematic = false;
        pointLight.SetActive(true);
        if (finsToHide != null)
        {
            finsToHide.SetActive(true);
        }

        // Give missile jet's velocity on launch
        var plane = AirplaneController.instance;
        rb.linearVelocity = plane.rb.linearVelocity;
        jetSpdOnLaunch = plane.rb.linearVelocity.magnitude;

        swarmRadar = GetComponent<SwarmMRadar>();
        swarmRadar.enabled = false;
        DeviateCourseRandomly();
    }

    private void FixedUpdate()
    {
        if (rb == null) return;

        if (!swarmRadar.enabled && armingTimer < armingDelay)
            armingTimer += Time.fixedDeltaTime;
        else
            swarmRadar.enabled = true;

        Vector3 shooterPos = transform.position;
        float missileSpeed = thrust + jetSpdOnLaunch; // magnitude of missile velocity
        thrust += acceleration * Time.fixedDeltaTime;
        missileSpeed = Mathf.Clamp(missileSpeed, 10, maxSpeed);

        Vector3 aimPoint;

        if (target == null)
        {
            if (targetSet)
            {
                // target was destroyed, reactivate Swarm Radar.
                swarmRadar.enabled = true;
                targetSet = false;
            }
            // no target: keep current forward direction
            aimPoint = transform.position + transform.forward;

            if (Time.time > nextTimeToDeviate)
            {
                nextTimeToDeviate = Time.time + deviationFrequency;
                DeviateCourseRandomly();
            }            
        }
        else
        {
            // target position and velocity
            Vector3 targetPos = target.position;
            Vector3 targetVel = target.forward * target.GetComponent<EnemySantaMove>().currentMoveSpeed;

            // Solve for intercept point
            Vector3 interceptPoint;
            bool hasSolution = FirstInterceptPoint(shooterPos, missileSpeed, targetPos, targetVel, out interceptPoint);

            if (hasSolution)
                aimPoint = interceptPoint;
            else
                aimPoint = targetPos; // fallback: pure pursuit
        }

        // Rotate front towards aimPoint using turnSpeed (degrees per second).
        Vector3 targetDir = (aimPoint - transform.position).normalized;
        if (targetDir.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(targetDir);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRot,
                turnSpeed * Time.fixedDeltaTime
            );
        }

        // Move forward
        if (swarmRadar.enabled)
        {
            rb.linearVelocity = transform.forward * missileSpeed;
        }
        else
        {
            float vertical = rb.linearVelocity.y;
            Vector3 horizontalVel = transform.forward;
            horizontalVel.y = 0; // ensure horizontal
            horizontalVel = horizontalVel.normalized * missileSpeed;
            rb.linearVelocity = new Vector3(horizontalVel.x, vertical, horizontalVel.z);
        }

        // Rotate missile visually
        rotator.transform.Rotate(new Vector3(0, 0, -visualRotationSpeed * Time.fixedDeltaTime));

        blowUpTimer += Time.fixedDeltaTime;
        if (blowUpTimer > lifeTime)
            Explode();        
    }

    /// <summary>
    /// Computes intercept point for a shooter at 'shooterPos' with projectile speed 'projSpeed'
    /// chasing a target at 'targetPos' moving with velocity 'targetVel'.
    /// Returns true and sets 'interceptPoint' if a positive intercept time exists; otherwise returns false.
    /// </summary>
    private bool FirstInterceptPoint(Vector3 shooterPos, float projSpeed, Vector3 targetPos, Vector3 targetVel, out Vector3 interceptPoint)
    {
        interceptPoint = targetPos;

        Vector3 r = targetPos - shooterPos;         // relative position
        Vector3 v = targetVel;                      // target velocity
        float s = projSpeed;                        // missile speed (assumed constant)

        float a = Vector3.Dot(v, v) - s * s;
        float b = 2f * Vector3.Dot(r, v);
        float c = Vector3.Dot(r, r);

        float t = 0;
        // Solve a*t^2 + b*t + c = 0 for t >= 0
        if (Mathf.Abs(a) < 1e-6f)
        {
            // Degenerate: a ~ 0 => linear equation b*t + c = 0
            if (Mathf.Abs(b) < 1e-6f)
            {
                // No relative motion: target stationary relative to missile origin, or indeterminate
                return false;
            }
            t = -c / b;
            if (t > 0f)
            {
                interceptPoint = targetPos + v * t;
                return true;
            }
            return false;
        }

        float discr = b * b - 4f * a * c;
        if (discr < 0f) return false; // no real solution => cannot intercept at given speed

        float sqrtD = Mathf.Sqrt(discr);

        // two roots
        float t1 = (-b + sqrtD) / (2f * a);
        float t2 = (-b - sqrtD) / (2f * a);

        // pick the smallest positive time
        t = float.MaxValue;
        if (t1 > 0f && t1 < t) t = t1;
        if (t2 > 0f && t2 < t) t = t2;

        if (t == float.MaxValue) return false; // no positive solution

        interceptPoint = targetPos + v * t;
        return true;
    }

    bool targetHasEnteredSphere;
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform == target)
        {
            targetHasEnteredSphere = true;
        }
        if (other.CompareTag("Ground"))
        {
            Explode();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.transform == target)
        {
            if (targetHasEnteredSphere)
            {
                Detonate();
            }
        }
    }

    void DeviateCourseRandomly()
    {
        Vector3 dev = Random.insideUnitSphere * deviationMagnitude;
        Vector3 newDir = (transform.forward + dev).normalized;
        transform.rotation = Quaternion.LookRotation(newDir, Vector3.up);
    }

    void Detonate()
    {
        var cols = Physics.OverlapSphere(transform.position, damageRadius);
        foreach (Collider col in cols)
        {
            if (col.CompareTag("Enemy"))
            {
                DamageEnemy(col.GetComponent<EnemySantaUtils>());
            }
        }
        Explode();
    }

    void DamageEnemy(EnemySantaUtils enemy)
    {
        if (enemy == null) return;
        float dist = Vector3.Distance(enemy.gameObject.transform.position, transform.position);
        bool insideRange = (damageRadius - dist) > 0;
        if (!insideRange)
            return;
        float dmg = Random.Range(damageRange.x, damageRange.y);
        enemy.GetHit(dmg);
        print("SWRM DAMAGED: " + enemy.name + " DMG: " + dmg + " DIST: " + dist);
    }

    void Explode()
    {
        GameObject expl = Instantiate(explosion, transform.position, Quaternion.identity);
        expl.transform.localScale *= explosionEffectSize;
        SoundSpawner.SpawnSound(transform.position, AirplaneController.instance.transform, SoundLibrary.GetClip("missile_explode"));
        Destroy(expl, 7);
        Destroy(gameObject);
    }
}
