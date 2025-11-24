using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AircraftUtils : MonoBehaviour
{
    public static AircraftUtils instance;

    AirplaneController ac;
    AircraftPhysics ap;

    public float terrainWarningDist, terrainWarningDistDown, turnOnDelay;
    [HideInInspector] public bool turnedOn;
    bool warningAboutTerrain;
    float nextTerrainWarningTime;

    AudioSource engineSfxLoop;

    public float health = 100;
    public GameObject plrDeathPrefab;

    private void Awake()
    {
        instance = this;
        ac = GetComponent<AirplaneController>();
        ap = GetComponent<AircraftPhysics>();
        turnedOn = false;
    }

    private void Start()
    {
        ac.enabled = false;
        ap.enabled = false;
        engineSfxLoop = GetComponent<AudioSource>();
        engineSfxLoop.Stop();
        SoundSpawner.SpawnSound(transform.position, transform, SoundLibrary.GetClip("jet_player_ignition"), 0, 0, 0.3f);
        StartCoroutine(TurnOnAfter(turnOnDelay));
    }

    private void FixedUpdate()
    {
        TerrainWarning();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            Die();
        }
    }

    IEnumerator TurnOnAfter(float delay)
    {
        yield return new WaitForSeconds(delay);
        ac.enabled = true;
        ap.enabled = true;
        engineSfxLoop.Play();
        ac.thrustPercent = 1;
        turnedOn = true;
        StartCoroutine(StartBoost());
    }

    IEnumerator StartBoost()
    {
        var t = Time.time + 2;
        EZCameraShake.CameraShaker.Instance.ShakeOnce(5, 15, 0, 2.5f);
        while (Time.time < t)
        {
            ac.GetComponent<Rigidbody>().AddForce(-Vector3.forward * 50000, ForceMode.Force);
            yield return null;
        }
    }

    void TerrainWarning()
    {
        if (AircraftCollision.isGrounded)
            return;

        LayerMask groundMask = LayerMask.GetMask("Ground");
        Vector3 velocity = ac.rb.linearVelocity;
        if (velocity.sqrMagnitude > 0.0001f)
        {
            Vector3 dir = velocity.normalized;
            bool hit = Physics.Raycast(transform.position, dir, terrainWarningDist, groundMask);
            bool hitDown = Physics.Raycast(transform.position, Vector3.down, terrainWarningDistDown, groundMask); 
            if (hit && hitDown)
            {                
                if (!warningAboutTerrain)
                {
                    SoundSpawner.SpawnSound(transform.position, transform, SoundLibrary.GetClip("terrain"), 0, 0);
                    warningAboutTerrain = true;
                    nextTerrainWarningTime = Time.time + 2;
                }
            }

            if (warningAboutTerrain)
            {
                if (Time.time > nextTerrainWarningTime)
                {
                    warningAboutTerrain = false;
                }
            }

            Debug.DrawLine(transform.position, transform.position + dir * terrainWarningDist, Color.green);
            Debug.DrawLine(transform.position, transform.position + Vector3.down * terrainWarningDistDown, Color.red);
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        print("PLAYER IS HIT -" + damage + "HP! Health now: " + health);
        EZCameraShake.CameraShaker.Instance.ShakeOnce(2.5f, 10, 0, 0.3f);
        if (health < 0)
        {
            Die();
        }
    }
    void Die()
    {
        Destroy(gameObject);
        var death = Instantiate(plrDeathPrefab, transform.position, Quaternion.identity, null);
    }
}
