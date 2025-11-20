using DG.Tweening;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionsTimer : MonoBehaviour
{
    public List<GameObject> explosions;

    public float timeBetweenExplosions;
    float nextExplosionTime;
    int iNextExplosion;
    bool stopExplosions;

    private void Update()
    {
        if (stopExplosions)
            return;
        if (Time.time > nextExplosionTime)
        {
            explosions[iNextExplosion].SetActive(true);
            Helpers.ExplosionSound(transform.position);
            nextExplosionTime = Time.time + timeBetweenExplosions;
            ++iNextExplosion;

            if (iNextExplosion == explosions.Count)
            {
                stopExplosions = true;
            }
        }
    }
}
