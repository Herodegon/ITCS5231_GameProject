using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class CombatManager : MonoBehaviour
{
    public GameObject player;
    public GameObject fishTarget;
    public PopupManager popupManager;
    public PauseMenu pauseMenu;

    [Header("Combat Settings")]
    public float destroyDelay = 1f;

    [Header("Debug")]
    [SerializeField] private bool debugMousePopup = true;
    [SerializeField] private float debugDamageValue = 42f;
    [SerializeField] private LayerMask debugRaycastMask = ~0; // everything
    [SerializeField] private Sprite debugFishSprite;

    private PlayerController playerController;
    private WeaponManager weaponManager;
    private FishAI fishAI;
    private CameraController cameraController;
    private Coroutine damageRoutine;

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
        PlayerController.AddFishToInventoryEvent += OnAddFishToInventory;

        if (fishTarget != null)
        {
            OnFishHit(fishTarget);
        }
    }

    void Update()
    {
        if (!debugMousePopup || popupManager == null) return;
        
        if (Mouse.current != null && Mouse.current.rightButton.isPressed)
        {
            popupManager.GenDamagePopup(debugDamageValue, HelperClass.GetMouseWorldPosition(Camera.main, 100f, debugRaycastMask));;
        }
    }

    void LateUpdate()
    {
        if (fishTarget != null)
        {
            float angleToFish = Vector3.Angle(player.transform.forward, fishTarget.transform.position - player.transform.position);
            pauseMenu.GetComponent<fishBarCreator>().updateBar((int)fishAI.health*5, (angleToFish+90f)/180f);
        }
    }

    public void OnPause(bool paused)
    {
        playerController.OnPause(paused);
    }

    public void OnAddFishToInventory(string fishName, int quantity)
    {
        if (pauseMenu != null)
        {
            // TODO: Store fish sprite in FishStats structure and send it as parameter instead of `string fishName`
            pauseMenu.addFish(fishName, quantity, debugFishSprite);
        }
        else
        {
            Debug.LogWarning("PauseMenu not found, cannot add fish to inventory");
        }
    }

    public void OnFishHit(GameObject fish)
    {
        if (fish == null || playerController == null) return;

        Debug.Log("CombatManager received hit on fish: " + fish.name);
        fishTarget = fish;
        fishAI = fishTarget.GetComponent<FishAI>();
        if (fishAI == null) return;

        StopDamageRoutine();
        playerController.EnterCombat(fishTarget);
        fishAI.EnterCombat();
        if (cameraController != null)
        {
            cameraController.EnterCombatView(player.transform, fishTarget.transform);
        }
        damageRoutine = StartCoroutine(DealDamageToFish(fishTarget, fishAI));

        pauseMenu.GetComponent<fishBarCreator>().createBar((int)fishAI.health*5, 100);
    }

    IEnumerator DealDamageToFish(GameObject targetFish, FishAI targetFishAI)
    {
        while (targetFishAI != null && targetFish != null && weaponManager != null)
        {
            if (fishTarget != targetFish || fishAI != targetFishAI)
            {
                yield break;
            }

            float damage = weaponManager.CalculateDamage();
            targetFishAI.TakeDamage(damage);
            if (popupManager != null)
            {
                popupManager.GenDamagePopup(damage, targetFish.transform.position);
            }
            if (targetFishAI.isCaptured)
            {
                break;
            }

            float fireInterval = Mathf.Max(0.02f, weaponManager.CalculateFireRate());
            yield return new WaitForSeconds(fireInterval);
        }

        if (cameraController != null)
        {
            cameraController.ExitCombatView();
        }
        if (playerController != null && playerController.fishHooked != null)
        {
            pauseMenu.GetComponent<fishBarCreator>().destroyBar();
            playerController.ExitCombat(destroyDelay);
        }
        damageRoutine = null;
    }

    private void StopDamageRoutine()
    {
        if (damageRoutine != null)
        {
            StopCoroutine(damageRoutine);
            damageRoutine = null;
        }
    }

    private void OnDestroy()
    {
        StopDamageRoutine();
        Projectile.OnFishHitEvent -= OnFishHit;
        PauseMenu.OnPauseEvent -= OnPause;
    }
}
