using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class ARSwipe : NetworkBehaviour
{
    private Vector2 startTouchPosition;
    private Vector2 endTouchPosition;
    private bool isSwiping = false;
    private float swipeDistanceThreshold = 400f;

    private InputAction touchPress;
    private InputAction touchPosition;

    public PlayerInputControls playerInputControls;

    public override void OnNetworkSpawn()
    {
        if (GetComponent<NetworkObject>().IsOwner)
        {
            var playerInput = playerInputControls.playerControlsInputAction;

            touchPress = playerInput.PlayerControls.TouchPress;
            touchPosition = playerInput.PlayerControls.TouchPosition;

            touchPress.Enable();
            touchPosition.Enable();
        }
    }

    private void OnDestroy()
    {
        touchPress.Disable();
        touchPosition.Disable();
    }

    private void Update()
    {
        if (!IsOwner || touchPress == null || touchPosition == null)
            return;

        bool isTouching = touchPress.ReadValue<float>() > 0f;
        Vector2 currentTouch = touchPosition.ReadValue<Vector2>();

        if (isTouching && !isSwiping)
        {
            // Start swipe
            isSwiping = true;
            startTouchPosition = currentTouch;
        }
        else if (!isTouching && isSwiping)
        {
            // End swipe
            isSwiping = false;
            endTouchPosition = currentTouch;

            Vector2 swipeVector = endTouchPosition - startTouchPosition;
            if (swipeVector.magnitude > swipeDistanceThreshold)
            {
                HandleSwipe(swipeVector);
            }
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
