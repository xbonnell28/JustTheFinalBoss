using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityController : MonoBehaviour
{

    [SerializeField] private int wallSlideSpeed;
    [SerializeField] private int fallingGravityMultiplier;
    [SerializeField] private int lowJumpGravityMultiplier;

    private Rigidbody2D rb;
    private PlayerInputHandler inputHandler;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        inputHandler = GetComponent<PlayerInputHandler>();
    }

    public void handleGravity(bool isWallSliding)
    {
        // Slow fall for wall sliding
        if (isWallSliding)
        {
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlideSpeed, float.MaxValue));
        }

        // Basic gravity to neutral player
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallingGravityMultiplier - 1) * Time.deltaTime;
        }
        // Higher gravity applied to short hop
        else if (ShortHopCheck())
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpGravityMultiplier - 1) * Time.deltaTime;
        }
    }

    private bool ShortHopCheck()
    {
        return rb.velocity.y > 0 && (inputHandler.jumpButtonUp || !inputHandler.withinHeldJumpTime());
    }

}
