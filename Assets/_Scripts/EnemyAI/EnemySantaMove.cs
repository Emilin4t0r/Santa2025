using UnityEngine;
using UnityEngine.SocialPlatforms;
using static UnityEngine.GraphicsBuffer;

public class EnemySantaMove : MonoBehaviour
{
    public enum AIState { Chase, Disengage, Reengage }

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
    bool isEscaping = false;
    float escapeTimer = 0f;
    Vector3 escapeDirection = Vector3.up; // world-space direction to head toward while escaping

    [Header("Disengage / Re-engage")]
    public float disengageChancePerSecond = 0.12f; // chance per second to start a disengage while in range
    public float minDistBeforeDisengage = 800f; // distance from target to consider disengage
    public float disengageDuration = 2.5f; // how long it flies away when disengaging
    public float reengageDistanceMultiplier = 1.6f; // how much farther away it wants to be before resuming chase
    public float reengageExtraDistance = 100f; // added to computed reengage distance
    public float minTimeBetweenDisengages = 6f; // cooldown to avoid too frequent disengages

    [HideInInspector] public AIState state = AIState.Chase;
    float disengageTimer = 0f;
    float lastDisengageTime = -999f;
    Vector3 disengageDirection = Vector3.back;
    float desiredReengageDistance = 0f;

    // stored info about the target at the moment of disengage
    Vector3 lastTargetPositionAtDisengage = Vector3.zero;
    float storedDistanceAtDisengage = 0f;

    public Transform target;
    EnemySantaUtils utils;
    [HideInInspector] public float turnAmt;
    float lastYRot;

    private void Start()
    {
        utils = GetComponent<EnemySantaUtils>();
        GetTarget();
        previousPosition = transform.position;
    }

    private void FixedUpdate()
    {
        UpdateVelocity();

        // always run Move so the jet keeps flying during Disengage/Reengage/escape
        Move();
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
        // compute distanceToDestination only if we have a live target
        float distanceToDestination = float.PositiveInfinity;
        Vector3 toTarget = Vector3.forward;
        Quaternion targetRotation = transform.rotation;
        if (target != null)
        {
            toTarget = (target.position - transform.position).normalized;
            targetRotation = Quaternion.LookRotation(toTarget);
            distanceToDestination = Vector3.Distance(transform.position, target.position);
        }

        // --- Forward movement speed (based on distance to target if present) ---
        float desiredMoveSpeed = maxSpeed;
        if (!float.IsInfinity(distanceToDestination))
            desiredMoveSpeed = Mathf.Clamp(maxSpeed * (distanceToDestination / 100f), maxSpeed / 2f, maxSpeed);
        currentMoveSpeed = Mathf.MoveTowards(currentMoveSpeed, desiredMoveSpeed, moveAcceleration * Time.fixedDeltaTime);

        // ---------- Raycast check to trigger escape ----------

        LayerMask arenaMask = LayerMask.GetMask("Arena");
        LayerMask groundMask = LayerMask.GetMask("Ground");

        Vector3 probePoint = transform.position + transform.forward * forwardProbeDistance;

        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hitArena, forwardProbeDistance, arenaMask)) // Avoid Arena walls & ceiling
        {
            // Start escaping for escapeDuration seconds times 2 (to get far enough from the edges and not risk spazzing out into the walls)
            isEscaping = true;
            escapeTimer = escapeDuration * 2;
            // Escape direction: towards center of arena (there's a specific empty object there)
            escapeDirection = GameObject.Find("ArenaEscapeForEnemies").transform.position - transform.position;            
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


        // ---------- If escaping: ignore other AI and fly toward escapeDirection ----------
        if (isEscaping)
        {
            Quaternion escapeRot = Quaternion.LookRotation(escapeDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, escapeRot, maxTurnRate * Time.fixedDeltaTime);

            escapeTimer -= Time.fixedDeltaTime;
            if (escapeTimer <= 0f)
            {
                isEscaping = false;
            }

            // Make sure we won't disengage out of arena after escape
            if (state != AIState.Chase)
            {
                SetState(AIState.Chase);
                lastDisengageTime = Time.time;
                // pick a new target now that we're re-engaging
                GetTarget();
            }

            Debug.DrawLine(transform.position, transform.position + escapeDirection * 200f, Color.white);
        }
        else
        {
            // ---------- AI state logic: Chase / Disengage / Reengage ----------
            switch (state)
            {
                case AIState.Chase:
                    // If we don't have a target, try to acquire one
                    if (target == null)
                    {
                        GetTarget();
                    }

                    // now (if we have a target) possibly start a disengage
                    if (target != null)
                    {
                        distanceToDestination = Vector3.Distance(transform.position, target.position);

                        if (Time.time - lastDisengageTime >= minTimeBetweenDisengages
                            && distanceToDestination <= minDistBeforeDisengage)
                        {
                            if (Random.value < disengageChancePerSecond * Time.fixedDeltaTime)
                            {
                                StartDisengage(distanceToDestination);
                                break;
                            }
                        }

                        // normal chase rotation
                        transform.rotation = Quaternion.RotateTowards(
                            transform.rotation,
                            Quaternion.LookRotation((target.position - transform.position).normalized),
                            maxTurnRate * Time.fixedDeltaTime
                        );
                    }
                    else
                    {
                        // No target available — just keep current forward heading (or you could idle-turn)
                    }
                    break;

                case AIState.Disengage:
                    // Turn toward disengageDirection
                    Quaternion disengageRot = Quaternion.LookRotation(disengageDirection);
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, disengageRot, maxTurnRate * Time.fixedDeltaTime);

                    disengageTimer -= Time.fixedDeltaTime;
                    if (disengageTimer <= 0f)
                    {
                        // finished flying away — start reengage phase:
                        SetState(AIState.Reengage);


                        // compute how far away we want to be before re-entering chase based on storedDistanceAtDisengage
                        desiredReengageDistance = storedDistanceAtDisengage * reengageDistanceMultiplier + reengageExtraDistance;
                        desiredReengageDistance = Mathf.Max(desiredReengageDistance, minDistBeforeDisengage + 50f);
                    }

                    Debug.DrawLine(transform.position, transform.position + disengageDirection * 200f, Color.yellow);
                    break;

                case AIState.Reengage:
                    // Use the stored target position from when we disengaged to measure separation
                    float currentDistanceFromOldTarget = Vector3.Distance(transform.position, lastTargetPositionAtDisengage);

                    // If we're already further than desired distance, go back to chase and pick a new target
                    if (currentDistanceFromOldTarget >= desiredReengageDistance)
                    {
                        SetState(AIState.Chase);
                        lastDisengageTime = Time.time;
                        // pick a new target now that we're re-engaging
                        GetTarget();
                        break;
                    }

                    // Otherwise, back off from the old target position until we hit desired distance
                    Vector3 awayFromOldTarget = (transform.position - lastTargetPositionAtDisengage).normalized;
                    Quaternion backOffRot = Quaternion.LookRotation(awayFromOldTarget);
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, backOffRot, maxTurnRate * Time.fixedDeltaTime);

                    break;
            }
        }

        // Move forward
        Vector3 move = transform.forward * currentMoveSpeed * Time.fixedDeltaTime;
        transform.position += move;

        // If we have a target draw debug line to it, otherwise draw to the last stored position for debugging
        if (target != null)
            Debug.DrawLine(transform.position, target.position, Color.blue);
        else
            Debug.DrawLine(transform.position, lastTargetPositionAtDisengage, Color.cyan);

        // Calculate turn speed
        turnAmt = lastYRot - transform.localEulerAngles.y;
        lastYRot = transform.localEulerAngles.y;
    }

    void SetState(AIState _state)
    {
        state = _state;
    }

    public void StartDisengage(float currentDistance)
    {
        if (target != null)
        {
            // store info about the target at the moment of disengage
            lastTargetPositionAtDisengage = target.position;
            storedDistanceAtDisengage = currentDistance;

            // if this was a player target, remove from attacking list
            if (target.CompareTag("Player"))
            {
                EnemiesController.enemiesAttacking.Remove(gameObject);
            }

            // forget the target immediately
            target = null;
        }

        // pick a disengage direction: generally away from the stored target position with some random yaw
        Vector3 dirAway;
        if (lastTargetPositionAtDisengage != Vector3.zero)
            dirAway = (transform.position - lastTargetPositionAtDisengage).normalized;
        else
            dirAway = -transform.forward;

        float randomYaw = Random.Range(-40f, 40f);
        Quaternion yaw = Quaternion.AngleAxis(randomYaw, Vector3.up);
        disengageDirection = (yaw * dirAway).normalized;

        SetState(AIState.Disengage);
        disengageTimer = disengageDuration;
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
