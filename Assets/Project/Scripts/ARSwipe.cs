using System;
using Unity.Netcode;
using UnityEngine;

public class ARSwipe : NetworkBehaviour
{
    public PlayerInputControls playerInputControls; // Assign via Inspector

    private Vector2 startTouchPosition;
    private Vector2 endTouchPosition;
    private float swipeDistanceThreshold = 50f;
    private bool isSwiping = false;

    void Update()
    {
        if (IsOwner) HandleInput();
    }

    private void HandleInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    startTouchPosition = touch.position;
                    isSwiping = true;
                    break;
                case TouchPhase.Moved:
                case TouchPhase.Ended:
                    if (isSwiping)
                    {
                        endTouchPosition = touch.position;
                        Vector2 swipeVector = endTouchPosition - startTouchPosition;
                        if (swipeVector.magnitude > swipeDistanceThreshold)
                        {
                            HandleSwipe(swipeVector);
                            isSwiping = false;
                        }
                    }
                    break;
                case TouchPhase.Canceled:
                    isSwiping = false;
                    break;
            }
        }
    }

    private void HandleSwipe(Vector2 swipeVector)
    {
        Debug.Log("Swiped");
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
