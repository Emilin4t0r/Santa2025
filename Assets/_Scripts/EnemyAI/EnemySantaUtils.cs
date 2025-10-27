using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySantaUtils : MonoBehaviour
{    
    public List<Transform> gunMuzzles;
    public float shootForce;
    public float inaccuracy;
    public Vector2 shootFrequency;
    float shootTimer = 3;
    public GameObject bulletPrefab, muzzleFlashPrefab;
    public GameObject deathParticle;

    public float hitPoints = 10;

    public EnemyTrackCollider trackCollider;

    EnemySantaMove move;

    public event Action<float> OnHit;

    private void Start()
    {
        move = GetComponent<EnemySantaMove>();
    }

    private void Update()
    {
        //Shoot
        if (Time.time > shootTimer && trackCollider.readyToFire)
        {
            int shots = UnityEngine.Random.Range(4, 10);
            StartCoroutine(FireBurst(shots));
            float nextShootTime = Time.time + UnityEngine.Random.Range(shootFrequency.x, shootFrequency.y);
            shootTimer = nextShootTime;
        }
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
        print(gameObject.name + " got hit " + damage + ". health: " + (hitPoints - damage).ToString());
        if (hitPoints <= 0)
            return;
        hitPoints -= damage;

        if (hitPoints > 0)
            OnHit?.Invoke(hitPoints);

        if (hitPoints <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        SoundSpawner.SpawnSound(transform.position, transform.parent, SoundLibrary.GetClip("enemy_explode"));
        var partc = Instantiate(deathParticle, transform.position, transform.rotation);
        print(gameObject.name + " Dying... in radar: " + Radar.instance.enemies.Contains(gameObject));
        EnemiesController.enemiesAttacking.Remove(gameObject);
        Radar.instance.enemies.Remove(gameObject);
        print(gameObject.name + " removed from lists. in radar: " + Radar.instance.enemies.Contains(gameObject));
        Destroy(partc, 10);
        Destroy(gameObject);        
    }
}
