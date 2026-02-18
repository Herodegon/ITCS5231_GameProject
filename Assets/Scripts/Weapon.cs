using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("Weapon Settings")]
    [SerializeField] private string weaponName;
    [SerializeField] public float damage = 1f; // Damage dealt to target
    [SerializeField] public float fireRate = 1f; // Amount of time in seconds damage is applied to target after firing
    [SerializeField] public float range = 100f;
    [SerializeField] public float castSpeed = 2f; // Time in seconds for the shot to reach max range 
    [SerializeField] public Transform equipPoint;

    [SerializeField] private Transform firePoint;

    private GameObject projectileInstance;

    public void Fire(GameObject projectilePrefab, Vector3 cursorPosition)
    {
        if (projectilePrefab != null)
        {
            // Clean up any existing projectile first
            if (projectileInstance != null)
            {
                Destroy(projectileInstance);
            }

            projectileInstance = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
            Projectile projectileScript = projectileInstance.GetComponent<Projectile>();
            if (projectileScript != null)
            {
                projectileScript.LaunchInArc(cursorPosition, castSpeed);
            }
        }
    }
}
