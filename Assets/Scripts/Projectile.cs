using UnityEngine;

public class Projectile : MonoBehaviour
{
    public static System.Action<GameObject> OnFishHitEvent;

    [Header("Projectile Settings")]
    [SerializeField] public Vector3 hitboxSize = new Vector3(0.5f, 0.5f, 0.5f);

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private bool hasLanded;
    
    public bool HasLanded => hasLanded;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Fish"))
        {
            Debug.Log("Projectile hit a fish!");

            // Send message to combat manager with the hit fish to start combat
            // SendMessageUpwards("OnFishHit", other.transform.parent.gameObject, SendMessageOptions.DontRequireReceiver);

            GameObject hitFish = other.transform.parent.gameObject;
            OnFishHitEvent?.Invoke(hitFish);

            transform.parent = hitFish.transform;
            transform.localPosition = Vector3.zero; // Attach to the center of the fish
            GetComponent<Collider>().enabled = false; // Disable hitbox after hitting a fish
        }
    }

    public void LaunchInArc(Vector3 target, float castSpeed)
    {
        startPosition = transform.position;
        targetPosition = target;
        hasLanded = false;

        // Start the arc movement
        StartCoroutine(ArcMovement(castSpeed));
    }

    private System.Collections.IEnumerator ArcMovement(float castSpeed)
    {
        float elapsedTime = 0f;
        while (elapsedTime < castSpeed)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / castSpeed;

            // Calculate the arc position using a parabolic formula
            Vector3 currentPosition = Vector3.Lerp(startPosition, targetPosition, t);
            currentPosition.y += Mathf.Sin(t * Mathf.PI) * 2f; // Add vertical arc

            transform.position = currentPosition;
            yield return null;
        }

        // Ensure the projectile ends at the target position
        transform.position = targetPosition;
        hasLanded = true;
    }
}