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
    public Vector2 damageRange;
    public float damageRadius;
    public float armingDelay = 0.7f;
    float armingTimer;
    bool armed;
    public GameObject explosion;
    public GameObject rotator;
    public GameObject pointLight;
    public Transform target;
    TrailRenderer trail;
    CapsuleCollider sc;
    Rigidbody rb;
    public float explosionEffectSize = 0.5f;

    public GameObject finsToHide;

    float blowUpTimer;

    [Header("Proportional Navigation")]
    [Tooltip("Navigation constant (N). Typical 2-5. Higher = more aggressive lead.")]
    public float navigationConstant = 3f;

    private void Start()
    {
        SoundSpawner.SpawnSound(transform.position, transform, SoundLibrary.GetClip("missile_launch"));
        trail = transform.GetComponentInChildren<TrailRenderer>();
        sc = GetComponent<CapsuleCollider>();
        rb = GetComponent<Rigidbody>();
        trail.enabled = true;
        sc.enabled = true;
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

        if (!armed && armingTimer < armingDelay)        
            armingTimer += Time.fixedDeltaTime;            
        else        
            armed = true;
        
            Vector3 shooterPos = transform.position;
        thrust += acceleration * Time.fixedDeltaTime;
        float missileSpeed = Mathf.Clamp(thrust + jetSpdOnLaunch, 10f, maxSpeed);

        Vector3 aimPoint = transform.position + transform.forward; // fallback aim

        if (target != null || !armed)
        {
            // get target pos & velocity (best available)
            Vector3 targetPos = target.position;
            Vector3 targetVel = GetTargetVelocity(target);

            // ---------- Proportional Navigation Guidance ----------
            // r: vector from missile to target
            Vector3 r = targetPos - transform.position;
            float rSqr = r.sqrMagnitude;
            if (rSqr < 0.0001f) rSqr = 0.0001f;
            Vector3 rHat = r.normalized;

            // missile velocity (current)
            Vector3 v_m = rb.linearVelocity;
            // if missile hasn't yet got a velocity magnitude, approximate it with forward*speed
            if (v_m.sqrMagnitude < 0.01f)
            {
                v_m = transform.forward * missileSpeed;
            }

            // AI copypaste math
            Vector3 v_rel = targetVel - v_m;
            Vector3 omega = Vector3.Cross(r, v_rel) / rSqr;
            Vector3 a_cmd = navigationConstant * Vector3.Cross(omega, v_m);
            Vector3 predictedVel = v_m + a_cmd * Time.fixedDeltaTime;
            Vector3 desiredForward = predictedVel.sqrMagnitude > 0.001f ? predictedVel.normalized : transform.forward;

            // optionally compute aimPoint for debug/visualization: a short point along desiredForward
            aimPoint = transform.position + desiredForward * 50f;

            // Rotate missile toward desiredForward using your turnSpeed limit
            Quaternion desiredRot = Quaternion.LookRotation(desiredForward);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, desiredRot, turnSpeed * Time.fixedDeltaTime);
        }
        else
        {
            // no target - keep current forward (aimPoint already set)
            aimPoint = transform.position + transform.forward;
        }

        // Check if close enough to detonate
        if (target != null)
        {
            float dist = Vector3.Distance(transform.position, target.position);
            if (dist <= damageRadius * 0.8f)
            {
                Detonate();
                return;
            }
        }

        // Move forward: set rigidbody linear velocity consistently using missileSpeed
        rb.linearVelocity = transform.forward * missileSpeed;

        // Rotate missile visually
        if (rotator != null)
            rotator.transform.Rotate(new Vector3(0, 0, -visualRotationSpeed * Time.fixedDeltaTime));

        blowUpTimer += Time.fixedDeltaTime;
        if (blowUpTimer > lifeTime)
            Explode();
    }
    
    Vector3 GetTargetVelocity(Transform t)
    {
        if (t == null) return Vector3.zero;
        var mv = t.GetComponent<EnemySantaMove>();
        if (mv != null) return mv.currentVelocity;
        var trgRb = t.GetComponent<Rigidbody>();
        if (trgRb != null) return trgRb.linearVelocity;
        return Vector3.zero;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            Explode();
        }
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
        print("Distance to target " + enemy.name + ": " + dist);
        bool insideRange = (damageRadius - dist) > 0;
        if (!insideRange)
            return;
        float dmg = Random.Range(damageRange.x, damageRange.y);
        enemy.GetHit(dmg);
        print("MSL DAMAGED: " + enemy.name + " DMG: " + dmg + " DIST: " + dist);        
    }

    void Explode()
    {
        GameObject expl = Instantiate(explosion, transform.position, Quaternion.identity);
        expl.transform.localScale *= explosionEffectSize;
        SoundSpawner.SpawnSound(transform.position, AirplaneController.instance.transform, SoundLibrary.GetClip("missile_explode"));
        Destroy(expl, 5);
        Destroy(gameObject);
    }    
}

