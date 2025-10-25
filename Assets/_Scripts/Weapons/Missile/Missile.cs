using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class Missile : MonoBehaviour
{
    [HideInInspector] public float jetSpdOnLaunch;
    public float thrust;
    public float acceleration;
    public float maxSpeed;
    public float turnSpeed;
    public float lifeTime = 5;
    public float visualRotationSpeed;
    public float maxDamage;
    public float proximityFuseRadius = 8f;
    public LayerMask enemyLayerMask;
    public GameObject explosion;
    public GameObject rotator;
    public GameObject pointLight;
    public Transform target;
    TrailRenderer trail;
    CapsuleCollider cc;
    Rigidbody rb;    

    public GameObject finsToHide;

    float blowUpTimer;
    float distToTarget;

    private void Start()
    {
        SoundSpawner.SpawnSound(transform.position, transform, SoundLibrary.GetClip("missile_launch"));
        trail = transform.GetComponentInChildren<TrailRenderer>();
        cc = GetComponent<CapsuleCollider>();
        rb = GetComponent<Rigidbody>();
        trail.enabled = true;
        cc.enabled = true;
        rb.isKinematic = false;
        pointLight.SetActive(true);
        if (finsToHide != null)
        {
            finsToHide.SetActive(true);
        }
        jetSpdOnLaunch = AirplaneController.instance.rb.linearVelocity.magnitude;
    }   

    void FixedUpdate()
    {
        if (rb == null) return;

        Vector3 shooterPos = transform.position;
        float missileSpeed = thrust + jetSpdOnLaunch; // magnitude of missile velocity
        thrust += acceleration * Time.fixedDeltaTime;
        missileSpeed = Mathf.Clamp(missileSpeed, 10, maxSpeed);

        Vector3 aimPoint;

        if (target == null)
        {
            // no target: keep current forward direction
            aimPoint = transform.position + transform.forward;
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

            CheckDistanceToTarget();
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

        // Move forward: set rigidbody linear velocity
        rb.linearVelocity = transform.forward * (thrust + jetSpdOnLaunch);

        // Rotate missile visually
        rotator.transform.Rotate(new Vector3(0, 0, -visualRotationSpeed * Time.fixedDeltaTime));

        blowUpTimer += Time.fixedDeltaTime;
        if (blowUpTimer > lifeTime)
            BlowUp();
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

    void CheckDistanceToTarget()
    {
        float dist = Vector3.Distance(transform.position, target.position);
        if (dist > distToTarget && distToTarget != 0)
        {
            // Missile has passed enemy
            DamageEnemy(target.GetComponent<EnemySantaUtils>());
        }
        distToTarget = dist;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            DamageEnemy(other.GetComponent<EnemySantaUtils>());
        }
        if (other.CompareTag("Ground"))
            BlowUp();
    }

    void DamageEnemy(EnemySantaUtils enemy)
    {
        if (enemy == null) return;
        float dist = Vector3.Distance(enemy.gameObject.transform.position, transform.position);
        float dmg = Mathf.Max((maxDamage - dist) + 5, 0); // +5 to offset distance from enemy collider's edge to enemy's center
        enemy.GetHit(dmg);
        print("MSL DAMAGED: " + enemy.name + " DMG: " + dmg + " DIST: " + dist);
        BlowUp();
    }

    void BlowUp(float explSizeMultiplier = 0.5f)
    {
        GameObject expl = Instantiate(explosion, transform.position, Quaternion.identity);
        expl.transform.localScale *= explSizeMultiplier;
        SoundSpawner.SpawnSound(transform.position, AirplaneController.instance.transform, SoundLibrary.GetClip("missile_explode"));
        Destroy(expl, 1);
        Destroy(gameObject);
    }    
}

