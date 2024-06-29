using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerV2 : MonoBehaviour
{
    [HideInInspector] public bool isGrounded;
    [HideInInspector] public bool isWallSliding;

    private MovementController movementController;
    private GravityController gravityController;
    public PlayerInputHandler inputHandler; // TODO: don't like that this is public

    private void Awake()
    {
        movementController = GetComponent<MovementController>();
        gravityController = GetComponent<GravityController>();
        inputHandler = GetComponent<PlayerInputHandler>();
    }

    void FixedUpdate()
    {
        movementController.handleMovement(inputHandler.horizontalInput, isGrounded);
        movementController.handleJump(inputHandler.cachedJump, isGrounded, isWallSliding);
        gravityController.handleGravity(isWallSliding);
    }
}
