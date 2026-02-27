using UnityEngine;
using UnityEngine.InputSystem;

enum PlayerState
{
    Moving,
    FishSnagged
}

public class PlayerController : MonoBehaviour
{
    [Header("Player Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float acceleration = 2f;
    [SerializeField] private float deceleration = 1f; // How fast boat slows down
    [SerializeField] private float turnSpeed = 90f; // Degrees per second
    [SerializeField] private float driftTurnRate = 0.5f; // How much velocity turns with the boat
    [SerializeField] private LayerMask layerMask;

    [Header("Combat Settings")]
    [SerializeField] private WeaponManager weaponManager;

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
            weaponManager.UseWeapon(HelperClass.GetMouseWorldPosition(Camera.main, 100f, layerMask));
        }
    }

    #endregion

    #region Physics and Movement
    private void FixedUpdate()
    {
        UpdateRotation();
        UpdateMovement();
    }

    private void UpdateRotation()
    {
        // Horizontal input (A/D or Left/Right) controls turning
        if (Mathf.Abs(movementInput.x) > 0.1f)
        {
            float turnAmount = movementInput.x * turnSpeed * Time.fixedDeltaTime;
            Quaternion turnRotation = Quaternion.Euler(0f, turnAmount, 0f);
            transform.rotation = transform.rotation * turnRotation;
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
            Vector3 targetVelocity = transform.forward * forwardInput * moveSpeed;
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