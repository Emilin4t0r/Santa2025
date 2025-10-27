using System.Collections;
using System.Reflection;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class ABBullet : Bullet
{
    BracketController bc;
    float distFromLockedOn;

    private void Start()
    {
        bc = BracketController.instance;
    }

    private void FixedUpdate()
    {
        if (bc.lockedOn != null) {
            // If we have enemy locked, burst when we're closest to it
            CheckDistanceToTarget();
        } else
        {
            // No enemy locked, search for one using an overlapsphere
            DoOverlapSphereSearch();
        }
    }

    void CheckDistanceToTarget()
    {
        float dist = Vector3.Distance(transform.position, bc.lockedOn.transform.position);
        if (dist > distFromLockedOn && distFromLockedOn != 0)
        {
            // Bullet has passed enemy
            Detonate();
        }
        distFromLockedOn = dist;
    }

    void DoOverlapSphereSearch()
    {
        var cols = Physics.OverlapSphere(transform.position, damage);
        foreach (Collider col in cols)
        {
            if (col.CompareTag("Enemy"))
            {
                Detonate();
            }
        }
    }

    void Detonate()
    {
        var cols = Physics.OverlapSphere(transform.position, damage);
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
        float dmg = Mathf.Max((damage - dist) / 12, 0); // 12 is an arbitrary number to lower damage (janky as hell)
        enemy.GetHit(dmg);
        print("BURST, DMG: " + dmg + ", DIST: " + dist);        
    }
}
