using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("Weapon Settings")]
    [SerializeField] private string weaponName;
    [SerializeField] public WeaponType weaponType;
    [SerializeField] public float damage = 1f; // Damage dealt to target
    [SerializeField] public float fireRate = 1f; // Amount of time in seconds damage is applied to target after firing
    [SerializeField] public float range = 10f;
    [SerializeField] public float castSpeed = 2f; // Time in seconds for the shot to reach max range 
    [SerializeField] public Transform equipPoint;

    public FishingLine fishingLine;

    [Header("References")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private int lineNodeCount = 25;

    private GameObject projectileInstance;

    public void Fire(GameObject projectilePrefab, Vector3 cursorPosition)
    {
        if (projectilePrefab == null) return;

        // Clean up any existing cast
        CleanUpCast();

        // Spawn projectile
        projectileInstance = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Projectile projectileScript = projectileInstance.GetComponent<Projectile>();
        if (projectileScript != null)
        {
            projectileScript.LaunchInArc(cursorPosition, castSpeed);
        }

        // Spawn fishing line between firePoint and the projectile
        SpawnFishingLine(projectileInstance.transform);
    }

    private void SpawnFishingLine(Transform target)
    {
        GameObject lineObj = new GameObject("FishingLine");
        fishingLine = lineObj.AddComponent<FishingLine>();
        fishingLine.Initialize(firePoint, target, lineNodeCount);
    }

    public void CleanUpCast()
    {
        if (fishingLine != null)
        {
            fishingLine.Deactivate();
            Destroy(fishingLine.gameObject);
            fishingLine = null;
        }

        if (projectileInstance != null)
        {
            Destroy(projectileInstance);
            projectileInstance = null;
        }
    }

    void OnDestroy()
    {
        CleanUpCast();
    }

    public Vector3[] CalculateTrajectory(Vector3 targetPosition, int points)
    {
        Vector3[] trajectoryPoints = new Vector3[points];
        switch(weaponType)
        {
            case WeaponType.Rod:
                for (int i = 0; i < points; i++)
                {
                    float t = (float)i / (points - 1);
                    Vector3 point = Vector3.Lerp(firePoint.position, targetPosition, t);
                    point.y += Mathf.Sin(t * Mathf.PI) * 2f; // Add vertical arc
                    trajectoryPoints[i] = point;
                }
                break;
            default:
                for (int i = 0; i < points; i++)
                {
                    float t = (float)i / (points - 1);
                    trajectoryPoints[i] = Vector3.Lerp(firePoint.position, targetPosition, t);
                }
                break;
        }
        return trajectoryPoints;
    }
}
