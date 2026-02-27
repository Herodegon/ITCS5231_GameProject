using UnityEditor;
using UnityEngine;

enum FishState
{
    Wandering,
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
    [SerializeField] public float turnSpeed = 90f; // Degrees per second
    [SerializeField] public float wanderRadius = 5f; // Radius for choosing random wander targets around the fish's current position
    [SerializeField] public float wanderInterval = 10f; // Time in seconds between choosing new wander target
    [SerializeField] public float detectionRadius = 5f; // Radius in which it will react to the player or their bait
    [SerializeField] public float fleeSpeedMultiplier = 1.5f; // Multiplier for speed when fleeing from player or bait

    [Header("Fish Stats")]
    [SerializeField] public float health = 10f;
    [SerializeField] public float attack = 1f;  // Damage dealt to player's rod durability per second when player marker is in red zone
    [SerializeField] public float defense = 0f;

    private Vector3 wanderTarget;
    private float wanderTimer = 0f;

    private FishState fishState = FishState.Wandering;
    private LineRenderer lineRenderer;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (startPosition == Vector3.zero)
        {
            startPosition = transform.position;
        }

        lineRenderer = HelperClass.InitRenderLine(gameObject, 10, 0.1f);
    }

    // Update is called once per frame
    void Update()
    {
        DrawWanderRadius();
    }

    private void FixedUpdate()
    {
        StateMachine();
    }

    private void StateMachine()
    {
        switch(fishState)
        {
            case FishState.Wandering:
                Wander();
                wanderTimer -= Time.fixedDeltaTime;
                if (wanderTimer <= 0f)
                {
                    // Choose a new random target within the wander radius
                    Vector2 randomCircle = Random.insideUnitCircle * wanderRadius;
                    wanderTarget = startPosition + new Vector3(randomCircle.x, 0, randomCircle.y);
                    wanderTimer = wanderInterval;
                }
                break;
        }
    }

    private void Wander()
    {
        // Move towards the random point
        Vector3 directionToTarget = (wanderTarget - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    #region Private Helper Methods
    private void DrawWanderRadius()
    {
        if (lineRenderer == null) return;

        for (int i = 0; i < lineRenderer.positionCount; i++)
        {
            float angle = (float)i / lineRenderer.positionCount * Mathf.PI * 2f;
            Vector3 pointOnCircle = startPosition + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * wanderRadius;
            lineRenderer.SetPosition(i, pointOnCircle);
        }
    }

    #endregion
}
