using System.Collections;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public GameObject player;
    public GameObject fishTarget;
    public PopupManager popupManager;
    public PauseMenu pauseMenu;

    private PlayerController playerController;
    private WeaponManager weaponManager;
    private FishAI fishAI;
    private CameraController cameraController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (player == null) return;

        playerController = player.GetComponent<PlayerController>();
        if (playerController == null) return;

        weaponManager = playerController.weaponManager;
        cameraController = Camera.main != null ? Camera.main.GetComponent<CameraController>() : null;

        Projectile.OnFishHitEvent += OnFishHit;
        PauseMenu.OnPauseEvent += OnPause;

        if (fishTarget != null)
        {
            OnFishHit(fishTarget);
        }
    }

    // Update is called once per frame
    void Update()
    {
        return;
    }

    public void OnPause(bool paused)
    {
        playerController.OnPause(paused);
    }

    public void OnFishHit(GameObject fish)
    {
        if (fish == null || playerController == null) return;

        Debug.Log("CombatManager received hit on fish: " + fish.name);
        fishTarget = fish;
        fishAI = fishTarget.GetComponent<FishAI>();
        if (fishAI == null) return;

        playerController.EnterCombat(fishTarget);
        fishAI.EnterCombat();
        cameraController.EnterCombatView(player.transform, fishTarget.transform);
        StartCoroutine(DealDamageToFish());
    }

    IEnumerator DealDamageToFish()
    {
        while (fishAI != null && fishTarget != null && weaponManager != null)
        {
            float damage = weaponManager.CalculateDamage();
            fishAI.TakeDamage(damage);
            popupManager.GenDamagePopup(damage, fishTarget.transform.position);
            if (fishAI.isCaptured)
            {
                break;
            }
            yield return new WaitForSeconds(weaponManager.CalculateFireRate());
        }
        cameraController.ExitCombatView();
        if (playerController != null && playerController.fishHooked != null)
        {
            playerController.ExitCombat();
        }
    }

    private void OnDestroy()
    {
        Projectile.OnFishHitEvent -= OnFishHit;
    }
}
