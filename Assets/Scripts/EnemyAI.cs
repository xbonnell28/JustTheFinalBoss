using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;

    [Header("Movement")]
    [SerializeField] private float movementSpeed = 3.5f;
    [SerializeField] private float airMovementSpeed = 0.1f;
    [SerializeField] private float maxAirMovementSpeed = 5f;
    [SerializeField] private float jumpForce = 16f;
    [SerializeField] private float fallingGravityMultiplier = 4f;
    [SerializeField] private float lowJumpGravityMultiplier = 12f;
    [SerializeField] private float maxTrackingDistance = 5f;

    [Header("Enemy Stats")]
    [SerializeField] private float health = 30f;
    [SerializeField] private float attackSpeed = 0.5f;
    [SerializeField] private float attackRange = 2f;

    [Header("Weapons")]
    public GameObject weaponRight;

    private Animation weaponRightAnimation;


    private Rigidbody2D rb;
    private float attackTimer = 0;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        weaponRightAnimation = weaponRight.GetComponent<Animation>();
    }

    private void Update()
    {
        movementCheck();
        attackCheck();
    }

    private void movementCheck()
    {
        int playerDirection = getPlayerDirection();
        float distanceToPlayer = getDisctanceToPlayer();

        if (distanceToPlayer < attackRange)
        {
            playerDirection = 0;
        }
        else if (distanceToPlayer > maxTrackingDistance)
        {
            playerDirection = 0;
        }
        rb.velocity = new Vector2(playerDirection * movementSpeed, rb.velocity.y);

    }

    private void attackCheck()
    {
        float distanceToPlayer = getDisctanceToPlayer();

        if (distanceToPlayer < attackRange)
        {
            attackPlayer();
        }
    }

    private void attackPlayer()
    {
        if (attackTimer < attackSpeed + 1)
        {
            attackTimer += Time.deltaTime;
        }

        float playerDistance = getDisctanceToPlayer();
        if (attackTimer > attackSpeed)
        {
            weaponRightAnimation.Play();
            attackTimer = 0;
        }
    }

    private int getPlayerDirection()
    {
        float enemyX = this.transform.position.x;
        float playerX = playerController.transform.position.x;

        if (enemyX < playerX)
        {
            transform.rotation = new Quaternion(transform.rotation.x, 0, transform.rotation.x, transform.rotation.w);
            return 1;
        } 
        else
        {
            transform.rotation = new Quaternion(transform.rotation.x, 180, transform.rotation.x, transform.rotation.w);
            return -1;
        }
    }

    private float getDisctanceToPlayer()
    {
        return Math.Abs(this.transform.position.x - playerController.transform.position.x);
    }

    public void applyDamage(float damage)
    {
        health -= damage;
        if(health <= 0)
        {
            Destroy(this.gameObject);
        }
    }
}
