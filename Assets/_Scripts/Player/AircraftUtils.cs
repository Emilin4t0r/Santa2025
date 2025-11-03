using System.Collections;
using UnityEngine;

public class AircraftUtils : MonoBehaviour
{
    AirplaneController ac;
    AircraftPhysics ap;

    public float terrainWarningDist, terrainWarningDistDown, turnOnDelay;
    bool warningAboutTerrain;
    float nextTerrainWarningTime;

    AudioSource engineSfxLoop;

    private void Awake()
    {
        ac = GetComponent<AirplaneController>();
        ap = GetComponent<AircraftPhysics>();
    }

    private void Start()
    {
        ac.enabled = false;
        ac.enabled = false;
        engineSfxLoop = GetComponent<AudioSource>();
        engineSfxLoop.Stop();
        SoundSpawner.SpawnSound(transform.position, transform, SoundLibrary.GetClip("jet_player_ignition"), 0, 0, 0.3f);
        StartCoroutine(TurnOnAfter(turnOnDelay));
    }

    private void FixedUpdate()
    {
        TerrainWarning();
    }

    IEnumerator TurnOnAfter(float delay)
    {
        yield return new WaitForSeconds(delay);
        ac.enabled = true;
        ac.enabled = true;
        engineSfxLoop.Play();
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
}
