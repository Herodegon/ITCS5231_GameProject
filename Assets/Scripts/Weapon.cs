using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("Weapon Settings")]
    [SerializeField] private float damage = 1f; // Damage dealt to target
    [SerializeField] private float fireRate = 1f; // Amount of time in seconds damage is applied to target after firing
    [SerializeField] private float range = 100f;
    [SerializeField] private float castSpeed = 3f; // Time in seconds for the shot to reach max range 
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private string weaponName;

    public void Fire(Vector3 cursorPosition)
    {
        if (projectilePrefab != null)
        {
            GameObject projectileInstance = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
            Projectile projectileScript = projectileInstance.GetComponent<Projectile>();
            if (projectileScript != null)
            {
                projectileScript.LaunchInArc(cursorPosition, castSpeed);
            }
        }
    }
}
