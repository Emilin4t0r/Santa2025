using UnityEngine;

public class SwarmMissile : MonoBehaviour
{
    [HideInInspector] public float jetSpdOnLaunch;
    public float speed;
    public float turnSpeed;
    public float visualRotationSpeed;
    public Vector2 damageRange;
    public GameObject explosion;
    GameObject rotator;
    public Transform target;
    TrailRenderer trail;
    CapsuleCollider cc;
    Rigidbody rb;
    GameObject pointLight;

    public GameObject radar;

    public float lifeTime = 5;
    float blowUpTimer;

    public float launchAccuracy;
    public GameObject finsToHide;

    private void Start()
    {
        SoundSpawner.SpawnSound(transform.position, transform, SoundLibrary.GetClip("missile_launch"));
        rotator = transform.Find("MissileRotator").gameObject;
        trail = transform.GetComponentInChildren<TrailRenderer>();
        cc = GetComponent<CapsuleCollider>();
        rb = GetComponent<Rigidbody>();
        pointLight = transform.Find("Point Light").gameObject;
        trail.enabled = true;
        cc.enabled = true;
        rb.isKinematic = false;
        pointLight.SetActive(true);
        if (finsToHide != null)
        {
            finsToHide.SetActive(true);
        }
        jetSpdOnLaunch = AirplaneController.instance.rb.linearVelocity.magnitude;
        DoInitialLaunchOffset();
    }

    private void FixedUpdate()
    {
        //TODO: Try to make the missile fly on an intercept course instead of following the target
        if (target)
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
        }

        // Move forward
        rb.linearVelocity = transform.forward * (speed + jetSpdOnLaunch);

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
            float rand = Random.Range(damageRange.x, damageRange.y);
            other.GetComponent<EnemySanta>().GetHit(rand);
        }
        if (other.CompareTag("Ground"))
            BlowUp();
    }

    void DoInitialLaunchOffset()
    {
        Vector2 deviation2D = Random.insideUnitCircle * launchAccuracy;
        Quaternion deviationRot = Quaternion.Euler(deviation2D.y, deviation2D.x, 0f);
        Vector3 newDirection = deviationRot * transform.forward;
        transform.rotation = Quaternion.LookRotation(newDirection, Vector3.up);
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
