using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundDetection : MonoBehaviour
{
    public string groundTag;

    private PlayerControllerV2 player;

    private void Awake()
    {
        player = GetComponentInParent<PlayerControllerV2>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag != player.gameObject.tag && collision.gameObject.tag == groundTag)
        {
            player.inputHandler.heldJumpTimer = 0;
            player.isGrounded = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == groundTag)
        {
            player.isGrounded = false;
        }
    }
}
