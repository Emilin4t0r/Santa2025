using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using static UnityEditor.PlayerSettings;

public class Bullet : MonoBehaviour
{
    Vector3 lastPos, scndLastPos, thrdLastPos;
    public string[] enemyTags;
    GameObject trail;
    public GameObject bulletHit, altBulletHit;
    public float damage = 1;
    public bool doRandomRicochets;
    public string hitSoundName;
    private void Awake()
    {
        trail = transform.Find("Trail").gameObject;
        if (trail.activeSelf)
        {
            trail.SetActive(false);
            Invoke("ActivateVisuals", 0.02f);
        }
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
            foreach (var tag in enemyTags)
            {
                if (collision.gameObject.CompareTag(tag))
                {
                    // collision.gameObject is valid enemy for bullet.

                    if (tag == "Enemy")
                    {
                        collision.gameObject.GetComponent<EnemySantaUtils>().GetHit(damage);
                        KillBullet(useAltHit: true);
                        return;
                    }
                    if (tag == "Player")
                    {
                        float randDamage = damage + Random.Range(-damage / 2, damage / 2);
                        collision.gameObject.GetComponent<AircraftUtils>().TakeDamage(randDamage);
                        KillBullet(useAltHit: true);
                        return;
                    }
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
            if (hitSoundName == "20mm_hit")
                SoundPool.Instance.PlayOneShot(SoundLibrary.GetClip(hitSoundName), hit.transform.position, 0.75f, 1, 0.5f, spatial: true);
        }
        else
        {
            hit = Instantiate(bulletHit, thrdLastPos, transform.rotation);
            if (hitSoundName != "20mm_hit")
                SoundPool.Instance.PlayOneShot(SoundLibrary.GetClip(hitSoundName), hit.transform.position, 0.75f, 1, 0.5f, spatial: true);
        }
        hit.transform.eulerAngles = -transform.forward;
        float randScale = Random.Range(0.75f, 1.25f);
        hit.transform.localScale = new Vector3(randScale, randScale, randScale);        
        Destroy(hit, 2f);
        Destroy(gameObject);
    }

    void ActivateVisuals()
    {
        trail.SetActive(true);
    }
}
