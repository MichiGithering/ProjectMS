using System.Collections;
using UnityEngine;

public class Objects : Identity
{
    // -------------------------------------------------------------------------
    // Components
    // -------------------------------------------------------------------------

    [Header("Object")]
    public Rigidbody2D _rb { get; protected set; }
    public Animator _animator { get; protected set; }
    public Collider2D _collider { get; protected set; }

    [Header("Variable")]
    public int MaxHP;
    public float MaxMovementSpeed;

    [Header("Mirage")]
    public bool Mirage = false;

    // -------------------------------------------------------------------------
    // HitFlash Settings
    // -------------------------------------------------------------------------

    [Header("Hit Flash")]
    [SerializeField] private float flashDuration = 0.3f;
    [SerializeField] private float shakeStrength = 0.15f;
    [SerializeField] private float scalePulseSize = 1.2f;

    [SerializeField] private Color flashColor = Color.white;

    private SpriteRenderer _spriteRenderer;
    protected Color _originalColor;
    
    private Coroutine _activeFlash;
    private bool _isFlashing = false;
    private Vector3 _baseScale;
    private Vector3 _basePosition;

    // -------------------------------------------------------------------------
    // Initialization
    // -------------------------------------------------------------------------

    protected virtual void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _collider = GetComponent<Collider2D>();

        if (_rb == null)
            _rb = gameObject.AddComponent<Rigidbody2D>();

        if (_collider == null)
            _collider = gameObject.AddComponent<CircleCollider2D>();

        _rb.gravityScale = 0;
        _rb.freezeRotation = true;

        // Cache SpriteRenderer for HitFlash
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (_spriteRenderer != null)
        {
            _originalColor = _spriteRenderer.color;
        }

    }

    public override void Start()
    {
        base.Start();
    }

    // -------------------------------------------------------------------------
    // Trigger
    // -------------------------------------------------------------------------

    public virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (Mirage)
        {
            if (collision.CompareTag("Player") || collision.CompareTag("Missile"))
            {
                StartCoroutine(FadeOutAndDestroy(1f));
                return;
            }
        }
    }
    public void DestroyWithFade(float fadeDuration = 1f)
    {
        if (_collider != null)
            _collider.enabled = false;

        StartCoroutine(FadeOutAndDestroy(fadeDuration));
    }
    private IEnumerator FadeOutAndDestroy(float fadeDuration)
    {
        // 1. Instantly turn off the collider so it can't be hit twice!
        if (_collider != null)
        {
            _collider.enabled = false;
        }

        // 2. Set up our colors for the Lerp
        if (_spriteRenderer != null)
        {
            Color startColor = _spriteRenderer.color;

            // Create the exact same color, but set the Alpha (transparency) to 0
            Color transparentColor = new Color(startColor.r, startColor.g, startColor.b, 0f);

            float elapsed = 0f;

            // 3. Run the fade loop
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / fadeDuration;

                // Blend from solid to invisible
                _spriteRenderer.color = Color.Lerp(startColor, transparentColor, t);

                yield return null;
            }
        }
        else
        {
            // Safety fallback just in case the object has no sprite renderer
            yield return new WaitForSeconds(fadeDuration);
        }

        // 4. Finally, wipe it from the game's memory once it is completely invisible
        Destroy(gameObject);
    }

    // -------------------------------------------------------------------------
    // HitFlash — White flash + position shake + scale pulse
    // -------------------------------------------------------------------------

    /// <summary>
    /// Call this on any Objects subclass to trigger the hit feedback effect.
    /// Automatically called by Entity.TakeDamage().
    /// Can also be called manually before Destroy() for a death flash.
    /// </summary>
    public void HitFlash()
    {
        // 1. Only capture the original state if we aren't ALREADY flashing!
        // This stops the position from drifting and captures the correct scale.
        if (!_isFlashing)
        {
            _baseScale = transform.localScale;
            _basePosition = transform.localPosition;
            _isFlashing = true;
        }

        // If a flash is already running, stop it and restart cleanly
        if (_activeFlash != null)
            StopCoroutine(_activeFlash);

        _activeFlash = StartCoroutine(HitFlashRoutine());
    }

    private IEnumerator HitFlashRoutine()
    {
        // --- WHITE FLASH ---
        if (_spriteRenderer != null)
            _spriteRenderer.color = flashColor;

        float elapsed = 0f;

        while (elapsed < flashDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / flashDuration;

            // Scale pulse
            float scaleCurve = Mathf.Sin(t * Mathf.PI);
            float scaleValue = Mathf.Lerp(1f, scalePulseSize, scaleCurve);
            transform.localScale = _baseScale * scaleValue; // Uses the safe _baseScale

            // Position shake
            float shakeAmount = shakeStrength * (1f - t);
            Vector3 shakeOffset = new Vector3(
                Random.Range(-1f, 1f) * shakeAmount,
                Random.Range(-1f, 1f) * shakeAmount,
                0f
            );
            transform.localPosition = _basePosition + shakeOffset; // Uses the safe _basePosition

            yield return null;
        }

        // --- RESTORE ---
        transform.localPosition = _basePosition;
        transform.localScale = _baseScale;

        if (_spriteRenderer != null)
            _spriteRenderer.color = _originalColor;

        // Unlock the flasher so it can capture new positions/scales next time
        _isFlashing = false;
        _activeFlash = null;
    }
}