using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallDetection : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    public string wallLayer;

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer(wallLayer))
        {
            player.isWallSliding = false;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer(wallLayer))
        {
            player.isWallSliding = true;
        }
    }
}
