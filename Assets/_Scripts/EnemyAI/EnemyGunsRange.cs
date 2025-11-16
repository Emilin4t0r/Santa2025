using System.Collections.Generic;
using UnityEngine;

public class EnemyGunsRange : MonoBehaviour
{
    [Header("Detection")]
    [Tooltip("Max distance to detect targets")]
    public float radius = 50f;
    [Tooltip("Full cone angle in degrees (e.g. 60 => 30 degrees to each side)")]
    [Range(1f, 180f)]
    public float coneAngle = 60f;
    [Tooltip("How often (seconds) to run the detection check. Set to 0 to check every frame.")]
    public float detectionInterval = 0.1f;
    [Tooltip("Optional layer mask to filter targets by layer")]
    public LayerMask layerMask = ~0;

    [Header("References")]
    public bool readyToFire;
    public EnemySantaMove moveScript;
    public EnemySantaUtils utils;

    // internal state
    bool playerWasInRange = false;
    bool targetWasInRange = false;
    float nextCheckTime = 0f;

    void FixedUpdate()
    {
        // run every frame if detectionInterval <= 0, otherwise at intervals
        if (detectionInterval > 0f)
        {
            if (Time.time < nextCheckTime) return;
            nextCheckTime = Time.time + detectionInterval;
        }

        RunDetection();
    }

    void RunDetection()
    {
        if (AllTargetsManager.instance == null || AllTargetsManager.instance.targets == null) return;

        float halfAngle = coneAngle * 0.5f;
        float cosHalfAngle = Mathf.Cos(halfAngle * Mathf.Deg2Rad);
        Vector3 forward = transform.forward;
        Vector3 selfPos = transform.position;

        bool foundPlayerThisFrame = false;
        bool foundCurrentTargetThisFrame = false;

        List<Transform> targets = AllTargetsManager.instance.targets;
        for (int i = 0; i < targets.Count; i++)
        {
            Transform t = targets[i];
            if (t == null) continue;

            // optional layer filter
            if ((layerMask.value & (1 << t.gameObject.layer)) == 0) continue;

            Vector3 toTarget = t.position - selfPos;
            float sqrDist = toTarget.sqrMagnitude;
            if (sqrDist > radius * radius) continue; // out of radius

            Vector3 dir = toTarget.normalized;
            float dot = Vector3.Dot(forward, dir);
            if (dot < cosHalfAngle) continue; // outside cone

            // target is inside cone & radius
            // detect player by tag
            if (t.gameObject.CompareTag("Player"))
            {
                foundPlayerThisFrame = true;
            }
            else if (t.gameObject.CompareTag("Enemy"))
            {
                // Found an enemy...
            }
            else
            {
                continue;
            }

            // Check if this target matches our assigned target
            if (moveScript != null && moveScript.target != null)
            {
                if (t == moveScript.target)
                {
                    foundCurrentTargetThisFrame = true;
                }
            }
        }

        // Player HUD enter/exit
        if (foundPlayerThisFrame && !playerWasInRange)
        {
            if (TargetInfo.instance != null && moveScript != null)
                TargetInfo.instance.ChangeEnemiesInGunrange(moveScript.transform, false);
        }
        else if (!foundPlayerThisFrame && playerWasInRange)
        {
            if (TargetInfo.instance != null && moveScript != null)
                TargetInfo.instance.ChangeEnemiesInGunrange(moveScript.transform, true);
        }

        playerWasInRange = foundPlayerThisFrame;

        // Firing target enter/exit
        if (foundCurrentTargetThisFrame && !targetWasInRange)
        {
            if (utils != null)
                utils.nextShootTime = Time.time + Random.Range(0.2f, 2f);
            readyToFire = true;
        }
        else if (!foundCurrentTargetThisFrame && targetWasInRange)
        {
            readyToFire = false;
        }
        targetWasInRange = foundCurrentTargetThisFrame;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.2f, 0.2f, 0.25f);
        Gizmos.DrawWireSphere(transform.position, radius);

        Vector3 forward = transform.forward;
        Quaternion leftRot = Quaternion.AngleAxis(-coneAngle * 0.5f, transform.up);
        Quaternion rightRot = Quaternion.AngleAxis(coneAngle * 0.5f, transform.up);

        Vector3 leftDir = leftRot * forward;
        Vector3 rightDir = rightRot * forward;

        Gizmos.DrawLine(transform.position, transform.position + leftDir.normalized * radius);
        Gizmos.DrawLine(transform.position, transform.position + rightDir.normalized * radius);
    }
}
