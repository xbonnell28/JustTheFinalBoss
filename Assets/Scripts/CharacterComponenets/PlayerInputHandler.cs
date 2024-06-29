using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    [HideInInspector] public float verticalInput;
    [HideInInspector] public float horizontalInput;
    [HideInInspector] public bool cachedJump;
    [HideInInspector] public bool jumpButtonUp;

    [HideInInspector] public float heldJumpTimer;
    public float maxHeldJumpTimer;
    [HideInInspector] public float bufferJumpTimer;
    public float maxBufferJumpTimer;

    void Update()
    {
        verticalInput = Input.GetAxisRaw("Vertical");
        horizontalInput = Input.GetAxisRaw("Horizontal");
        //jumpInput = Input.GetButtonDown("Jump");
        if (Input.GetButtonDown("Jump"))
        {
            cachedJump = true;
            bufferJumpTimer = 0;
            jumpButtonUp = false;
        }
        else if (Input.GetButtonUp("Jump"))
        {
            jumpButtonUp = true;
            cachedJump = false;
        }

        if (Input.GetButton("Jump"))
        {
            heldJumpTimer += Time.deltaTime;
        }

        if (bufferJumpTimer < maxBufferJumpTimer)
        {
            bufferJumpTimer += Time.deltaTime;
        }
        else
        {
            cachedJump = false;
        }
    }

    public bool withinHeldJumpTime()
    {
        return heldJumpTimer < maxHeldJumpTimer;
    }
}