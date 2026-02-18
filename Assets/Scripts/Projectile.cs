using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField] public Vector3 hitboxSize = new Vector3(0.5f, 0.5f, 0.5f);
    
    [SerializeField] private int numPoints = 32;
    
    private Vector3 startPosition;
    private Vector3 targetPosition;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Fish"))
        {
            // Notify weapon/manager that fish was hit
            SendMessageUpwards("OnFishHit", other.gameObject, SendMessageOptions.DontRequireReceiver);
        }
    }

    public void LaunchInArc(Vector3 target, float castSpeed)
    {
        startPosition = transform.position;
        targetPosition = target;

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
            currentPosition.y += Mathf.Sin(t * Mathf.PI) * 2f; // Adjust the height of the arc

            transform.position = currentPosition;
            yield return null;
        }

        // Ensure the projectile ends at the target position
        transform.position = targetPosition;
    }
}