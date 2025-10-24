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
    public float forwardProbeDistance = 200f;         // how far in front of the jet we probe
    public float groundClearanceThreshold = 50;    // variable1 — if clearance < this, we escape
    public float escapeDuration = 2.0f;               // variable2 — how long we fly the escape vector
    public float escapeTurnRate = 60f;                // how fast we rotate to face escape vector (deg/s)
    bool isEscaping = false;
    float escapeTimer = 0f;
    Vector3 escapeDirection = Vector3.up;            // world-space direction to head toward while escaping

    public Transform target;
    EnemySantaUtils utils;

    private void Start()
    {
        utils = GetComponent<EnemySantaUtils>();
        GetTarget();
        previousPosition = transform.position;        
    }

    private void Update()
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
            SoundSpawner.SpawnSound(target.position, target, SoundLibrary.GetClip("rwr_lock"), 0, false);
            EnemiesController.enemiesAttacking.Add(gameObject);
        }
        utils.trackCollider.target = target;
    }

    void Move()
    {
        // --- Rotation (turning) ---
        Vector3 toTarget = (target.position - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(toTarget);

        // --- Forward movement speed (unchanged) ---
        float distanceToDestination = Vector3.Distance(transform.position, target.position);
        float desiredMoveSpeed = Mathf.Clamp(maxSpeed * (distanceToDestination / 100), maxSpeed / 2, maxSpeed);
        currentMoveSpeed = Mathf.MoveTowards(currentMoveSpeed, desiredMoveSpeed, moveAcceleration * Time.deltaTime);

        // ---------- Raycast check to trigger escape ----------
        if (!isEscaping)
        {
            // Probe a point in front of the plane
            Vector3 probePoint = transform.position + transform.forward * forwardProbeDistance;
            LayerMask groundMask = LayerMask.GetMask("Ground");

            // Raycast down from reasonably high above the probePoint to find terrain Y
            // (use a large height to be robust; tweak if you have a very large world)
            if (Physics.Raycast(probePoint + Vector3.up * 20000f, Vector3.down, out RaycastHit hitAhead, 40000f, groundMask))
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
            transform.rotation = Quaternion.RotateTowards(transform.rotation, escapeRot, escapeTurnRate * Time.deltaTime);

            // Move forward exactly as before (no speed changes)
            Vector3 move = transform.forward * currentMoveSpeed * Time.deltaTime;
            transform.position += move;

            // Countdown escape timer and stop escaping when time's up
            escapeTimer -= Time.deltaTime;
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
                maxTurnRate * Time.deltaTime
            );

            // --- Forward movement ---
            Vector3 moveDirection = transform.forward * currentMoveSpeed * Time.deltaTime;
            transform.position += moveDirection;
        }

        // Debug line to destination
        Debug.DrawLine(transform.position, target.position, Color.blue);
    }

    void UpdateVelocity()
    {
        // guard against first frame huge delta
        float dt = Time.deltaTime;
        if (dt <= 0) return;
        currentVelocity = (transform.position - previousPosition) / dt;
        previousPosition = transform.position;
    }
}
