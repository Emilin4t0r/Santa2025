using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class Bullet : MonoBehaviour
{
    Vector3 lastPos, scndLastPos, thrdLastPos;
    public string enemyTag;
    GameObject trail, pointLight;
    float lightKillTimer;
    public GameObject bulletHit;
    public float damage = 1;

    private void Awake()
    {
        trail = transform.Find("Trail").gameObject;
        pointLight = transform.Find("Point Light").gameObject;
        trail.SetActive(false);
        Invoke("ActivateVisuals", 0.02f);
    }

    private void Update()
    {
        thrdLastPos = scndLastPos;
        scndLastPos = lastPos;
        lastPos = transform.position;
        lightKillTimer += Time.deltaTime;
        if (lightKillTimer > 0.05f)
        {
            pointLight.SetActive(false);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        try
        {
            if (collision.gameObject.CompareTag(enemyTag))
            {
                if (enemyTag == "Enemy")
                {
                    collision.gameObject.GetComponent<EnemySanta>().GetHit(damage);
                }               
            }
            print("Bullet hit " + collision.gameObject.name);
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
        trail.SetActive(true);
    }
}
