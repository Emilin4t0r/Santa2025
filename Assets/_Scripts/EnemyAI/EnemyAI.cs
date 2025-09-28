using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float moveSpeed = 200f; // Speed of movement
    public float visionAngle = 300f; // Field of vision angle in degrees
    public float moveRange = 10f; // Maximum range within which to pick random points
    public float turnSpeed = 3f;
    public float moveAcceleration = 10f;
    public float turnAcceleration = 10f;
    public float lateralDampen = 1f;
    public float flightAltitude = 300f;
    public float firingDistance = 150f;    
    private Vector3 destination; // Destination point for movement
    public float distToTurnAwayFromDestination = 40f;
    float currentMoveSpeed, currentTurnSpeed;

    public List<Transform> gunMuzzles;
    public float shootForce;
    public float inaccuracy;
    public GameObject bulletPrefab, muzzleFlashPrefab;

    public Vector3 velocity;
    public int velocitiesToCountAmt = 5;
    Vector3 lastPos;
    Vector3[] velocities;
    int nextVelIndexToUpdate;

    AirplaneController ac;

    public GameObject deathParticle;

    bool attacking = false;
    bool repositioning = false;

    float timeToFireMissile = 0;
    public int missiles = 1;
    int missilesFired = 0;

    void Start()
    {        
        velocities = new Vector3[velocitiesToCountAmt];
        ac = AirplaneController.instance;
        StartCoroutine(Move());
        currentMoveSpeed = moveSpeed;
        currentTurnSpeed = turnSpeed;
    }

    private void FixedUpdate()
    {
        // Velocity calculation
        velocities[nextVelIndexToUpdate] = (transform.position - lastPos) / Time.fixedDeltaTime;
        if (nextVelIndexToUpdate < velocitiesToCountAmt - 1)
            nextVelIndexToUpdate++;
        else
            nextVelIndexToUpdate = 0;        
        if (velocities.Length == velocitiesToCountAmt)
        {
            //Calculate approximation
            Vector3 sum = Vector3.zero;
            foreach (var v in velocities)
            {
                sum += v;
            }
            Vector3 average = sum / velocitiesToCountAmt;
            velocity = average;
        } else
        {
            velocity = velocities[velocities.Length - 1];
        }
        lastPos = transform.position;

        // Seek for player
        if (!attacking && IsInCone(ac.transform.position, 55, firingDistance * 3))
            SeekTarget();
    }

    void SeekTarget()
    {
        int rand = Random.Range(0, 1);
        if (rand == 0)
        {
            // Start attack
            attacking = true;
            EnemiesController.enemiesAttacking.Add(gameObject);
            timeToFireMissile = Time.time + 0.5f;
            destination = ac.transform.position;
            //destination = Reposition();
            //repositioning = true;
        }
    }

    IEnumerator Move()
    {
        while (true)
        {
            if (attacking)
            {                
                // Move towards the destination
                while (Vector3.Distance(transform.position, destination) > distToTurnAwayFromDestination)
                {
                    if (!repositioning)
                        destination = ac.transform.position;
                    MoveCalculations();
                    print("Attacking, destination: " + destination);
                    print("AC " + ac.name);
                    // Shoot
                    if (IsInCone(destination, 55, firingDistance))
                    {
                        float dist = Vector3.Distance(transform.position, destination);
                        int rand = Random.Range(0, 60);
                        if (rand == 0 && dist < firingDistance)
                        {
                            int shots = Random.Range(0, 4);
                            StartCoroutine(ShootBurst(shots));
                        }

                        // Wait for missile firing
                        if (Time.time > timeToFireMissile && missilesFired < missiles)
                        {
                            int r = Random.Range(0, 2);
                            if (r == 0)
                            {                                
                                // Fire missile
                                SoundSpawner.SpawnSound(ac.transform.position, ac.transform, SoundLibrary.GetClip("rwr_missile"), 0, false);
                                TargetInfo.instance.TriggerMissileWarning();
                                missilesFired++;
                            }
                            timeToFireMissile = Time.time + 0.5f;
                        }
                    }

                    Debug.DrawLine(transform.position, destination, Color.blue);
                    yield return null;
                }
                if (repositioning)
                {
                    // resume attack after repositioning
                    destination = ac.transform.position;
                    repositioning = false;
                    if (!EnemiesController.enemiesAttacking.Contains(gameObject))
                        EnemiesController.enemiesAttacking.Add(gameObject);
                }
                else
                {
                    int r = Random.Range(0, 2);
                    if (r == 0)
                    {
                        // keep attacking
                        EnemiesController.enemiesAttacking.Remove(gameObject);
                        repositioning = true;
                        destination = Reposition();
                    } else
                    {
                        // stop the attack
                        EnemiesController.enemiesAttacking.Remove(gameObject);
                        repositioning = false;
                        attacking = false;
                        destination = CalculateRandomDestination();
                    }
                }
            }
            else
            {
                destination = CalculateRandomDestination();
                // Move towards the destination
                while (Vector3.Distance(transform.position, destination) > distToTurnAwayFromDestination)
                {
                    MoveCalculations();
                    yield return null;
                }
            }
        }
    }

    void MoveCalculations()
    {
        // Calculate direction and target rotation
        Vector3 direction = (destination - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        // Calculate actual turn speed and move towards it
        float distanceToDestination = Vector3.Distance(transform.position, destination);
        float desiredTurnSpeed = Mathf.Clamp(turnSpeed / (distanceToDestination / 100), 0.1f, 3);
        currentTurnSpeed = Mathf.MoveTowards(currentTurnSpeed, desiredTurnSpeed, turnAcceleration * Time.deltaTime);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, currentTurnSpeed * Time.deltaTime);

        // Calculate actual move speed and move towards it
        float desiredMoveSpeed = Mathf.Clamp(moveSpeed * (distanceToDestination / 100), moveSpeed / 2, moveSpeed);
        currentMoveSpeed = Mathf.MoveTowards(currentMoveSpeed, desiredMoveSpeed, moveAcceleration * Time.deltaTime);
        Vector3 moveDirection = transform.forward * currentMoveSpeed * Time.deltaTime;

        // Move the object
        transform.position = Vector3.Lerp(transform.position, transform.position + moveDirection, lateralDampen * Time.deltaTime);

        // Altitude adjustment
        LayerMask groundMask = LayerMask.GetMask("Ground");
        if (Physics.Raycast(transform.position + Vector3.up * 1000f, Vector3.down, out RaycastHit hit, 2000f, groundMask))
        {
            float targetY = hit.point.y + flightAltitude;
            Vector3 pos = transform.position;

            // Smooth vertical correction
            float altitudeChangeSpeed = 120f; // units per second, tweak this
            pos.y = Mathf.MoveTowards(pos.y, targetY, altitudeChangeSpeed * Time.deltaTime);

            transform.position = pos;
        }

        // Debug line to destination
        Debug.DrawLine(transform.position, destination, Color.blue);
    }

    IEnumerator ShootBurst(int shots)
    {
        int shotsFired = 0;
        while (shotsFired < shots)
        {
            Fire();
            shotsFired++;
            yield return new WaitForSeconds(0.1f);
        }
        yield return null;
    }

    void Fire()
    {
        foreach (var gm in gunMuzzles)
        {
            // Bullet spread calculations
            Vector3 deviation3D = Random.insideUnitCircle * inaccuracy;
            Quaternion rot = Quaternion.LookRotation(Vector3.forward + deviation3D);
            Vector3 fwd = gm.transform.rotation * rot * Vector3.forward;

            // Spawn bullet
            var bullet = Instantiate(bulletPrefab, gm.position, gm.transform.rotation, null);
            bullet.GetComponent<Rigidbody>().AddForce(fwd * shootForce, ForceMode.Impulse);
            Destroy(bullet, 5);

            // Spawn muzzle flash
            int doMzf = Random.Range(0, 2);
            if (doMzf == 0)
            {
                var mzf = Instantiate(muzzleFlashPrefab, gm.position, gm.transform.rotation, gm.transform);
                float rand = Random.Range(2f, 3.5f);
                mzf.transform.localScale = new Vector3(rand, rand, rand);
                Destroy(mzf, 0.02f);
            }
        }
    }

    Vector3 Reposition()
    {
        Vector3 newDestination = ac.transform.position + Random.insideUnitSphere * (moveRange / 2);

        float groundHeight = 0;
        LayerMask mask = LayerMask.GetMask("Ground");
        if (Physics.Raycast(newDestination + new Vector3(0, 5000, 0), Vector3.down, out RaycastHit hit, 5000, mask))
        {
            if (hit.collider.CompareTag("Ground"))
            {
                groundHeight = hit.point.y; // Return the hit point if it hits the ground
            }
        }
        newDestination.y = groundHeight + flightAltitude;

        return newDestination;
    }
    public bool IsInCone(Vector3 point, float coneApexAngle, float coneHeight)
    {
        Vector3 directionToObject = point - transform.position;
        float angleToObject = Vector3.Angle(transform.forward, directionToObject);
        float distanceToObject = directionToObject.magnitude;
        float maxDistance = Mathf.Tan(coneApexAngle * 0.5f * Mathf.Deg2Rad) * coneHeight;
        return angleToObject < coneApexAngle * 0.5f && distanceToObject < maxDistance;
    }

    Vector3 CalculateRandomDestination()
    {
        Vector3 newDestination = Random.insideUnitSphere * moveRange;        

        float groundHeight = 0;
        LayerMask mask = LayerMask.GetMask("Ground");
        if (Physics.Raycast(newDestination + new Vector3(0, 5000, 0), Vector3.down, out RaycastHit hit, 5000, mask))
        {
            Debug.DrawRay(newDestination + new Vector3(0, 5000, 0), Vector3.down * 5000, Color.green, 10);
            if (hit.collider.CompareTag("Ground"))
            {
                groundHeight = hit.point.y; // Return the hit point if it hits the ground
            }
        }
        newDestination.y = groundHeight + flightAltitude;

        return newDestination;
    }

    public void Die()
    {
        EnemiesController.enemiesAttacking.Remove(gameObject);
        SoundSpawner.SpawnSound(transform.position, transform.parent, SoundLibrary.GetClip("enemy_explode"));
        var partc = Instantiate(deathParticle, transform.position, transform.rotation);
        Destroy(partc, 1);
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        if (attacking)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position + new Vector3(0, 50, 0), 10);
        }
    }
}
