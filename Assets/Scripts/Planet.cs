using UnityEngine;
using System.Collections;

public class Planet : Objects
{
    [Header("Planet")]
    [SerializeField] public RewardConfig _rewardConfig;
    private SpriteRenderer spriteRenderer;
    [SerializeField] public bool randomizeColor = true;

    [Header("Minimap")]
    [SerializeField] private Sprite minimapIconSprite;
    [SerializeField] private Material minimapIconMaterial;

    [Header("Reward")]
    public bool HasReward = true;
    private float ReFuel;
    private int ReMissile;

    // Track the minimap renderer so we can modify it when the planet is harvested!
    private SpriteRenderer minimapSpriteRenderer;

    protected override void Awake()
    {
        base.Awake();

        if (_rewardConfig == null)
        {
            Debug.LogWarning($"Planet {gameObject.name} is missing a RewardConfig!");
            _rewardConfig = ScriptableObject.CreateInstance<RewardConfig>();
        }

        if (!Mirage)
        {
            HasReward = true;
            ReFuel = _rewardConfig.ReFuel;
            ReMissile = _rewardConfig.ReMissile;
        }
        else
        {
            HasReward = false;
        }
    }

    public override void Start()
    {
        base.Start();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null && randomizeColor)
        {
            Color randomColor = Random.ColorHSV(0f, 1f, 0.5f, 1f, 0.5f, 1f);
            spriteRenderer.color = randomColor;
            _originalColor = randomColor;
        }

        SetUpMinimapIcon("MinimapIcon");
    }

    private void SetUpMinimapIcon(string minimapIconName)
    {
        GameObject minimapIcon = new GameObject(minimapIconName);
        minimapIcon.transform.SetParent(transform);

        // Center the icon on the parent
        minimapIcon.transform.localPosition = Vector3.zero;

        // CHANGED: Setting this to pure 1x1x1 means it will natively inherit 
        // the parent's exact scale multiplier automatically!
        minimapIcon.transform.localScale = Vector3.one;

        minimapSpriteRenderer = minimapIcon.AddComponent<SpriteRenderer>();
        minimapSpriteRenderer.color = Color.red;
        minimapSpriteRenderer.material = minimapIconMaterial != null ? minimapIconMaterial : spriteRenderer.material;
        if (minimapIconSprite != null)
        {
            minimapSpriteRenderer.sprite = minimapIconSprite;
        }
        else
        {
            Debug.LogError($"[Minimap Error] {name}: 'minimapIconSprite' is not assigned in the Inspector slot!");
        }


        minimapIcon.layer = LayerMask.NameToLayer("MinimapIcon");
        minimapSpriteRenderer.sortingLayerName = "MinimapIcon";
    }

    public override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
    }

    public void GetReward(Player playerScript)
    {
        if (HasReward && !Mirage)
        {
            if (_rewardConfig.chanceToBlank > 0f)
            {
                float roll = Random.Range(0f, 1f);
                if (roll < _rewardConfig.chanceToBlank)
                {
                    ReFuel = 0f;
                    ReMissile = 0;
                }
            }
            playerScript.Fuel += ReFuel;
            playerScript.Missiles += ReMissile;

            GetComponent<Collider2D>().enabled = false;
            HasReward = false;

            Debug.Log($"Collected {ReFuel} Fuel and {ReMissile} Missiles from Planet!");

            Color emptyColor = new Color(0.5f, 0.5f, 0.5f, 1f);
            _originalColor = emptyColor;

            if (minimapSpriteRenderer != null)
            {
                minimapSpriteRenderer.color = new Color(0.25f, 0.25f, 0.25f, 1f);
            }

            StartCoroutine(FadeToGrey(emptyColor, 1f));

            GameManager.Instance.AddResearchPoints(_rewardConfig.RewardPoints);
        }
    }

    private IEnumerator FadeToGrey(Color targetColor, float fadeDuration)
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        if (sr != null)
        {
            Color startColor = sr.color;
            float elapsed = 0f;

            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / fadeDuration;

                sr.color = Color.Lerp(startColor, targetColor, t);

                yield return null;
            }

            sr.color = targetColor;
        }
    }
}