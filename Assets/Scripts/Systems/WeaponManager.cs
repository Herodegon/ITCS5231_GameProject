using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [Header("Player Equipment Settings")]
    public GameObject weapon;
    public GameObject projectile;
    public Dictionary<string, List<GameObject>> items = new();
    public Transform weaponMountPoint;

    private Weapon weaponScript;
    
    [Header("Projectile Settings")]
    [SerializeField] private GameObject projectileContainer;
    [SerializeField] private int trajectoryLinePoints = 16;
    private LineRenderer trajectoryLine;

    void Start()
    {
        EquipWeapon(weapon);

        // Create LineRenderer for trajectory visualization
        trajectoryLine = HelperClass.InitRenderLine(gameObject, trajectoryLinePoints);
    }

    public void UseWeapon(Vector3 cursorPosition)
    {
        if (weapon == null || weaponScript == null) return;

        weaponScript.Fire(projectile, cursorPosition, projectileContainer);
    }

    public float CalculateDamage()
    {
        // Damage calculation based on weapon type, stats, and modifiers
        // TODO: Implement damage calculation logic
        return weaponScript.damage; 
    }

    public float CalculateFireRate()
    {
        // Fire rate calculation based on weapon type, stats, and modifiers
        // TODO: Implement fire rate calculation logic
        return weaponScript.fireRate;
    }

    public void AddItem(string itemType, GameObject itemPrefab)
    {
        if (!items.ContainsKey(itemType))
        {
            items[itemType] = new List<GameObject>();
        }
        items[itemType].Add(itemPrefab);
    }

    public void DrawTrajectory(Vector3 targetPosition)
    {
        if (weapon == null || weaponScript == null) return;

        if (targetPosition == Vector3.zero)
        {
            trajectoryLine.enabled = false;
            return;
        }
        else if (!trajectoryLine.enabled)
        {
            trajectoryLine.enabled = true;
        }

        Vector3[] trajectoryPoints = weaponScript.CalculateTrajectory(targetPosition, trajectoryLinePoints);
        trajectoryLine.SetPositions(trajectoryPoints);
    }

    private void EquipWeapon(GameObject newWeapon)
    {
        weapon = Instantiate(newWeapon, weaponMountPoint.position, weaponMountPoint.rotation, weaponMountPoint);
        weaponScript = weapon.GetComponent<Weapon>();
        if (weaponScript != null)
        {
            weapon.transform.localPosition -= weaponScript.equipPoint.localPosition; // Adjust position based on equip point
        }
    }
}
