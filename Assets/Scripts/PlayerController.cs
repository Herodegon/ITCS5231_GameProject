using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

public class PlayerController : MonoBehaviour
{
    [Header("Player Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float acceleration = 2f;

    [Header("Debug Settings")]
    [SerializeField] private bool showVelocity = false;
    [SerializeField] private float velocityDebugScale = 1f;

    private Vector3 currentVelocity;
    private Vector3 currentDirection;
    private LineRenderer velocityLine;

    private InputActionAsset inputActions;
    private Vector2 movementInput;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
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
        currentVelocity = Vector3.zero;
        currentDirection = Vector3.forward;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMovement();
        UpdateDirection();
        UpdateVelocityLine();
    }

    void OnMove(InputValue value)
    {
        // Movement input should be relative to camera orientation, disregarding vertical rotation
        movementInput = value.Get<Vector2>();
    }

    private void UpdateMovement()
    {
        // Apply camera-relative movement directly in UpdateMovement
        Vector3 inputDirection = new Vector3(movementInput.x, 0f, movementInput.y);
        Vector3 cameraRelativeMovement = Camera.main.transform.TransformDirection(inputDirection);
        cameraRelativeMovement.y = 0;
        
        Vector3 movement = cameraRelativeMovement.normalized;
        currentVelocity = Vector3.Lerp(currentVelocity, movement * moveSpeed, acceleration * Time.deltaTime);
        transform.Translate(currentVelocity * Time.deltaTime, Space.World);
    }

    private void UpdateDirection()
    {
        if (currentVelocity.magnitude > 0.1f)
        {
            currentDirection = currentVelocity.normalized;
            Quaternion targetRotation = Quaternion.LookRotation(currentDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * acceleration);
        }
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
