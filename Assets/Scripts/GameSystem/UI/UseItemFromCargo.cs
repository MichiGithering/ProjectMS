using UnityEngine;
using UnityEngine.UI;

public class UseItemFromCargo : MonoBehaviour
{

    [SerializeField] private Image itemIcon;
    public bool allowToggle = false;
    private Image buttonImage;
    private bool isUsed = false;

    private void Start()
    {
        buttonImage = GetComponent<Image>();
        if(!allowToggle)
        {
            buttonImage.color = Color.red;
        }
    }

    public void ToggleButton()
    {
        if(allowToggle)
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
}