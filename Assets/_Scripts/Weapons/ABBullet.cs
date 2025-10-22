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
            CheckDistanceToTarget();
        } else
        {
            DoOverlapSphereSearch();
        }
    }

    void CheckDistanceToTarget()
    {
        float dist = Vector3.Distance(transform.position, bc.lockedOn.transform.position);
        if (dist > distFromLockedOn && distFromLockedOn != 0)
        {
            DoAirburst();
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
                float dmg = Mathf.Max(damage - dist, 0);
                col.GetComponent<EnemySantaUtils>().GetHit(dmg);
                print("BURST DMG: " + dmg + ", Distance: " + dist);
                break;
            }
        }
    }

    void DoAirburst()
    {
        float dmg = Mathf.Max(damage - distFromLockedOn, 0);
        bc.lockedOn.GetComponent<EnemySantaUtils>().GetHit(dmg);
        KillBullet();
    }
}
