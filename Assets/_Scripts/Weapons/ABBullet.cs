using UnityEngine;


public class ABBullet : Bullet
{
    BracketController bc;
    float distFromLockedOn;
    GameObject altTarget;
    public float blastRadius;
    public float burstDamageMod;

    private void Start()
    {
        bc = BracketController.instance;
    }

    private void FixedUpdate()
    {
        if (bc.lockedOn != null) {
            // If we have enemy locked, burst when we're closest to it
            CheckDistanceToTarget(bc.lockedOn);
        } else
        {
            if (!altTarget)
            {
                // No enemy locked, search for one using an overlapsphere
                DoOverlapSphereSearch();
            } else
            {
                CheckDistanceToTarget(altTarget);
            }
        }
    }

    void CheckDistanceToTarget(GameObject target)
    {
        float dist = Vector3.Distance(transform.position, target.transform.position);
        if (dist > distFromLockedOn && distFromLockedOn != 0)
        {
            // Bullet has passed enemy
            Detonate();
        }
        distFromLockedOn = dist;
    }

    void DoOverlapSphereSearch()
    {
        var cols = Physics.OverlapSphere(transform.position, blastRadius);
        foreach (Collider col in cols)
        {
            if (col.CompareTag("Enemy"))
            {
                altTarget = col.gameObject;                
            }
        }
    }

    void Detonate()
    {
        var cols = Physics.OverlapSphere(transform.position, blastRadius);
        foreach (Collider col in cols)
        {
            if (col.CompareTag("Enemy"))
            {
                DoDamage(col.GetComponent<EnemySantaUtils>());
            }
        }
        KillBullet(false);
    }

    void DoDamage(EnemySantaUtils enemy)
    {
        float dist = Vector3.Distance(transform.position, enemy.transform.position);
        float dmg = Mathf.Max((blastRadius - dist) / burstDamageMod, 0);
        enemy.GetHit(dmg);
        print("BURST, DMG: " + dmg + ", DIST: " + dist);
    }
}
