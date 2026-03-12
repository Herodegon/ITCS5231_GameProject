using UnityEngine;

public class CombatManager : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject fishTarget;

    private PlayerController playerController;
    private WeaponManager weaponManager;
    private FishAI fishAI;

    private bool inCombat = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerController = player.GetComponent<PlayerController>();
        weaponManager = playerController.weaponManager;

        Projectile.OnFishHitEvent += OnFishHit;

        if (fishTarget != null)
        {
            OnFishHit(fishTarget);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnFishHit(GameObject fish)
    {
        Debug.Log("CombatManager received hit on fish: " + fish.name);
        fishTarget = fish;
        fishAI = fishTarget.GetComponent<FishAI>();
        playerController.EnterCombat(fishTarget);
        fishAI.EnterCombat();
        inCombat = true;
    }
}
