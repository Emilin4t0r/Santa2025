using System.Collections;
using System.Reflection;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

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
            DoAirburst(bc.lockedOn.GetComponent<EnemySantaUtils>(), distFromLockedOn);
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
                float dist = Vector3.Distance(transform.position, col.transform.position);
                DoAirburst(col.GetComponent<EnemySantaUtils>(), dist);                
                break;
            }
        }
    }

    void DoAirburst(EnemySantaUtils enemy, float distance)
    {
        float dmg = Mathf.Max((damage - distance) + 5, 0); // +5 to offset distance from enemy collider's edge to enemy's center
        enemy.GetHit(dmg);
        print("BURST, DMG: " + dmg + ", DIST: " + distance);
        KillBullet(false);
    }
}
