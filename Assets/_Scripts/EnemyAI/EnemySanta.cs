using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySanta : MonoBehaviour
{
    public float maxSpeed = 200f; // Speed of movement
    public float moveAcceleration = 10f;
    public float currentMoveSpeed;
    public float maxTurnRate = 30f; // degrees per second    
    public float lateralDampen = 1f;
    public float flightAltitude = 5000;
    Vector3 previousPosition;
    public Vector3 currentVelocity; // Current velocity vector

    public List<Transform> gunMuzzles;
    public float shootForce;
    public float inaccuracy;
    public Vector2 shootFrequency;
    float shootTimer = 3;
    public GameObject bulletPrefab, muzzleFlashPrefab;
    public GameObject deathParticle;

    public float hitPoints = 10;

    public EnemyTrackCollider trackCollider;

    public Transform target;

    private void Start()
    {
        GetTarget();
        previousPosition = transform.position;
    }

    private void Update()
    {
        //Shoot
        if (Time.time > shootTimer && trackCollider.readyToFire)
        {
            int shots = Random.Range(4, 10);
            StartCoroutine(FireBurst(shots));
            float nextShootTime = Time.time + Random.Range(shootFrequency.x, shootFrequency.y);
            shootTimer = nextShootTime;
        }

        UpdateVelocity();

        if (target != null)
            Move();
        else
            GetTarget();
    }

    void GetTarget()
    {
        target = AllTargetsManager.instance.GetRandomTarget(transform);
        if (target.CompareTag("Player"))
        {
            SoundSpawner.SpawnSound(target.position, target, SoundLibrary.GetClip("rwr_lock"), 0, false);
            EnemiesController.enemiesAttacking.Add(gameObject);
        }
        trackCollider.target = target;
    }

    void Move()
    {
        // --- Rotation (turning) ---
        Vector3 toTarget = (target.position - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(toTarget);

        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRotation,
            maxTurnRate * Time.deltaTime
        );

        // --- Forward movement ---
        float distanceToDestination = Vector3.Distance(transform.position, target.position);
        float desiredMoveSpeed = Mathf.Clamp(maxSpeed * (distanceToDestination / 100), maxSpeed / 2, maxSpeed);
        currentMoveSpeed = Mathf.MoveTowards(currentMoveSpeed, desiredMoveSpeed, moveAcceleration * Time.deltaTime);
        Vector3 moveDirection = transform.forward * currentMoveSpeed * Time.deltaTime;

        transform.position += moveDirection;

        // --- Altitude adjustment ---
        LayerMask groundMask = LayerMask.GetMask("Ground");
        if (Physics.Raycast(transform.position + Vector3.up * 5000f, Vector3.down, out RaycastHit hit, 5000f, groundMask))
        {
            float targetY = hit.point.y + flightAltitude;
            Vector3 pos = transform.position;

            // Smooth vertical correction
            float altitudeChangeSpeed = 120f; // units per second, tweak this
            pos.y = Mathf.MoveTowards(pos.y, targetY, altitudeChangeSpeed * Time.deltaTime);

            transform.position = pos;
        }

        // Debug line to destination
        Debug.DrawLine(transform.position, target.position, Color.blue);
    }

    void UpdateVelocity()
    {
        currentVelocity = (transform.position - previousPosition) / Time.deltaTime;
        previousPosition = transform.position;
    }

    IEnumerator FireBurst(int shots)
    {
        TargetInfo.instance.TriggerMissileWarning();
        int shotsFired = 0;
        while (shotsFired < shots)
        {
            Fire();
            shotsFired++;
            yield return new WaitForSeconds(0.1f);
        }
        yield return null;
    }

    void Fire()
    {
        foreach (var gm in gunMuzzles)
        {
            // Bullet spread calculations
            Vector3 deviation3D = Random.insideUnitCircle * inaccuracy;
            Quaternion rot = Quaternion.LookRotation(Vector3.forward + deviation3D);
            Vector3 fwd = gm.transform.rotation * rot * Vector3.forward;

            // Spawn bullet
            var bullet = Instantiate(bulletPrefab, gm.position, gm.transform.rotation, null);
            bullet.GetComponent<Rigidbody>().AddForce(fwd * shootForce, ForceMode.Impulse);
            Destroy(bullet, 5);

            // Spawn muzzle flash
            int doMzf = Random.Range(0, 2);
            if (doMzf == 0)
            {
                var mzf = Instantiate(muzzleFlashPrefab, gm.position, gm.transform.rotation, gm.transform);
                float rand = Random.Range(2f, 3.5f);
                mzf.transform.localScale = new Vector3(rand, rand, rand);
                Destroy(mzf, 0.02f);
            }
        }
    }

    public void GetHit(float damage)
    {
        hitPoints -= damage;
        if (hitPoints <= 0)
        {
            Die();
        }
    }
    public void Die()
    {
        SoundSpawner.SpawnSound(transform.position, transform.parent, SoundLibrary.GetClip("enemy_explode"));
        var partc = Instantiate(deathParticle, transform.position, transform.rotation);
        EnemiesController.enemiesAttacking.Remove(gameObject);
        Destroy(partc, 1);
        Destroy(gameObject);        
    }

    private void OnDrawGizmos()
    {
    }
}
