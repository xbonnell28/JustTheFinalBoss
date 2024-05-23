using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitcheck : MonoBehaviour
{
    public string hazardLayer, enemyLayer;
    [SerializeField] private PlayerController playerController;
    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer(hazardLayer))
        {
            playerController.pogoKnockback();
        }
        else if(collision.gameObject.layer == LayerMask.NameToLayer(enemyLayer))
        {
            EnemyAI enemyAI = collision.gameObject.GetComponent<EnemyAI>();
            Debug.Log("attacking enemy: " + enemyAI);
            enemyAI.applyDamage(playerController.damage);
            playerController.pogoKnockback();
        }
    }
}
