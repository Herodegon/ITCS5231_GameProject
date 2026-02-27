using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [Header("Player Equipment Settings")]
    [SerializeField] public GameObject weapon;
    [SerializeField] public GameObject projectile;
    [SerializeField] public Dictionary<string, List<GameObject>> items = new Dictionary<string, List<GameObject>>();
    [SerializeField] public Transform weaponMountPoint;
    [SerializeField] private int trajectoryLinePoints = 16;

    private LineRenderer trajectoryLine;

    void Start()
    {
        EquipWeapon(weapon);

        // Create LineRenderer for trajectory visualization
        trajectoryLine = HelperClass.InitRenderLine(gameObject, trajectoryLinePoints);
    }

    private void EquipWeapon(GameObject newWeapon)
    {
        weapon = Instantiate(newWeapon, weaponMountPoint.position, weaponMountPoint.rotation, weaponMountPoint);
        weapon.transform.localPosition -= weapon.GetComponent<Weapon>().equipPoint.localPosition; // Adjust position based on equip point
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void UseWeapon(Vector3 cursorPosition)
    {
        if (weapon == null) return;

        Weapon weaponScript = weapon.GetComponent<Weapon>();
        if (weaponScript != null)
        {
            weaponScript.Fire(projectile, cursorPosition);
        }
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
        if (weapon == null) return;

        if (targetPosition == Vector3.zero)
        {
            trajectoryLine.enabled = false;
            return;
        }
        else if (!trajectoryLine.enabled)
        {
            trajectoryLine.enabled = true;
        }

        Weapon weaponScript = weapon.GetComponent<Weapon>();
        if (weaponScript != null)
        {
            Vector3[] trajectoryPoints = weaponScript.CalculateTrajectory(targetPosition, trajectoryLinePoints);
            trajectoryLine.SetPositions(trajectoryPoints);
        }
    }
}
