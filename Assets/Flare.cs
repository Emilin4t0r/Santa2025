using System.Collections.Generic;
using UnityEngine;

public class Flare : MonoBehaviour
{
    public GameObject sphere;
    public Rigidbody rb;

    public void TurnOn(float launchForce)
    {
        sphere.SetActive(true);
        rb.isKinematic = false;
        rb.AddForce(Random.insideUnitSphere * launchForce, ForceMode.Impulse);

        List<EnemySantaMove> attackingEnemies = new List<EnemySantaMove>();

        foreach(var e in EnemiesController.enemiesAttacking)
        {
            attackingEnemies.Add(e.GetComponent<EnemySantaMove>());          
        }

        foreach(var e in attackingEnemies)
        {
            var eScript = e.GetComponent<EnemySantaMove>();
            var distanceToDestination = Vector3.Distance(transform.position, eScript.target.position);
            eScript.StartDisengage(distanceToDestination);
        }
    }
}
