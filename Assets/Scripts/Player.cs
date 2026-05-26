using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class Player : Spaceship
{

    [Header("Player Specific")]
    [SerializeField] private ProjectMSInputAction playerInputActions;
    public Movement movementScript;
    public static Player Instance { get; private set; }

    [Header("Thruster FX")]
    [SerializeField] private float thrusterBaseEmission = 50f; // tune in Inspector

    private bool isThrusting = false;
    protected override void Awake()
    {
        base.Awake();
        playerInputActions = new ProjectMSInputAction();

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (movementScript == null)
        {
            movementScript = GetComponent<Movement>();
            if (movementScript == null)
                movementScript = gameObject.AddComponent<Movement>();
        }

        ApplyUpgrades();
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
        CurrentHP = MaxHP;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.MaxFuel = MaxFuel;
            GameManager.Instance.Fuel = Fuel;
            GameManager.Instance.MaxMissiles = MaxMissiles;
            GameManager.Instance.Missiles = Missiles;
            GameManager.Instance.MaxHp = MaxHP;
            GameManager.Instance.Hp = CurrentHP;
        }
    }

    public void UpdateVariance()
    {
        Fuel = Mathf.Clamp(Fuel, 0f, MaxFuel);
        Missiles = Mathf.Clamp(Missiles, 0, MaxMissiles);
        CurrentHP = Mathf.Clamp(CurrentHP, 0, MaxHP);

        if (GameManager.Instance != null)
        {
            GameManager.Instance.Fuel = Fuel;
            GameManager.Instance.Missiles = Missiles;
            GameManager.Instance.Hp = CurrentHP;
            GameManager.Instance.MaxHp = MaxHP;
        }
    }


    public override void FixedUpdate()
    {
        base.FixedUpdate();

        UpdateVariance();
        RotationOnMove();

        if (Fuel <= 0)
            movementScript.enabled = false;

        UpdateThrusterFX();
    }

    private void UpdateThrusterFX()
    {
        if (activeThruster == null) return;

        bool hasFuel = Fuel > 0;
        bool isMovingNormal = isThrusting && hasFuel;
        bool isCurrentlyBoosting = movementScript != null
                                   && movementScript.IsBoosting  // ? uses IsBoosting property
                                   && hasFuel;

        if (isMovingNormal || isCurrentlyBoosting)
        {
            if (!activeThruster.isEmitting)
                activeThruster.Play();

            var emissionModule = activeThruster.emission;

            // Boost = 4x emission to match the 4x speed in ApplyBoosterThruster()
            emissionModule.rateOverTime = isCurrentlyBoosting
                ? thrusterBaseEmission * 4f
                : thrusterBaseEmission;
        }
        else
        {
            if (activeThruster.isEmitting)
                activeThruster.Stop();
        }
    }

    private void RotationOnMove()
    {
        float rotationSpeed = 450f;
        Vector2 direction = movementScript.smoothedInput;

        if (direction.magnitude > 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle - 90f);

            transform.rotation = Quaternion.RotateTowards(
                transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

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
        isThrusting = true;
    }

    private void OnMovementCanceled(InputAction.CallbackContext context)
    {
        movementScript.MoveHorizontal(0f);
        movementScript.MoveVertical(0f);
        isThrusting = false;
    }


    protected override void LaunchMissile()
    {
        if (GameManager.Instance.currentState != GameManager.GameState.Playing)
            return;

        base.LaunchMissile();
    }
    private void ApplyUpgrades()
    {

        if (UpgradeManager.Instance == null)
            return;

        UpgradeManager.UpgradeProfileData upgrades = UpgradeManager.Instance.LoadUpgrades();

        MaxHP += (upgrades.HealthLevel - 1);
        MaxMovementSpeed += (upgrades.SpeedLevel - 1) * 0.5f;
        MaxFuel += (upgrades.FuelLevel - 1) * (5f + upgrades.FuelLevel);
        MaxMissiles += (upgrades.MissileLevel - 1) * 1;

        Fuel = MaxFuel;
        Missiles = MaxMissiles;
        CurrentHP = MaxHP;

        if (movementScript != null)
            movementScript.moveSpeed = MaxMovementSpeed;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.MaxFuel = MaxFuel;
            GameManager.Instance.MaxMissiles = MaxMissiles;
            GameManager.Instance.MaxHp = MaxHP;
            GameManager.Instance.Fuel = Fuel;
            GameManager.Instance.Missiles = Missiles;
            GameManager.Instance.Hp = CurrentHP;
        }
    }

    protected override void OnDeath()
    {
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Collider2D>().enabled = false;
        movementScript.enabled = false;

        if (activeThruster != null)
            activeThruster.Stop();

        if (ImpactEffect != null)
            Instantiate(ImpactEffect, transform.position, Quaternion.identity);

        StartCoroutine(WaitAndGameOver());
    }

    private IEnumerator WaitAndGameOver()
    {
        yield return new WaitForSeconds(3f);
        GameManager.Instance.GameOver(2);
        Destroy(gameObject);
    }
}