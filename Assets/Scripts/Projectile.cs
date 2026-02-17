using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField] private float speed = 20f;
    [SerializeField] private float damage = 0f;
    [SerializeField] private float lifetime = 5f;
    [SerializeField] private float arcHeight = 3f; // Height of parabolic arc
    
    private Vector3 startPosition;
    private Vector3 targetPosition;
    private float travelTime;
    private float elapsedTime;
    private bool isArcMotion = false;

    private Rigidbody projectileRigidbody;

    void Start()
    {
        projectileRigidbody = GetComponent<Rigidbody>();
        Destroy(gameObject, lifetime);
    }

    public void LaunchInArc(Vector3 target, float castSpeed)
    {
        isArcMotion = true;
        startPosition = transform.position;
        targetPosition = target;
        travelTime = castSpeed;
        elapsedTime = 0f;
        
        projectileRigidbody.useGravity = false;
        projectileRigidbody.isKinematic = true;
    }

    void Update()
    {
        if (isArcMotion)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / travelTime);
            
            // Parabolic arc calculation
            Vector3 currentPos = Vector3.Lerp(startPosition, targetPosition, t);
            currentPos.y += arcHeight * Mathf.Sin(t * Mathf.PI); // Parabolic height
            
            transform.position = currentPos;
            
            if (t >= 1f)
            {
                isArcMotion = false;
                OnReachedDestination();
            }
        }
    }

    void OnReachedDestination()
    {
        // Bob is now floating, waiting for fish
        projectileRigidbody.useGravity = true;
        projectileRigidbody.isKinematic = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Fish"))
        {
            // Notify weapon/manager that fish was hit
            SendMessageUpwards("OnFishHit", other.gameObject, SendMessageOptions.DontRequireReceiver);
        }
    }
}