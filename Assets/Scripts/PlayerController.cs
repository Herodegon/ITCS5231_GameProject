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

    private Rigidbody playerRigidbody;
    private Vector3 currentVelocity;
    private Vector3 currentDirection;
    private LineRenderer velocityLine;

    private InputActionAsset inputActions;
    private Vector2 movementInput;

    void Start()
    {
        // Reset global input action map based on Unity recommendations
        inputActions = GetComponent<PlayerInput>().actions;
        inputActions.Disable();
        inputActions.FindActionMap("Player").Enable();

        // Create LineRenderer
        velocityLine = gameObject.AddComponent<LineRenderer>();
        velocityLine.startWidth = 0.1f;
        velocityLine.endWidth = 0.1f;
        velocityLine.material = new Material(Shader.Find("Sprites/Default"));
        velocityLine.positionCount = 2;

        // Initialize velocity and direction
        playerRigidbody = GetComponent<Rigidbody>();
        currentVelocity = Vector3.zero;
        currentDirection = transform.forward;
    }

    void Update()
    {
        // Only update visuals in Update
        UpdateVelocityLine();
    }

    void OnMove(InputValue value)
    {
        movementInput = value.Get<Vector2>();
    }

    void OnClick(InputValue value)
    {
        Debug.Log("Fire input received");
        if (value.isPressed)
        {
            // Get mouse position in world space
            Vector3 mouseScreenPos = Mouse.current.position.ReadValue();
            Ray ray = Camera.main.ScreenPointToRay(mouseScreenPos);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, layerMask))
            {
                weaponManager.UseWeapon(hit.point);
            }
        }
    }

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
            Vector3 targetDirection = transform.forward;
            
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

    private void UpdateVelocityLine()
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
}