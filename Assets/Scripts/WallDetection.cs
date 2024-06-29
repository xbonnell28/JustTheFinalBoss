using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallDetection : MonoBehaviour
{
    public string wallLayer;

    private PlayerControllerV2 player;

    private void Awake()
    {
        player = GetComponentInParent<PlayerControllerV2>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == wallLayer)
        {
            player.inputHandler.heldJumpTimer = 0;
            player.isWallSliding = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == wallLayer)
        {
            player.isWallSliding = false;
        }
    }
}
