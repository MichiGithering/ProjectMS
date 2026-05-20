using UnityEngine;
using UnityEngine.UI;

public class UseItemFromCargo : MonoBehaviour
{
    [SerializeField] private Image itemIcon;
    private Image buttonImage;
    private bool isUsed = false;

    private void Awake()
    {
        buttonImage = GetComponent<Image>();
    }

    public void ToggleButton()
    {
        isUsed = !isUsed;

        if (isUsed)
        {
            buttonImage.color = Color.yellow;
            itemIcon.color = Color.green;
        }
        else
        {
            buttonImage.color = Color.white;
            itemIcon.color = Color.white;
        }
    }
}