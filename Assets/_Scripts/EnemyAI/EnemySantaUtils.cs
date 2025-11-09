using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class EnemySantaUtils : MonoBehaviour
{
    public List<Transform> gunMuzzles;
    public float shootForce;
    public float inaccuracy;
    public Vector2 shootFrequency;
    public Vector2 shotsPerBurst;
    public float nextShootTime = 0;
    public GameObject bulletPrefab, muzzleFlashPrefab;
    public GameObject deathParticle;
    public float hitPoints = 100;
    public EnemyGunsRange trackCollider;
    public Transform aircraftVisual;
    public float visualRollAmt;
    public TrailRenderer trail;

    EnemySantaMove move;
    AirplaneController ac;
    public event Action<float> OnHit;
    GameObject shootLoopSound;
    bool firing;

    Vector3 spawnPoint, spawnRot;
    private void Start()
    {
        move = GetComponent<EnemySantaMove>();
        ac = AirplaneController.instance;
        spawnPoint = transform.position;
        spawnRot = transform.eulerAngles;
    }

    private void Update()
    {
        //Shoot
        if (Time.time > nextShootTime && trackCollider.readyToFire && !firing)
        {
            firing = true;
            int shots = UnityEngine.Random.Range((int)shotsPerBurst.x, (int)shotsPerBurst.y);
            StartCoroutine(FireBurst(shots));
            nextShootTime = Time.time + UnityEngine.Random.Range(shootFrequency.x, shootFrequency.y);
        }

        // Roll visuals
        float targetRoll = move.turnAmt * visualRollAmt;
        Vector3 currentRot = aircraftVisual.localEulerAngles;
        // Safety
        if (currentRot.z > 180f) currentRot.z -= 360f;
        float newRoll = Mathf.Lerp(currentRot.z, targetRoll, Time.deltaTime * 0.75f);
        aircraftVisual.localEulerAngles = new Vector3(0f, 0f, newRoll);
    }

    IEnumerator FireBurst(int shots)
    {
        if (move.target == ac.transform)
            TargetInfo.instance.TriggerEnemyFireWarning();
        shootLoopSound = SoundSpawner.SpawnSoundLoop(transform.position, transform, SoundLibrary.GetClip("shoot_loop2"), 1, false);
        int shotsFired = 0;
        while (shotsFired < shots)
        {
            Fire();
            shotsFired++;
            yield return new WaitForSeconds(0.1f);
        }
        if (shootLoopSound)
        {
            SoundSpawner.EndLoop(shootLoopSound);
            SoundSpawner.SpawnSound(transform.position, transform, SoundLibrary.GetClip("shoot_tail2"), 1, 0);
        }
        firing = false;
        yield return null;
    }

    void Fire()
    {
        foreach (var gm in gunMuzzles)
        {
            // Bullet spread calculations
            Vector3 deviation3D = UnityEngine.Random.insideUnitCircle * inaccuracy;
            Quaternion rot = Quaternion.LookRotation(Vector3.forward + deviation3D);
            Vector3 fwd = gm.transform.rotation * rot * Vector3.forward;

            // Spawn bullet
            var bullet = Instantiate(bulletPrefab, gm.position, gm.transform.rotation, null);
            bullet.GetComponent<Rigidbody>().AddForce(fwd * shootForce, ForceMode.Impulse);
            Destroy(bullet, 5);

            // Spawn muzzle flash
            int doMzf = UnityEngine.Random.Range(0, 2);
            if (doMzf == 0)
            {
                var mzf = Instantiate(muzzleFlashPrefab, gm.position, gm.transform.rotation, gm.transform);
                float rand = UnityEngine.Random.Range(2f, 3.5f);
                mzf.transform.localScale = new Vector3(rand, rand, rand);
                Destroy(mzf, 0.02f);
            }
        }
    }

    public void GetHit(float damage)
    {
        print(gameObject.name + " Getting hit with " + damage + " damage. HP before: " + hitPoints);
        if (hitPoints <= 0)
            return;
        hitPoints -= damage;
        OnHit?.Invoke(hitPoints);

        if (move.state != EnemySantaMove.AIState.Disengage && move.target != null)
        {
            var distanceToDestination = Vector3.Distance(transform.position, move.target.position);
            move.StartDisengage(distanceToDestination);
        }

        print(gameObject.name + " damaged, HP now: " + hitPoints);

        if (hitPoints <= 0)
        {
            Die();
        }
    }

    IEnumerator TurnTrailOffFor(float seconds)
    {
        trail.enabled = false;
        yield return new WaitForSeconds(seconds);
        trail.enabled = true;
    }

    public void Die()
    {
        SoundSpawner.SpawnSound(transform.position, transform.parent, SoundLibrary.GetClip("enemy_explode"), 0, 0.1f, 0.9f);
        var partc = Instantiate(deathParticle, transform.position, transform.rotation);
        EnemiesController.enemiesAttacking.Remove(gameObject);
        Radar.instance.enemies.Remove(gameObject);
        if (trackCollider.readyToFire)
            TargetInfo.instance.ChangeEnemiesInGunrange(transform, true);
        Destroy(partc, 10);
        TurnTrailOffFor(0.2f);
        move.target = null;
        transform.position = spawnPoint;
        transform.eulerAngles = spawnRot;
        move.currentMoveSpeed = 0;
        hitPoints = 100;        
    }
}
