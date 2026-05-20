using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class UIManager : MonoBehaviour
{
    [SerializeField] public Image missileButton;
    [SerializeField] public TextMeshProUGUI missileCountText;
    [SerializeField] public Image evacButton;

    public static UIManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (missileButton == null)
        {
            Debug.LogError("Missile Button Image is not assigned in the inspector.");
        }
        if (evacButton == null)
        {
            Debug.LogError("Evac Button Image is not assigned in the inspector.");
        }
    }

    private void Start()
    {
        missileButton.gameObject.SetActive(true);
        evacButton.gameObject.SetActive(false);
    }

    public void EnterEvacZone()
    {
        if (GameManager.Instance != null)
        {
            missileButton.gameObject.SetActive(false);
            evacButton.gameObject.SetActive(true);
        }
    }

    public void ExitEvacZone()
    {
        if (GameManager.Instance != null)
        {
            missileButton.gameObject.SetActive(true);
            evacButton.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (GameManager.Instance != null)
        {
            // --- MISSILE LOGIC ---
            if (GameManager.Instance.Missiles > 0)
            {
                missileCountText.gameObject.SetActive(true);
                missileCountText.text = GameManager.Instance.Missiles.ToString();

                missileButton.color = Color.white;
            }
            else
            {
                missileCountText.gameObject.SetActive(false);

                missileButton.color = Color.red;
            }

            // --- EVAC LOGIC ---
            if (GameManager.Instance.hasGivenFuelWarning)
            {
                evacButton.color = Color.red;
            }
            else
            {
                evacButton.color = Color.white;
            }
        }
    }
}