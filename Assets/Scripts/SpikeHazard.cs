using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeHazard : MonoBehaviour
{
    public int damage;
    public float horizontalKnockbackForce;
    public float verticalKnockbackForce;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
            playerController.applyKnockback(new Vector2(-playerController.direction * horizontalKnockbackForce, verticalKnockbackForce));
            playerController.applyDamage(damage);
        }
    }

    private int getPlayerDirection(PlayerController playerController)
    {
        float enemyX = this.transform.position.x;
        float playerX = playerController.transform.position.x;

        if (enemyX < playerX)
        {
            return 1;
        }
        else
        {
            return -1;
        }
    }
}
