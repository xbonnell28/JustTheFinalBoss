using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public int damage;
    public float horizontalKnockback;
    public float verticalKnockback;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
            int direction = getPlayerDirection(playerController);
            playerController.applyKnockback(new Vector2(direction * horizontalKnockback, verticalKnockback));
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
