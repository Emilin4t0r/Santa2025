using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    public float speed;
    public float turnSpeed;
    public float visualRotationSpeed;
    public GameObject explosion;
    GameObject rotator;
    public Transform target;
    TrailRenderer trail;
    CapsuleCollider cc;
    Rigidbody rb;

    public float lifeTime = 5;
    float blowUpTimer;

    private void Start()
    {
        SoundSpawner.SpawnSound(transform.position, transform, SoundLibrary.GetClip("missile_launch"));
        rotator = transform.Find("MissileRotator").gameObject;
        trail = transform.GetComponentInChildren<TrailRenderer>();
        cc = GetComponent<CapsuleCollider>();
        rb = GetComponent<Rigidbody>();
        trail.enabled = true;
        cc.enabled = true;
        rb.isKinematic = false;
    }
    
    private void FixedUpdate()
    {
        float dist = Vector3.Distance(transform.position, target.position) / 10;
        Vector3 targetPos = target.position + target.forward * dist;

        // Rotate front towards target
        Vector3 targetDir = (targetPos - transform.position).normalized;
        Quaternion targetRot = Quaternion.LookRotation(targetDir);
        // Rotate at 'turnSpeed' degrees per second
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRot,
            turnSpeed * Time.deltaTime
        );

        // Move forward
        transform.Translate(Vector3.forward * speed * Time.fixedDeltaTime);

        // Rotate missile visually
        rotator.transform.Rotate(new Vector3(0, 0, -visualRotationSpeed * Time.fixedDeltaTime));

        blowUpTimer += Time.fixedDeltaTime;
        if (blowUpTimer > lifeTime)
            BlowUp();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            BlowUp();
            other.GetComponent<EnemySanta>().Die();            
        }
        if (other.CompareTag("Ground"))
            BlowUp();
    }

    void BlowUp(float explSizeMultiplier = 0.5f)
    {
        GameObject expl = Instantiate(explosion, transform.position, Quaternion.identity);
        expl.transform.localScale *= explSizeMultiplier;
        SoundSpawner.SpawnSound(transform.position, AirplaneController.instance.transform, SoundLibrary.GetClip("missile_explode"));
        Destroy(expl, 1);
        Destroy(gameObject);
    }    
}

