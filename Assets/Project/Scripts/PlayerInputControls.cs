using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputControls : NetworkBehaviour
{
    private PlayerControlsInputAction playerControlsInputAction;
    Vector3 movementVector;

    public event Action<Vector3> OnMoveInput;
    public event Action OnMoveActionCanceled;
    public override void OnNetworkSpawn()
    {
        if (GetComponent<NetworkObject>().IsOwner)
        {
            playerControlsInputAction = new PlayerControlsInputAction();
            playerControlsInputAction.Enable();

            playerControlsInputAction.PlayerControls.Move.performed += OnMovePerformed;
            playerControlsInputAction.PlayerControls.Move.canceled += OnMoveCanceled;
        }
    }

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        Vector2 movement = context.ReadValue<Vector2>();
        movementVector = new Vector3(movement.x, 0, movement.y);
    }
    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        movementVector = Vector3.zero;
        OnMoveActionCanceled?.Invoke();
    }

    private void Update()
    {
        if (movementVector != Vector3.zero)
        {
            OnMoveInput?.Invoke(movementVector);
        }
    }

    public override void OnNetworkDespawn()
    {
        if (GetComponent<NetworkObject>().IsOwner)
        {
            playerControlsInputAction.PlayerControls.Move.performed -= OnMovePerformed;
            playerControlsInputAction.PlayerControls.Move.canceled -= OnMoveCanceled;
        }
    }
}
