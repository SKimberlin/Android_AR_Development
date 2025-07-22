using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARFireObject : MonoBehaviour
{
	[SerializeField] private XROrigin xrOrigin;
	[SerializeField] private GameObject[] prefabs;

	bool isFiring = false;

	void Start()
	{
		// Get the XROrigin component if it's not already assigned
		xrOrigin ??= GetComponent<XROrigin>();
	}

	void Update()
	{
		// Exit early if ARRaycastManager is not assigned
		if (xrOrigin == null) return;

		// Handle touch input (on phones/tablets)
		if (Touchscreen.current != null &&
			Touchscreen.current.touches.Count > 0 &&
			Touchscreen.current.touches[0].phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Began &&
			!isFiring)
		{
			isFiring = true;

			Instantiate(prefabs[Random.Range(0, prefabs.Length)], xrOrigin.Camera.transform.position, xrOrigin.Camera.transform.rotation);
			print("fire mouse");
			// Wait briefly before allowing another placement
			StartCoroutine(SetPlacingToFalseWithDelay());
		}
		// Handle mouse input (for desktop testing)
		else if (Mouse.current != null &&
				 Mouse.current.leftButton.wasPressedThisFrame &&
				 !isFiring)
		{
			isFiring = true;

			Instantiate(prefabs[Random.Range(0, prefabs.Length)], xrOrigin.Camera.transform.position, xrOrigin.Camera.transform.rotation);
			print("fire mouse");
			// Wait briefly before allowing another placement
			StartCoroutine(SetPlacingToFalseWithDelay());

		}
	}

	//void FireObject(Vector2 position)
	//{
	//	// Wait briefly before allowing another placement
	//	StartCoroutine(SetPlacingToFalseWithDelay());
	//}

	IEnumerator SetPlacingToFalseWithDelay()
	{
		// Wait for a short delay
		yield return new WaitForSeconds(0.5f);
		print("reset");

		// Allow placing again
		isFiring = false;
	}

	Vector2 GetScreenCenter()
	{
		Vector2 center = Vector2.zero;

		// Set X to half the screen width
		center.x = Screen.width / 2;
		// Set Y to half the screen height
		center.y = Screen.height / 2;

		return center;
	}
}
