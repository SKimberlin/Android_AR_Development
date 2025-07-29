using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class ARSwipe : NetworkBehaviour
{
    private Vector2 startTouchPosition;
    private Vector2 endTouchPosition;
    private bool isSwiping = false;
    private float swipeDistanceThreshold = 50f;

    private InputAction touchPress;
    private InputAction touchPosition;

    public PlayerInputControls playerInputControls;

    private void Start()
    {
        var playerInput = playerInputControls.playerControlsInputAction;

        touchPress = playerInput.PlayerControls.TouchPress;
        touchPosition = playerInput.PlayerControls.TouchPosition;

        touchPress.started += OnTouchStarted;
        touchPress.canceled += OnTouchEnded;

        touchPress.Enable();
        touchPosition.Enable();
    }

    private void OnDestroy()
    {
        touchPress.started -= OnTouchStarted;
        touchPress.canceled -= OnTouchEnded;

        touchPress.Disable();
        touchPosition.Disable();
    }

    private void OnTouchStarted(InputAction.CallbackContext ctx)
    {
        if (!IsOwner) return;

        startTouchPosition = touchPosition.ReadValue<Vector2>();
        isSwiping = true;
    }

    private void OnTouchEnded(InputAction.CallbackContext ctx)
    {
        if (!IsOwner || !isSwiping) return;

        endTouchPosition = touchPosition.ReadValue<Vector2>();
        isSwiping = false;

        Vector2 swipeVector = endTouchPosition - startTouchPosition;
        if (swipeVector.magnitude > swipeDistanceThreshold)
        {
            HandleSwipe(swipeVector);
        }
    }

    private void HandleSwipe(Vector2 swipeVector)
    {
        Debug.Log($"Swiped: {swipeVector}");
        swipeVector.Normalize();

        if (Mathf.Abs(swipeVector.x) > Mathf.Abs(swipeVector.y))
        {
            if (swipeVector.x > 0)
                playerInputControls?.TriggerSwipeAttack("Right");
            else
                playerInputControls?.TriggerSwipeAttack("Left");
        }
        else
        {
            if (swipeVector.y > 0)
                playerInputControls?.TriggerSwipeAttack("Up");
            else
                playerInputControls?.TriggerSwipeAttack("Down");
        }
    }
}
