using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundDetection : MonoBehaviour
{
    [SerializeField]private PlayerController player;
    public string groundLayer;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer != player.gameObject.layer && collision.gameObject.layer == LayerMask.NameToLayer(groundLayer))
        {
            player.isGrounded = true;
            player.isWallSliding = false;
            player.wasOnWall = false;
            player.currentJumpTime = 0;
            player.hasJumped = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer(groundLayer))
        {
            player.isGrounded = false;
            player.coyoteJumpTimer = 0;
        }
    }
}
