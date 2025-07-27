using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ARSwipe : MonoBehaviour
{
    private Vector2 startTouchPosition;
    private Vector2 endTouchPosition;
    private float swipeDistanceThreshold = 50f;
    private bool isSwiping = false;

    // Update is called once per frame
    void Update()
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
                case TouchPhase.Ended:
                    if (isSwiping)
                    {
                        endTouchPosition = touch.position;
                        Vector2 swipeVector = endTouchPosition - startTouchPosition;
                        if (swipeVector.magnitude > swipeDistanceThreshold)
                        {
                            HandleSwipe(swipeVector);
                        }
                        isSwiping = false;
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
        // Example: If you want to move an AR object based on the swipe
        // transform.position += new Vector3(swipeVector.x, 0, swipeVector.y) * Time.deltaTime;

        Vector2 swipeDirection = endTouchPosition - startTouchPosition;
        swipeDirection.Normalize();

        string swipe;

        if (Mathf.Abs(swipeDirection.x) > Mathf.Abs(swipeDirection.y))
        {
            swipe = swipeDirection.x > 0 ? "Right" : "Left";
        }
        else
        {
            swipe = swipeDirection.y > 0 ? "Up" : "Down";
        }

        switch (swipe)
        {
            case "Right":
                SwipeRight();
                break;
            case "Left":
                SwipeLeft();
                break;
            case "Up":
                SwipeUp();
                break;
            case "Down":
                SwipeDown();
                break;
            default:
                Debug.Log("Swipe direction not recognized.");
                break;
        }
    }

    void SwipeRight()
    {
        Transform arObject = GameObject.Find("ARObject").transform;
        arObject.position += new Vector3(1, 0, 0);
    }

    void SwipeLeft()
    {
        Transform arObject = GameObject.Find("ARObject").transform;
        arObject.position += new Vector3(-1, 0, 0);
    }

    void SwipeUp()
    {
        Transform arObject = GameObject.Find("ARObject").transform;
        arObject.position += new Vector3(0, 0, 1);
    }

    void SwipeDown()
    {
        Transform arObject = GameObject.Find("ARObject").transform;
        arObject.position += new Vector3(0, 0, -1);
    }
}
