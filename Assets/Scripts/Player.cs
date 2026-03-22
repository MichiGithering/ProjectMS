using JetBrains.Annotations;
using System.Xml.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class Player : Spaceship
{
    [Header("Player Specific")]
    [SerializeField] private ProjectMSInputAction playerInputActions;
    public Movement movementScript;


    // Initialization
    protected override void Awake()
    {
        base.Awake();
        playerInputActions = new ProjectMSInputAction();

        if (movementScript == null)
        {
            movementScript = GetComponent<Movement>();
            if (movementScript == null)
            {
                movementScript = gameObject.AddComponent<Movement>();
            }
        }

        InitializedVariance();
    }
    public override void Start()
    {
        base.Start();
    }
    public override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
    }

    private void InitializedVariance()
    {
        Fuel = MaxFuel;
        Missiles = MaxMissiles;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.MaxFuel = MaxFuel;
            GameManager.Instance.Fuel = Fuel;  
            GameManager.Instance.MaxMissiles = MaxMissiles;
            GameManager.Instance.Missiles = Missiles;
        }
    }
    public void UpdateVariance()
    {
        Fuel = Mathf.Clamp(Fuel, 0f, MaxFuel);
        Missiles = Mathf.Clamp(Missiles, 0, MaxMissiles);

        if (GameManager.Instance != null)
        {
            GameManager.Instance.Fuel = Fuel;
            GameManager.Instance.Missiles = Missiles;

            GameStatsUpdate.Instance.TextUpdate();
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        UpdateVariance();
        RotationOnMove();

        if (Fuel < 0)
        {
            movementScript.enabled = false;
        }
    }

    private void RotationOnMove()
    {
        // Rotation Handling
        float rotationSpeed = 450f;

        Vector2 velocity = _rb.linearVelocity;

        if (velocity.magnitude > 0.1f)
        {
            // 1. Calculate the Target Rotation (where we WANT to face)
            float targetAngle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle - 90f);

            // 2. Smoothly rotate toward that target
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }

    // Input Handling
    private void OnEnable()
    {
        playerInputActions.Enable();

        playerInputActions.PlayerControl.Movement.performed += OnMovementPerformed;
        playerInputActions.PlayerControl.Movement.canceled += OnMovementCanceled;

        playerInputActions.PlayerControl.LaunchMissile.performed += context => LaunchMissile();
    }
    private void OnDisable()
    {
        playerInputActions.PlayerControl.Movement.performed -= OnMovementPerformed;
        playerInputActions.PlayerControl.Movement.canceled -= OnMovementCanceled;

        playerInputActions.Disable();
    }
    private void OnMovementPerformed(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        movementScript.MoveHorizontal(input.x);
        movementScript.MoveVertical(input.y);
    }
    private void OnMovementCanceled(InputAction.CallbackContext context)
    {
        movementScript.MoveHorizontal(0f);
        movementScript.MoveVertical(0f);
    }
    protected override void LaunchMissile()
    {
        Debug.Log("Player is launching a missile!");
        base.LaunchMissile();
    }
}

