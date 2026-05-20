using UnityEngine;
using Unity.Cinemachine;

[RequireComponent(typeof(Rigidbody2D))]
public class Movement : MonoBehaviour
{
    private Rigidbody2D rb;

    [Header("Movement Settings")]
    public float moveSpeed = 100f;
    public float acceleration = 30f;
    public float deceleration = 35f;

    private float baseMoveSpeed;
    private float currentMaxSpeed;

    public bool IsBoosting => currentMaxSpeed > baseMoveSpeed;

    [Header("Input Smoothing")]
    [SerializeField] private float inputSmoothing = 8f;

    [Header("Fuel Handling")]
    private Spaceship spaceship;
    private float RemainFuel;
    private bool HasFuel = true;

    [Header("Cinemachine Zoom Settings")]
    private CinemachineCamera cmCamera;
    public float BaseCameraZoom;
    [SerializeField] private float cameraZoomSpeed = 4f;
    private float targetCameraZoom;

    private Vector2 moveInput;
    public Vector2 smoothedInput;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.gravityScale = 0f;
        }

        cmCamera = FindFirstObjectByType<CinemachineCamera>();
        if (cmCamera != null)
        {
            BaseCameraZoom = cmCamera.Lens.OrthographicSize;
            targetCameraZoom = BaseCameraZoom;
        }
        else
        {
            BaseCameraZoom = 15f;
            targetCameraZoom = BaseCameraZoom;
        }

        EntityConfig entityConfig = GetComponent<Entity>()?._entityConfig;
        if (entityConfig != null)
        {
            moveSpeed = entityConfig.MaxSpeed;
        }

        spaceship = GetComponent<Spaceship>();

        baseMoveSpeed = moveSpeed;
        currentMaxSpeed = moveSpeed;
    }

    private void HandleCameraZoom()
    {
        if (cmCamera == null)
        {
            cmCamera = FindFirstObjectByType<CinemachineCamera>();
            if (cmCamera != null)
            {
                BaseCameraZoom = cmCamera.Lens.OrthographicSize;
                targetCameraZoom = BaseCameraZoom;
            }
            return;
        }

        cmCamera.Lens.OrthographicSize = Mathf.Lerp(cmCamera.Lens.OrthographicSize, targetCameraZoom, cameraZoomSpeed * Time.fixedDeltaTime);
    }

    private void FixedUpdate()
    {
        smoothedInput = Vector2.Lerp(smoothedInput, moveInput, inputSmoothing * Time.fixedDeltaTime);

        HandleCameraZoom();

        if (spaceship != null)
        {
            RemainFuel = spaceship.Fuel;
            HasFuel = RemainFuel > 0;

            if (!HasFuel && currentMaxSpeed > baseMoveSpeed)
            {
                StopBoosterThruster();
            }

            if (HasFuel)
                ApplyMovement();
        }
        else
        {
            ApplyMovement();
        }
    }

    public void MoveHorizontal(float input) => moveInput.x = input;
    public void MoveVertical(float input) => moveInput.y = input;

    public void ApplyMovement()
    {
        Vector2 targetVelocity = smoothedInput.normalized * currentMaxSpeed;
        float speedChange = smoothedInput.magnitude > 0.01f ? acceleration : deceleration;

        float newVelX = Mathf.MoveTowards(rb.linearVelocity.x, targetVelocity.x, speedChange * Time.fixedDeltaTime);
        float newVelY = Mathf.MoveTowards(rb.linearVelocity.y, targetVelocity.y, speedChange * Time.fixedDeltaTime);

        Vector2 newVelocity = new Vector2(newVelX, newVelY);
        if (newVelocity.magnitude > currentMaxSpeed)
            newVelocity = newVelocity.normalized * currentMaxSpeed;

        rb.linearVelocity = newVelocity;
    }

    public void ApplyBoosterThruster()
    {
        if (spaceship != null && spaceship.Fuel >= 20f)
        {
            spaceship.FuelConsumptionRate = 2.2f;
            currentMaxSpeed = baseMoveSpeed * 4;

            targetCameraZoom = BaseCameraZoom * 7f;
        }
    }

    public void StopBoosterThruster()
    {
        if (spaceship != null)
        {
            spaceship.FuelConsumptionRate = 1.0f;
        }

        currentMaxSpeed = baseMoveSpeed;
        targetCameraZoom = BaseCameraZoom;
    }
}