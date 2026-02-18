using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [Header("Player Equipment Settings")]
    [SerializeField] public GameObject weapon;
    [SerializeField] public GameObject projectile;
    [SerializeField] public Dictionary<string, List<GameObject>> items = new Dictionary<string, List<GameObject>>();
    [SerializeField] public Transform weaponMountPoint;

    void Start()
    {
        EquipWeapon(weapon);
    }

    private void EquipWeapon(GameObject newWeapon)
    {
        weapon = Instantiate(newWeapon, weaponMountPoint.position, weaponMountPoint.rotation, weaponMountPoint);
        weapon.transform.localPosition -= weapon.GetComponent<Weapon>().equipPoint.localPosition; // Adjust position based on equip point
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void UseWeapon(Vector3 cursorPosition)
    {
        if (weapon != null)
        {
            Weapon weaponScript = weapon.GetComponent<Weapon>();
            if (weaponScript != null)
            {
                weaponScript.Fire(projectile, cursorPosition);
            }
        }
    }
}
