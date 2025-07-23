using System;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(PlayerInputControls))]
public class PlayerMovement : NetworkBehaviour
{
    private PlayerInputControls playerInputControls;
    [SerializeField]
    private CharacterController characterController;
    [SerializeField]
    private float speed = 1.5f;
    [SerializeField]
    private float move_threshhold = 0.01f;
    public override void OnNetworkSpawn()
    {
        if (GetComponent<NetworkObject>().IsOwner)
        {
            playerInputControls = GetComponent<PlayerInputControls>();
            playerInputControls.OnMoveInput += PlayerInputControlsOnMoveInput;
        }
    }

    private void PlayerInputControlsOnMoveInput(Vector3 inputMovement)
    {
        if (inputMovement.magnitude < move_threshhold) return;

        characterController.Move(inputMovement * speed);
    }

}
