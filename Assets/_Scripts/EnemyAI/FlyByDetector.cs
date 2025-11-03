using System.Collections;
using UnityEngine;

public class FlybyDetector : MonoBehaviour
{
    public Transform player;
    public float flybySpeedThreshold = 100f;
    public float soundTriggeringOffset = 0.2f;
    public float flybyTriggerDistance;

    private Vector3 lastPlayerRelativePos;
    private bool wasInFrontLastFrame;
    EnemySantaMove sMove;
    bool soundSpawned;    

    private void Awake()
    {
        player = GameObject.Find("Aircraft").transform;
    }

    void Start()
    {
        lastPlayerRelativePos = transform.position - player.position;
        wasInFrontLastFrame = Vector3.Dot(player.forward, lastPlayerRelativePos) > 0;
        sMove = GetComponent<EnemySantaMove>();
    }

    void Update()
    {
        if (Vector3.Distance(transform.position, player.position) > flybyTriggerDistance)
            return;
        Vector3 relativePos = transform.position - player.position;
        float dot = Vector3.Dot(player.forward, relativePos);
        bool isInFrontNow = dot > 0;

        // Check for passing from front to back
        if (wasInFrontLastFrame && dot < soundTriggeringOffset && !soundSpawned)
        {
            // Calculate relative velocity
            Rigidbody playerRb = player.GetComponent<Rigidbody>();
            Vector3 relativeVelocity = sMove.currentVelocity - (playerRb ? playerRb.linearVelocity : Vector3.zero);
            float relativeSpeed = relativeVelocity.magnitude;

            if (relativeSpeed >= flybySpeedThreshold)
            {
                SoundSpawner.SpawnSound(transform.position, transform, SoundLibrary.GetClip("jet_flyby_short"), 1, 0.35f, 0.6f);
                soundSpawned = true;
                StartCoroutine(SoundReloader());
            }
        }

        wasInFrontLastFrame = isInFrontNow;
    }

    IEnumerator SoundReloader()
    {        
        yield return new WaitForSeconds(5);
        soundSpawned = false;
    }
}