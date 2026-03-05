using System.Collections;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

enum FishState
{
    Wandering,
    Interested,
    Fleeing,
    Attacking,
    Hooked
}

public class FishAI : MonoBehaviour
{
    [Header("Fish Settings")]
    [SerializeField] public string fishName;
    [SerializeField] public FishType fishType;
    [SerializeField] public Color fishColor = Color.red;

    [Header("AI Settings")]
    [SerializeField] public Vector3 startPosition;
    [SerializeField] public float speed = 2f;
    [SerializeField] public float acceleration = 1f;
    [SerializeField] public float deceleration = 1f; // How fast it slows
    [SerializeField] public float turnSpeed = 90f; // Degrees per second
    [SerializeField] public float wanderRadius = 5f; // Radius for choosing random wander targets around the fish's current position
    [SerializeField] public float wanderInterval = 10f; // Time in seconds between choosing new wander target
    [SerializeField] public float fleeTime = 2f; // Time in seconds to flee after player or bait is detected
    [SerializeField] public float detectionRadius = 5f; // Radius in which it will react to the player or their bait
    [SerializeField] public float fleeSpeedMultiplier = 1.5f; // Multiplier for speed when fleeing from player or bait

    [Header("Fish Stats")]
    [SerializeField] public float health = 10f;
    [SerializeField] public float attack = 1f;  // Damage dealt to player's rod durability per second when player marker is in red zone
    [SerializeField] public float defense = 0f;

    [Header("Debug Settings")]
    [SerializeField] private GameObject debugWanderRadiusPrefab;
    [SerializeField] private GameObject debugDetectionRadiusPrefab;
    [SerializeField] private GameObject debugTargetPrefab;

    private Vector3 wanderTarget;
    private float currVelocity;
    private Transform baitTarget;
    private float distToTarget;
    private Coroutine lookAtTargetCoroutine;
    private float wanderTimer = 0f;
    private float fleeTimer = 0f;

    private FishState fishState = FishState.Wandering;
    private SphereCollider detectionCollider;

    #region Debug Variables
    private LineRenderer lineWanderRadius;
    private LineRenderer lineDetectionRadius;

    #endregion


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (startPosition == Vector3.zero)
        {
            startPosition = transform.position;
        }

        // Create detection collider, using detection radius for size
        detectionCollider = gameObject.AddComponent<SphereCollider>();
        detectionCollider.isTrigger = true;
        detectionCollider.radius = detectionRadius;

        // Setup debug lines
        if (debugWanderRadiusPrefab != null)
        {
            lineWanderRadius = HelperClass.InitRenderLine(debugWanderRadiusPrefab, 10, 0.1f);
        }

        if (debugDetectionRadiusPrefab != null)
        {
            lineDetectionRadius = HelperClass.InitRenderLine(debugDetectionRadiusPrefab, 10, 0.1f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        DrawWanderRadius();
        DrawDetectionRadius();
        DrawWanderTarget();
    }

    private void FixedUpdate()
    {
        StateMachine();
    }

#region AI Behavior
    private void StateMachine()
    {
        switch(fishState)
        {
            case FishState.Wandering:
                wanderTimer -= Time.fixedDeltaTime;
                if (wanderTimer <= 0f)
                {
                    // Choose a new random target within the wander radius
                    Vector2 randomCircle = Random.insideUnitCircle * wanderRadius;
                    wanderTarget = startPosition + new Vector3(randomCircle.x, 0, randomCircle.y);
                    wanderTimer = wanderInterval;
                }
                MoveTowards(wanderTarget);
                break;
            case FishState.Interested:
                if (baitTarget == null)
                {
                    fishState = FishState.Wandering;
                    break;
                }
                if (currVelocity < 0.1f && Vector3.Angle(transform.forward, (baitTarget.position-transform.position).normalized) > 2f)
                {
                    if (lookAtTargetCoroutine == null)
                    {
                        lookAtTargetCoroutine = StartCoroutine(LookAtTarget(baitTarget));
                    }
                    break;
                }
                wanderTimer -= Time.fixedDeltaTime;
                if (wanderTimer <= 0f)
                {
                    // Choose a point between the fish and the bait to simulate hesitation
                    Vector3 directionToBait = (baitTarget.position - transform.position).normalized;
                    float hesitationDistance = distToTarget * Random.Range(0.15f, 0.3f); // Randomize hesitation between 15% and 30% of the total travel distance
                    wanderTarget = transform.position + directionToBait * hesitationDistance;
                    wanderTimer = wanderInterval * 1.2f;
                    Debug.Log("Fish is moving " + hesitationDistance.ToString("F2") + " units towards the bait, simulating hesitation.");
                }
                MoveTowards(wanderTarget);
                break;
        }
    }

    private void MoveTowards(Vector3 targetPosition = default)
    {
        // Move towards the random point
        Vector3 directionToTarget = (targetPosition - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
        
        // Give fish the illusion of velocity and friction as it moves through the water
        // Check distance against max speed to prevent fish from overshooting and oscillating around the target point
        if (Vector3.Distance(transform.position, targetPosition) > currVelocity)
        {
            currVelocity = Mathf.Lerp(currVelocity, speed, acceleration * Time.deltaTime);
        }
        else
        {
            currVelocity = Mathf.Lerp(currVelocity, 0f, deceleration * Time.deltaTime);
        }
        transform.position += transform.forward * currVelocity * Time.deltaTime;
    }

    IEnumerator LookAtTarget(Transform target)
    {
        while (target != null)
        {
            Vector3 directionToTarget = (target.position - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);

            // Check if we're facing close enough to the target (within ~2 degrees)
            if (Vector3.Angle(transform.forward, directionToTarget) < 2f)
            {
                break;
            }

            yield return null; // Wait one frame before continuing the loop
        }
        lookAtTargetCoroutine = null; // Clear the coroutine reference when done
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bait"))
        {
            Projectile projectile = other.GetComponent<Projectile>();
            if (projectile == null || !projectile.HasLanded) return;
            baitTarget = other.transform;
            distToTarget = Vector3.Distance(transform.position, baitTarget.position);
            fishState = FishState.Interested;
            StartCoroutine(LookAtTarget(baitTarget));
            return;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Bait"))
        {
            Projectile projectile = other.GetComponent<Projectile>();
            if (!projectile.HasLanded || baitTarget != null) return;
            Debug.Log("Fish spotted bait and is now interested.");
            baitTarget = other.transform;
            distToTarget = Vector3.Distance(transform.position, baitTarget.position);
            fishState = FishState.Interested;
            StartCoroutine(LookAtTarget(baitTarget));
            return;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Bait"))
        {
            Debug.Log("Fish lost sight of bait.");
            baitTarget = null;
            fishState = FishState.Wandering;
            return;
        }
    }

    #endregion

    #region Private Helper Methods
    private void DrawWanderRadius()
    {
        if (lineWanderRadius == null) return;

        for (int i = 0; i < lineWanderRadius.positionCount; i++)
        {
            float angle = (float)i / lineWanderRadius.positionCount * Mathf.PI * 2f;
            Vector3 pointOnCircle = startPosition + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * wanderRadius;
            lineWanderRadius.SetPosition(i, pointOnCircle);
        }
    }

    private void DrawDetectionRadius()
    {
        if (lineDetectionRadius == null) return;

        for (int i = 0; i < lineDetectionRadius.positionCount; i++)
        {
            float angle = (float)i / lineDetectionRadius.positionCount * Mathf.PI * 2f;
            Vector3 pointOnCircle = transform.position + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * detectionRadius;
            lineDetectionRadius.SetPosition(i, pointOnCircle);
        }
    }

    private void DrawWanderTarget()
    {
        if (debugTargetPrefab == null) return;

        debugTargetPrefab.transform.position = wanderTarget;
    }

    #endregion
}
