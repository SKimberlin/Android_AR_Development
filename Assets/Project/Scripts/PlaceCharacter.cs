using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlaceCharacter : NetworkBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject placementObject;
    private void Start()
    {
        if (!mainCamera) { mainCamera = Camera.main; }
    }

    private void Update()
    {
        if (PlayerDataManager.Instance != default &&
            PlayerDataManager.Instance.GetHasPlacerPlaced(NetworkManager.Singleton.LocalClientId)) return;
#if UNITY_EDITOR
        if (Mouse.current.leftButton.isPressed)
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;

            TouchToRay(Mouse.current.position.ReadValue());
        }
#endif
#if UNITY_ANDROID

        if (Input.touchCount > 0 && Input.touchCount < 2 &&
            Input.GetTouch(0).phase == UnityEngine.TouchPhase.Began)
        {
            Touch touch = Input.GetTouch(0);

            PointerEventData pointerData = new PointerEventData(EventSystem.current);
            pointerData.position = touch.position;

            List<RaycastResult> results = new List<RaycastResult>();

            EventSystem.current.RaycastAll(pointerData, results);

            if (results.Count > 0)
            {
                return;
            }

            if (EventSystem.current.IsPointerOverGameObject(touch.fingerId)) 
            {
                return;
            }

            TouchToRay(touch.position);
#endif
        }
    }
    void TouchToRay(Vector3 touch)
    {
        Ray ray = mainCamera.ScreenPointToRay(touch);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Quaternion rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
            SpawnPlayerServerRpc(hit.point, rotation, NetworkManager.Singleton.LocalClientId);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void SpawnPlayerServerRpc(Vector3 position, Quaternion rotation, ulong callerId)
    {
        GameObject character = Instantiate(placementObject, position, rotation);

        NetworkObject characterNetworkObject = character.GetComponent<NetworkObject>();

        characterNetworkObject.SpawnWithOwnership(callerId);

        PlayerDataManager.Instance.AddPlacedPlayer(callerId);
    }
}
