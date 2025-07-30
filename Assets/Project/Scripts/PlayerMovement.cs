using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(PlayerInputControls))]
public class PlayerMovement : NetworkBehaviour
{
    private PlayerInputControls playerInputControls;
    [SerializeField]
    private Rigidbody rb;
    [SerializeField]
    private float speed = 0.05f;
    [SerializeField]
    private float move_threshhold = 0.01f;
    [SerializeField]
    private float lookAtPointDelta = 2f;
    private GameObject lookAtPoint;
    private Camera playerCamera;
    public override void OnNetworkSpawn()
    {
        if (GetComponent<NetworkObject>().IsOwner)
        {
            playerCamera = Camera.main;

            lookAtPoint = new GameObject();

            lookAtPoint.transform.position = transform.position;
            lookAtPoint.transform.rotation = transform.rotation;

            playerInputControls = GetComponent<PlayerInputControls>();
            playerInputControls.OnMoveInput += PlayerInputControlsOnMoveInput;

            rb.WakeUp();
        }
    }

    private void PlayerInputControlsOnMoveInput(Vector3 inputMovement)
    {
        if (inputMovement.magnitude < move_threshhold) return;

        Vector3 cameraForward = playerCamera.transform.forward;
        Vector3 cameraRight = playerCamera.transform.right;

        cameraForward.y = 0;
        cameraRight.y = 0;
        cameraForward.Normalize();
        cameraRight.Normalize();

        Vector3 moveDirection = (cameraForward * inputMovement.z) + (cameraRight * inputMovement.x);

        Debug.Log("Vector: " + inputMovement);

        PlayerLookInMovementDirectionServerRpc(moveDirection);
        MoveServerRpc(moveDirection * speed);
    }

    [ServerRpc(RequireOwnership = false)]
    void PlayerLookInMovementDirectionServerRpc(Vector3 inputVector)
    {
        Vector3 pointToLookAt = transform.position + (inputVector.normalized * lookAtPointDelta);

        lookAtPoint.transform.position = pointToLookAt;

        transform.LookAt(lookAtPoint.transform);
    }

    [ServerRpc(RequireOwnership = false)]
    void MoveServerRpc(Vector3 movement)
    {

        transform.position += movement;
    }

    public override void OnNetworkDespawn()
    {
        if (GetComponent<NetworkObject>().IsOwner)
        {
            playerInputControls.OnMoveInput -= PlayerInputControlsOnMoveInput;
        }
    }

}
