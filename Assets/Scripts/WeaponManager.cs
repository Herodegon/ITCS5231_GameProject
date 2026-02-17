using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [Header("Player Equipment Settings")]
    [SerializeField] private Weapon weapon;
    [SerializeField] private Dictionary<string, List<GameObject>> items = new Dictionary<string, List<GameObject>>();
    [SerializeField] private Transform weaponMountPoint;
    private GameObject currentTarget;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void UseWeapon(Vector3 cursorPosition)
    {
        if (weapon != null)
        {
            weapon.Fire(cursorPosition);
        }
    }
}
