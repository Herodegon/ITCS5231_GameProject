using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

enum PlayerState
{
    Moving,
    Combat
}

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float acceleration = 2f;
    [SerializeField] private float deceleration = 1f; // How fast boat slows down
    [SerializeField] private float turnSpeed = 90f; // Degrees per second
    [SerializeField] private float driftTurnRate = 0.5f; // How much velocity turns with the boat
    [SerializeField] private LayerMask layerMask;

    [Header("Combat Settings")]
    public WeaponManager weaponManager;
    public GameObject fishHooked;
    public float tetherRange = 10f; // Max radius from player to fish before it starts to pull the player towards it
    private PlayerState playerState = PlayerState.Moving;
    
    [Header("Player Stats")]
    public float hp = 100f;
    public float attack = 1f; // Damage dealt to fish's health per second when player marker is in green zone
    public float pullStrength = 1f; // Strength of the pull when the player is within the tether range

    [Header("Debug Settings")]
    [SerializeField] private bool showVelocity = false;
    [SerializeField] private float velocityDebugScale = 1f;

    #region Physics Variables
    private Rigidbody playerRigidbody;
    private Vector3 currentVelocity;
    private LineRenderer velocityLine;

    #endregion

    #region Input Variables
    private InputActionAsset inputActions;
    private InputAction aimAction;
    private Vector2 movementInput;
    private bool isAiming;

    #endregion

    void Start()
    {
        // Reset global input action map based on Unity recommendations
        inputActions = GetComponent<PlayerInput>().actions;
        inputActions.Disable();
        inputActions.FindActionMap("Player").Enable();

        aimAction = inputActions.FindAction("Aim");

        // Create LineRenderer
        velocityLine = HelperClass.InitRenderLine(gameObject, 2, 0.1f);

        // Initialize velocity and direction
        playerRigidbody = GetComponent<Rigidbody>();
        currentVelocity = Vector3.zero;
    }

    void Update()
    {
        #region Input Checking
        if (aimAction != null)
        {
            if (aimAction.WasPressedThisFrame())
            {
                Debug.Log("Aim input received");
                isAiming = true;
            }
            else if (aimAction.WasReleasedThisFrame())
            {
                Debug.Log("Aim input released");
                isAiming = false;
            }
        }

        #endregion

        #region State Transitions
        StateMachine();

        #endregion

        #region Visuals
        DrawVelocityLine();
        if (isAiming)
        {
            weaponManager.DrawTrajectory(HelperClass.GetMouseWorldPosition(Camera.main, 100f, layerMask));
        }
        else if (aimAction != null && aimAction.WasReleasedThisFrame())
        {
            weaponManager.DrawTrajectory(Vector3.zero);
        }

        #endregion
    }

    #region Input Callbacks
    public void OnMove(InputValue value)
    {
        movementInput = value.Get<Vector2>();
    }

    public void OnFire(InputValue value)
    {
        Debug.Log("Fire input received");
        if (value.isPressed)
        {
            // Prevent firing non-harpoon weapons while tethered to a fish
            Weapon weapon = weaponManager.weapon.GetComponent<Weapon>();
            if (weapon.weaponType != WeaponType.Harpoon && fishHooked != null) return;

            weaponManager.UseWeapon(HelperClass.GetMouseWorldPosition(Camera.main, 100f, layerMask));
        }
    }

    #endregion

    #region Physics and Movement
    private void FixedUpdate()
    {
        switch(playerState)
        {
            case PlayerState.Moving:
                UpdateRotation();
                UpdateMovement();
                break;
            case PlayerState.Combat:
                UpdateTetheredRotation();
                UpdateTetheredMovement();
                break;
        }
    }

    public void EnterCombat(GameObject fish)
    {
        fishHooked = fish;
        playerState = PlayerState.Combat;
    }

    private void StateMachine()
    {
        switch(playerState)
        {
            case PlayerState.Moving:
                if (fishHooked != null)
                {
                    playerState = PlayerState.Combat;
                }
                break;
            case PlayerState.Combat:
                if (fishHooked == null)
                {
                    playerState = PlayerState.Moving;
                }
                break;
        }
    }

    private void UpdateRotation()
    {
        // Horizontal input (A/D or Left/Right) controls turning
        if (Mathf.Abs(movementInput.x) > 0.1f)
        {
            float turnAmount = movementInput.x * turnSpeed * Time.fixedDeltaTime;
            Quaternion turnRotation = Quaternion.Euler(0f, turnAmount, 0f);
            playerRigidbody.MoveRotation(playerRigidbody.rotation * turnRotation);
        }

        // Gradually turn the velocity direction toward where boat is facing (drift effect)
        if (currentVelocity.magnitude > 0.1f)
        {
            Vector3 currentVelDirection = currentVelocity.normalized;
            Vector3 targetDirection = transform.forward * Mathf.Sign(Vector3.Dot(currentVelDirection, transform.forward)); // Keep forward/backward direction
            
            // Smoothly rotate velocity toward facing direction
            currentVelDirection = Vector3.Slerp(currentVelDirection, targetDirection, driftTurnRate * Time.fixedDeltaTime);
            currentVelocity = currentVelDirection * currentVelocity.magnitude;
        }
    }

    private void UpdateMovement()
    {
        // Vertical input (W/S or Up/Down) controls forward/backward speed
        float forwardInput = movementInput.y;
        
        if (Mathf.Abs(forwardInput) > 0.1f)
        {
            // Accelerate in the direction the boat is facing
            Vector3 targetVelocity = transform.forward * (forwardInput * moveSpeed);
            currentVelocity = Vector3.Lerp(currentVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);
        }
        else
        {
            // Decelerate when no input (boat drifts to a stop)
            currentVelocity = Vector3.Lerp(currentVelocity, Vector3.zero, deceleration * Time.fixedDeltaTime);
        }

        // Apply movement to Rigidbody
        playerRigidbody.MovePosition(playerRigidbody.position + currentVelocity * Time.fixedDeltaTime);
    }

    private void UpdateTetheredMovement()
    {
        if (fishHooked == null) return;
        float fishPullStrength = fishHooked.GetComponent<FishAI>().pullStrength;
        float fishPullSpeed = moveSpeed;
        float fishPullAcceleration = acceleration;
        float strafe = movementInput.x;
        float back = Mathf.Clamp01(-movementInput.y);

        Vector3 fishPosition = fishHooked.transform.position;
        Vector3 playerPosition = playerRigidbody.position;
        Vector3 fishDirection = (fishPosition - playerPosition).normalized;

        float distanceToFish = Vector3.Distance(playerPosition, fishPosition);
        if (distanceToFish >= tetherRange)
        {
            // "Tautness" ramps up quickly once the tether starts to pull.
            float tautOver = distanceToFish - tetherRange;
            float tautRampDistance = Mathf.Max(0.01f, tetherRange * 0.3f);
            float taut01 = Mathf.Clamp01(tautOver / tautRampDistance);
            taut01 = Mathf.SmoothStep(0f, 1f, taut01);

            // Start pulling at tetherRange, then strengthen as the line stretches further.
            fishPullSpeed *= 1f + fishPullStrength * 0.5f * taut01;
            fishPullAcceleration *= 1f + fishPullStrength * 0.5f * taut01;
        }
        float lerpT = fishPullAcceleration * Time.fixedDeltaTime;
        currentVelocity = Vector3.Lerp(currentVelocity, fishDirection * fishPullSpeed, Mathf.Clamp01(lerpT));

        playerRigidbody.MovePosition(playerPosition + currentVelocity * Time.fixedDeltaTime);
    }

    private void UpdateTetheredRotation()
    {
        Transform fishTransform = fishHooked.transform;
        Vector3 lookDir = fishTransform.position - playerRigidbody.position;
        lookDir.y = 0;
        if (lookDir.sqrMagnitude > 0.01f)
        {
            Quaternion targetRot = Quaternion.LookRotation(lookDir);
            playerRigidbody.MoveRotation(Quaternion.Slerp(
            playerRigidbody.rotation, targetRot, turnSpeed * Time.fixedDeltaTime));
        }
    }
    #endregion

    #region Private Helper Methods
    private void DrawVelocityLine()
    {
        if (showVelocity)
        {
            velocityLine.enabled = true;
            velocityLine.SetPosition(0, transform.position);
            velocityLine.SetPosition(1, transform.position + currentVelocity * velocityDebugScale);
            
            Color lineColor = Color.Lerp(Color.green, Color.red, currentVelocity.magnitude / moveSpeed);
            velocityLine.startColor = lineColor;
            velocityLine.endColor = lineColor;
        }
        else
        {
            velocityLine.enabled = false;
        }
    }

    #endregion
}