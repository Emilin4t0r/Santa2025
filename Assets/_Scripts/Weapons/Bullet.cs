using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class Bullet : MonoBehaviour
{
    Vector3 lastPos, scndLastPos, thrdLastPos;
    public string enemyTag;
    GameObject trail;
    public GameObject bulletHit, altBulletHit;
    public float damage = 1;
    public bool doRandomRicochets;
    private void Awake()
    {
        trail = transform.Find("Trail").gameObject;
        trail.SetActive(false);
        Invoke("ActivateVisuals", 0.02f);
    }

    private void Update()
    {
        thrdLastPos = scndLastPos;
        scndLastPos = lastPos;
        lastPos = transform.position;
    }

    private void OnCollisionEnter(Collision collision)
    {
        try
        {
            if (collision.gameObject.CompareTag(enemyTag))
            {
                if (enemyTag == "Enemy")
                {
                    collision.gameObject.GetComponent<EnemySantaUtils>().GetHit(damage);
                    KillBullet(true);
                    return;
                }               
            }
            if (doRandomRicochets)
            {
                // 1/10 chance bullet doesn't get destroyed on hit
                int random = Random.Range(0, 10); 
                if (random != 0)
                {
                    KillBullet(false);
                }
            } else
            {
                KillBullet(false);
            }
        }
        catch
        {
            return;
        }
    }

    public void KillBullet(bool useAltHit)
    {
        GameObject hit = null;
        if (useAltHit && altBulletHit != null)
        {
            hit = Instantiate(altBulletHit, thrdLastPos, transform.rotation);
        }
        else
        {
            hit = Instantiate(bulletHit, thrdLastPos, transform.rotation);
        }
        Destroy(hit, 2f);
        Destroy(gameObject);
    }

    void ActivateVisuals()
    {
        trail.SetActive(true);
    }
}
