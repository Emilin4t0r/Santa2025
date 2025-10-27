using UnityEngine;

public class AircraftUtils : MonoBehaviour
{
    AirplaneController ac;

    public float terrainWarningDist, terrainWarningDistDown;
    bool warningAboutTerrain;
    float nextTerrainWarningTime;

    private void Start()
    {
        ac = GetComponent<AirplaneController>();
    }

    private void FixedUpdate()
    {
        TerrainWarning();
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
                    SoundSpawner.SpawnSound(transform.position, transform, SoundLibrary.GetClip("terrain"), 0, false);
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
