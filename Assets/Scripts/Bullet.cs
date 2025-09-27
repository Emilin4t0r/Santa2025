using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    Vector3 lastPos, scndLastPos, thrdLastPos;
    public string enemyTag;
    GameObject sphere, trail;
    public GameObject bulletHit;

    private void Awake()
    {
        sphere = transform.Find("Sphere").gameObject;
        trail = transform.Find("Trail").gameObject;
        sphere.SetActive(false);
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
                    collision.gameObject.GetComponent<EnemySanta>().GetHit();
                }               
            }
            int random = Random.Range(0, 10);
            if (random != 0)
            {
                bulletHit = Instantiate(bulletHit, thrdLastPos, transform.rotation);
                Destroy(bulletHit, 0.5f);
                Destroy(gameObject);
            }
        }
        catch
        {
            return;
        }
    }

    void ActivateVisuals()
    {
        sphere.SetActive(true);
        trail.SetActive(true);
    }
}
