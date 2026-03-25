using TMPro;
using UnityEngine;

public class DebugBelowFuel : MonoBehaviour
{
    private GameManager gameManager;
    [SerializeField] private GameObject belowFuel;
     private void Awake()
    {
    }
    private void Start()
    {
        gameManager = GameManager.Instance;
    }
    private void Update()
    {
        if (gameManager != null && belowFuel != null)
        {
            if(gameManager.Fuel < gameManager.minimumReturnFuel)
            {
                belowFuel.SetActive(true);
            }
            else
            {
                belowFuel.SetActive(false);
            }
        }
    }
}
