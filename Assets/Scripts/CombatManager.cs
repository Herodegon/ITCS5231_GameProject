using UnityEngine;
using UnityEngine.InputSystem;

public class CombatManager : MonoBehaviour
{
    [Header("Combat Settings")]
    [SerializeField] public GameObject player;

    [SerializeField] private float reelSpeed = 2f;
    [SerializeField] private float fishStrength = 5f;
    [SerializeField] private float tensionThreshold = 10f; // Break point
    [SerializeField] private float catchDistance = 2f; // Distance to reel in fish
    
    [Header("UI")]
    [SerializeField] private GameObject combatUI;
    
    private GameObject caughtFish;
    private Vector3 playerPullDirection;
    private Vector3 fishPullDirection;
    private float currentTension;
    private float currentDistance;
    private bool inCombat;
    
    private InputActionAsset inputActions;
    private Vector2 pullInput;

    void Start()
    {
        inputActions = player.GetComponent<PlayerInput>().actions;
        if (combatUI != null) combatUI.SetActive(false);
    }

    public void StartFishingCombat(GameObject fish)
    {
        inCombat = true;
        caughtFish = fish;
        currentDistance = Vector3.Distance(transform.position, fish.transform.position);
        currentTension = 0f;
        
        if (combatUI != null) combatUI.SetActive(true);
        
        // Disable normal movement
        player.GetComponent<PlayerController>().enabled = false;
    }

    void Update()
    {
        if (!inCombat || caughtFish == null) return;
        
        // Fish AI - changes direction periodically
        fishPullDirection = GetFishPullDirection();
        
        // Player pulls with input
        playerPullDirection = new Vector3(pullInput.x, 0, pullInput.y).normalized;
        
        // Calculate if player is pulling opposite to fish
        float alignment = Vector3.Dot(playerPullDirection, fishPullDirection);
        
        if (alignment < -0.5f && pullInput.magnitude > 0.1f)
        {
            // Pulling correctly - reel in fish
            currentDistance -= reelSpeed * Time.deltaTime;
            currentTension = Mathf.Max(0, currentTension - Time.deltaTime * 2f);
        }
        else if (pullInput.magnitude > 0.1f)
        {
            // Pulling wrong direction - increase tension
            currentTension += Time.deltaTime * fishStrength;
        }
        
        // Check win/lose conditions
        if (currentTension >= tensionThreshold)
        {
            // Line broke!
            EndCombat(false);
        }
        else if (currentDistance <= catchDistance)
        {
            // Fish caught!
            EndCombat(true);
        }
        
        // Update fish position visually
        UpdateFishPosition();
    }

    void OnPull(InputValue value)
    {
        pullInput = value.Get<Vector2>();
    }

    private Vector3 GetFishPullDirection()
    {
        // Simple AI: fish changes direction every few seconds
        float time = Time.time;
        float angle = Mathf.PerlinNoise(time * 0.5f, 0f) * 360f;
        return new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));
    }

    private void UpdateFishPosition()
    {
        if (caughtFish != null)
        {
            Vector3 direction = (caughtFish.transform.position - transform.position).normalized;
            caughtFish.transform.position = transform.position + direction * currentDistance;
        }
    }

    private void EndCombat(bool success)
    {
        inCombat = false;
        
        if (success)
        {
            // Add fish to inventory, grant rewards
            Destroy(caughtFish);
        }
        else
        {
            // Fish escapes
            caughtFish = null;
        }
        
        if (combatUI != null) combatUI.SetActive(false);
        player.GetComponent<PlayerController>().enabled = true;
    }
}