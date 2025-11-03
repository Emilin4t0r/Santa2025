using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class EnemySantaMove : MonoBehaviour
{
    [Header("Movement")]
    public float maxSpeed = 200f; // Speed of movement
    public float moveAcceleration = 10f;
    public float currentMoveSpeed;
    public float maxTurnRate = 30f; // degrees per second    
    public float lateralDampen = 1f;
    public float minDistFromGround = 50;
    Vector3 previousPosition;
    public Vector3 currentVelocity; // Current velocity vector

    [Header("Ground Avoidance")]
    public float forwardProbeDistance = 200f; // how far in front of the jet to probe
    public float groundClearanceThreshold = 50; // if clearance < this, start escape
    public float escapeDuration = 2.0f; // how long we fly the escape vector
    public float escapeTurnRate = 60f; // how fast we rotate to face escape vector (deg/s)
    bool isEscaping = false;
    float escapeTimer = 0f;
    Vector3 escapeDirection = Vector3.up; // world-space direction to head toward while escaping

    public Transform target;
    EnemySantaUtils utils;

    private void Start()
    {
        utils = GetComponent<EnemySantaUtils>();
        GetTarget();
        previousPosition = transform.position;
    }

    private void FixedUpdate()
    {
        UpdateVelocity();

        if (target != null)
            Move();
        else
            GetTarget();
    }

    void GetTarget()
    {
        target = AllTargetsManager.instance.GetRandomTarget(transform);
        if (target == null)
            return;
        if (target.CompareTag("Player"))
        {
            EnemiesController.enemiesAttacking.Add(gameObject);
        }
    }

    void Move()
    {
        // --- Rotation (turning) ---
        Vector3 toTarget = (target.position - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(toTarget);

        // --- Forward movement speed ---
        float distanceToDestination = Vector3.Distance(transform.position, target.position);
        float desiredMoveSpeed = Mathf.Clamp(maxSpeed * (distanceToDestination / 100), maxSpeed / 2, maxSpeed);
        currentMoveSpeed = Mathf.MoveTowards(currentMoveSpeed, desiredMoveSpeed, moveAcceleration * Time.fixedDeltaTime);

        // ---------- Raycast check to trigger escape ----------
        if (!isEscaping)
        {
            LayerMask arenaMask = LayerMask.GetMask("Arena");

            Vector3 probePoint = transform.position + transform.forward * forwardProbeDistance;
            LayerMask groundMask = LayerMask.GetMask("Ground");

            if (Physics.Raycast(transform.position, transform.position + transform.forward, out RaycastHit hitArena, forwardProbeDistance, arenaMask)) // Avoid Arena walls & ceiling
            {
                // Start escaping for escapeDuration seconds
                isEscaping = true;
                escapeTimer = escapeDuration;
                // Escape direction: fly away from the arena hit point and bias backward
                Vector3 away = (transform.position - hitArena.point).normalized;
                escapeDirection = (away + -Vector3.forward * 0.6f).normalized;
            }
            else if (Physics.Raycast(probePoint + Vector3.up * 5f, Vector3.down, out RaycastHit hitAhead, 40000f, groundMask)) // Avoid the ground
            {
                float clearance = probePoint.y - hitAhead.point.y;
                if (clearance < groundClearanceThreshold)
                {
                    // Start escaping for escapeDuration seconds
                    isEscaping = true;
                    escapeTimer = escapeDuration;
                    // Escape direction: fly away from the ground hit point and bias upward
                    Vector3 away = (transform.position - hitAhead.point).normalized;
                    escapeDirection = (away + Vector3.up * 0.6f).normalized;
                    // ensure some forward component so we don't point straight down/up awkwardly
                    if (Vector3.Dot(escapeDirection, transform.forward) < 0.05f)
                        escapeDirection = (transform.forward + Vector3.up * 0.5f).normalized;
                }
            }
        }

        // ---------- If escaping: ignore target and fly toward escapeDirection for escapeDuration ----------
        if (isEscaping)
        {
            // Turn toward escapeDirection (uses escapeTurnRate)
            Quaternion escapeRot = Quaternion.LookRotation(escapeDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, escapeRot, escapeTurnRate * Time.fixedDeltaTime);

            // Countdown escape timer and stop escaping when time's up
            escapeTimer -= Time.fixedDeltaTime;
            if (escapeTimer <= 0f)
            {
                isEscaping = false;
            }

            // debug: draw escape dir
            Debug.DrawLine(transform.position, transform.position + escapeDirection * 200f, Color.red);
        }
        else
        {
            // ---------- Normal chase behaviour ----------
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                maxTurnRate * Time.fixedDeltaTime
            );
        }

        // Move forward
        Vector3 move = transform.forward * currentMoveSpeed * Time.fixedDeltaTime;
        transform.position += move;

        // Debug line to destination
        Debug.DrawLine(transform.position, target.position, Color.blue);
    }

    void UpdateVelocity()
    {
        // guard against first frame huge delta
        float dt = Time.fixedDeltaTime;
        if (dt <= 0) return;
        currentVelocity = (transform.position - previousPosition) / dt;
        previousPosition = transform.position;
    }
}
