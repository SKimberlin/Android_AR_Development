using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARPlaceObject : MonoBehaviour
{
    [SerializeField] private ARRaycastManager raycastManager;
    [SerializeField] private GameObject[] prefabGameObjects;
    bool isPlacing = false;

    void Start()
    {
        // Get the ARRaycastManager component if it's not already assigned
        raycastManager ??= GetComponent<ARRaycastManager>();
    }

    void Update()
    {
        // Exit early if ARRaycastManager is not assigned
        if (raycastManager == null) return;

        // Handle touch input (on phones/tablets)
        if (Touchscreen.current != null &&
            Touchscreen.current.touches.Count > 0 &&
            Touchscreen.current.touches[0].phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Began &&
            !isPlacing)
        {
            isPlacing = true;

            // Get touch position on screen
            Vector2 touchPos = Touchscreen.current.touches[0].position.ReadValue();

            // Place the object at touch position
            PlaceObject(touchPos);
        }
        // Handle mouse input (for desktop testing)
        else if (Mouse.current != null &&
                 Mouse.current.leftButton.wasPressedThisFrame &&
                 !isPlacing)
        {
            isPlacing = true;

            // Get mouse position on screen
            Vector2 mousePos = Mouse.current.position.ReadValue();

            // Place the object at mouse click
            PlaceObject(mousePos);
        }
    }


    void PlaceObject(Vector2 position)
    {
        // Create a list to store raycast hit results
        var rayHits = new List<ARRaycastHit>();

        // Raycast from screen position into AR world using  trackable type
        raycastManager.Raycast(position, rayHits, TrackableType.PlaneWithinPolygon);

        // If we hit a valid surface
        if (rayHits.Count > 0)
        {
            // Get the position and rotation of the first hit
            Vector3 hitPosePosition = rayHits[0].pose.position;
            Quaternion hitPoseRotation = rayHits[0].pose.rotation;

            // Instantiate the prefab at the hit location
            Instantiate(prefabGameObjects[Random.Range(0, prefabGameObjects.Length)], hitPosePosition, hitPoseRotation);
        }

        // Wait briefly before allowing another placement
        StartCoroutine(SetPlacingToFalseWithDelay());
    }

    IEnumerator SetPlacingToFalseWithDelay()
    {
        // Wait for a short delay
        yield return new WaitForSeconds(0.25f);

        // Allow placing again
        isPlacing = false;
    }
}