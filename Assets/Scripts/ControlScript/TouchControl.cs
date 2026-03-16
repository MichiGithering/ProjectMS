using UnityEngine;
using UnityEngine.InputSystem;

public class TouchInputReader : MonoBehaviour
{
    [SerializeField] Movement movementscript;
    [SerializeField] private float minimumSwipe; //Use for checking Touch button
    [SerializeField] private float maximumSwipe; //Use for checking for canceling swipe

    private ProjectMSInputAction inputActions;
    private Vector2 startTouchPosition;
    private Vector2 endTouchPosition;

    private void Awake()
    {
        inputActions = new ProjectMSInputAction();
    }

    private void OnEnable()
    {
        inputActions.Enable();

        inputActions.TouchControl.PrimaryContact.started += OnTouchStarted;
        inputActions.TouchControl.PrimaryContact.canceled += OnTouchEnded;
    }

    private void OnDisable()
    {
        inputActions.TouchControl.PrimaryContact.started -= OnTouchStarted;
        inputActions.TouchControl.PrimaryContact.canceled -= OnTouchEnded;

        inputActions.Disable();
    }

    private void OnTouchStarted(InputAction.CallbackContext context)
    {
        startTouchPosition = inputActions.TouchControl.PrimaryPosition.ReadValue<Vector2>();
    }

    private void OnTouchEnded(InputAction.CallbackContext context)
    {
        endTouchPosition = inputActions.TouchControl.PrimaryPosition.ReadValue<Vector2>();

        if (Mathf.Abs(startTouchPosition.x - endTouchPosition.x) > minimumSwipe)
        {
            if (startTouchPosition.x < endTouchPosition.x)
            {
            }
            else
            {
            }
        }

        if (Mathf.Abs(startTouchPosition.y - endTouchPosition.y) > 0)
        {
            if (startTouchPosition.y < endTouchPosition.y)
            {
            }
            else
            {
            }
        }
        else
        {
        }
    }
}
