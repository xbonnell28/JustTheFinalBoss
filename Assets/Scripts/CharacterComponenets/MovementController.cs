using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    [Header("Movement Variables")]
    [SerializeField] private float maxGroundMovementSpeed;
    [SerializeField] private float maxAirMovementSpeed;
    [SerializeField] private float lerpAmount;
    [SerializeField] private float groundMovementAcceleration;
    [SerializeField] private float groundMovementDeceleration;
    [SerializeField] private float airMovementAcceleration;
    [SerializeField] private float airMovementDeceleration;

    [Header("Jump Variables")]
    [SerializeField] private float jumpForce;
    [SerializeField] private float wallJumpHorizontalForce;
    [SerializeField] private float wallJumpVerticalForce;

    private Rigidbody2D rb;
    private PlayerInputHandler playerInputHandler;
    private int direction;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerInputHandler = GetComponent<PlayerInputHandler>();
    }
    public void handleMovement(float horizontalInput, bool isGrounded)
    {
        if (horizontalInput > 0)
        {
            transform.rotation = new Quaternion(transform.rotation.x, 0, transform.rotation.x, transform.rotation.w);
            direction = -1;
        }
        else if (horizontalInput < 0)
        {
            transform.rotation = new Quaternion(transform.rotation.x, 180, transform.rotation.x, transform.rotation.w);
            direction = 1;
        }

        float targetSpeed;
        if (isGrounded)
        {
            targetSpeed = horizontalInput * maxGroundMovementSpeed;
        }
        else
        {
            targetSpeed = horizontalInput * maxAirMovementSpeed;
        }

        targetSpeed = Mathf.Lerp(rb.velocity.x, targetSpeed, lerpAmount);

        float accelRate;

        if (isGrounded)
        {
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? groundMovementAcceleration : groundMovementDeceleration;
        }
        else
        {
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? groundMovementAcceleration * airMovementAcceleration : groundMovementDeceleration * airMovementDeceleration;
        }

        // Air acceleration
        //if (hasJumped && Mathf.Abs(rigidbody.velocity.y) < jumpHangTimeThreshold)
        //{
        //    accelRate *= jumpHangAccelerationMult;
        //    targetSpeed *= jumpHangMaxSpeedMult;
        //}

        //// Momentum Conservation
        //if (doConserveMomentum && Mathf.Abs(rigidbody.velocity.x) > Mathf.Abs(targetSpeed) && Mathf.Sign(rigidbody.velocity.x) == Mathf.Sign(targetSpeed) && Mathf.Abs(targetSpeed) > 0.01f && !isGrounded)
        //{
        //    //Prevent any deceleration from happening, or in other words conserve are current momentum
        //    //You could experiment with allowing for the player to slightly increae their speed whilst in this "state"
        //    accelRate = 0;
        //}

        float speedDif = targetSpeed - rb.velocity.x;

        float movement = speedDif * accelRate;

        rb.AddForce(movement * Vector2.right, ForceMode2D.Force);

    }

    public void handleJump(bool jumpInput, bool isGrounded, bool isWallSliding)
    {
        //if (!isGrounded)
        //{
        //    coyoteJumpTimer += Time.deltaTime;
        //}
        if (jumpInput)
        {
            if (isGrounded)
            {
                Jump(Vector2.up * jumpForce);
            }
            else if (!isGrounded && isWallSliding)
            {
                WallJump();
            }
            //    else if (!isGrounded && coyoteJumpTimer < coyoteJumpWindow && !hasJumped)
            //    {
            //        jump(Vector2.up * jumpForce);
            //    }
            //    else if (isWallSliding && !hasJumped)
            //    {
            //        WallJump(-direction);
            //        isWallSliding = false;
            //        wasOnWall = true;
            //    }
            //}
            //else
            //{
            //    jumpBufferCounter -= Time.deltaTime;
            //    if (isGrounded && jumpBufferCounter > 0 && !hasJumped)
            //    {
            //        jump(Vector2.up * jumpForce);
            //    }
        }
    }

    private void Jump(Vector2 jumpVector)
    {
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(jumpVector, ForceMode2D.Impulse);
        playerInputHandler.cachedJump = false;
        //currentJumpTime = 0;
        //hasJumped = true;
        //jumpingPressed = false;
    }

    private void WallJump()
    {
        Vector2 force = new Vector2(wallJumpHorizontalForce, wallJumpVerticalForce);
        force.x *= direction; //apply force in opposite direction of wall

        if (Mathf.Sign(rb.velocity.x) != Mathf.Sign(force.x))
            force.x -= rb.velocity.x;

        if (rb.velocity.y < 0) //checks whether player is falling, if so we subtract the velocity.y (counteracting force of gravity). This ensures the player always reaches our desired jump force or greater
            force.y -= rb.velocity.y;

        //Unlike in the run we want to use the Impulse mode.
        //The default mode will apply are force instantly ignoring masss
        rb.AddForce(force, ForceMode2D.Impulse);
        playerInputHandler.cachedJump = false;
    }
}
