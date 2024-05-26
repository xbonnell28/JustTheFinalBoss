using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [SerializeField] GameManager gameManager;

    [Header("Layers Info")]
    public string groundLayer;
    public string wallLayer;
    public string hazardLayer;

    [Header("Movement")]
    [SerializeField] private float groundMovementAcceleration;
    [SerializeField] private float maxGroundMovementSpeed;
    [SerializeField] private float airMovementAcceleration;
    [SerializeField] private float maxAirMovementSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float wallJumpVerticalForce;
    [SerializeField] private float wallJumpHorizontalForce;
    [SerializeField] private float fallingGravityMultiplier;
    [SerializeField] private float lowJumpGravityMultiplier;  
    [Tooltip("The max duration in seconds that a player can hold the jump button")]
    [SerializeField] private float maxHeldJumpTimer;
    [SerializeField] private float knockbackForce;
    [Tooltip("How long the player has held jump for")]
    

    [Header("Walljump Values")]
    [SerializeField] private float wallSlideSpeed;
    [SerializeField] private float wallJumpTimer;

    [Header("Player Stats")]
    [SerializeField] private float health;
    [SerializeField] private float attackSpeed;
    public float damage;
    [SerializeField] private float invincibilityDuration;
    public float coyoteJumpTimer;
    public float coyoteJumpWindow;

    [Header("Weapons")]
    public Weapon weaponRight;
    public Weapon weaponUp;
    public Weapon weaponDown;

    private Rigidbody2D rb;
    public bool isGrounded;
    public bool isWallSliding;
    public Boolean wasOnWall;
    public float currentJumpTime;
    private int direction = 1; // -1 is left 1 is right
    private Direction lastAttackDirection;
    private float attackTimer = 0;
    private bool invulnerable = false;
    private SpriteRenderer spriteRenderer;
    public bool hasJumped;

    enum Direction
    {
        Up , Down , Left, Right
    }

    void Awake()
    {
        gameManager.updateCharacterHealth(health);
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        isGrounded = true;
        isWallSliding = false;
        wasOnWall = false;
        currentJumpTime = 0;
        Time.timeScale = 1.0f;
    }

    void Update()
    {
        float horizontalInput, verticalInput;
        movementCheck(out horizontalInput, out verticalInput);
        jumpCheck();
        attackCheck(horizontalInput, verticalInput);
        gravityCheck();
    }

    private void attackCheck(float horizontalInput, float verticalInput)
    {
        if (attackTimer < attackSpeed)
        {
            attackTimer += Time.deltaTime;
        }

        if (attackTimer > attackSpeed && Input.GetButtonDown("Fire1"))
        {
            attackTimer = 0;
            if (verticalInput < 0) // Down
            {
                weaponDown.Attack();
                lastAttackDirection = Direction.Down;
            } 
            else if (verticalInput > 0) // Up
            {
                weaponUp.Attack();
                lastAttackDirection = Direction.Up;
            }
            else
            {
                weaponRight.Attack();
                lastAttackDirection = Direction.Right;
            }
        }
    }

    private void movementCheck(out float horizontalInput, out float verticalInput)
    {
        verticalInput = Input.GetAxisRaw("Vertical");
        horizontalInput = Input.GetAxisRaw("Horizontal");

        // Cacheing last faced direction
        if (horizontalInput > 0)
        {
            transform.rotation = new Quaternion(transform.rotation.x, 0, transform.rotation.x, transform.rotation.w);
            direction = 1;
        }
        else if (horizontalInput < 0)
        {
            transform.rotation = new Quaternion(transform.rotation.x, 180, transform.rotation.x, transform.rotation.w);
            direction = -1;
        }
        
        if(horizontalInput == 0 && !hasJumped)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
        else
        {
            if (!isGrounded)
            {
                float x = Mathf.Clamp(rb.velocity.x + horizontalInput * airMovementAcceleration, -maxAirMovementSpeed, maxAirMovementSpeed);
                rb.velocity = new Vector2(x, rb.velocity.y);
            }
            else
            {
                float x = Mathf.Clamp(rb.velocity.x + horizontalInput * groundMovementAcceleration, -maxGroundMovementSpeed, maxGroundMovementSpeed);
                rb.velocity = new Vector2(x + horizontalInput * groundMovementAcceleration, rb.velocity.y);
            }
        }
    }

    private void jumpCheck()
    {
        if (!isGrounded)
        {
            coyoteJumpTimer += Time.deltaTime;
        }
        if (Input.GetButtonDown("Jump"))
        {
            if (isGrounded)
            {
                jump(Vector2.up * jumpForce);
            }
            else if (!isGrounded && coyoteJumpTimer < coyoteJumpWindow && !hasJumped)
            {
                jump(Vector2.up * jumpForce);
            }
            else if (isWallSliding)
            {
                jump(new Vector2(-direction * wallJumpHorizontalForce, wallJumpVerticalForce));
                isWallSliding = false;
                wasOnWall = true;
            }
        }
    }

    private void jump(Vector2 jumpVector)
    {
        rb.velocity = jumpVector;
        currentJumpTime = 0;
        hasJumped = true;
    }

    private void gravityCheck()
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
        else if (Input.GetButton("Jump"))
        {
            currentJumpTime += Time.deltaTime;
        }
    }

    private bool ShortHopCheck()
    {
        return rb.velocity.y > 0 
            && (!Input.GetButton("Jump") || currentJumpTime > maxHeldJumpTimer)
            || wasOnWall;
    }

    public void pogoKnockback()
    {
        if (lastAttackDirection == Direction.Right)
        {
            rb.velocity = new Vector2(-knockbackForce, rb.velocity.y);
        }
        else if (lastAttackDirection == Direction.Left)
        {
            rb.velocity = new Vector2(knockbackForce, rb.velocity.y);
        }
        else if (lastAttackDirection == Direction.Up)
        {
            rb.velocity = new Vector2(rb.velocity.x, -knockbackForce);
        }
        else if (lastAttackDirection == Direction.Down)
        {
            rb.velocity = new Vector2(rb.velocity.x, knockbackForce);
        }
    }

    public void applyDamage(int  damage)
    {
        if (!invulnerable)
        {
            health -= damage;
            gameManager.updateCharacterHealth(health);
            invulnerable = true;
            StartCoroutine(iFrameFlash());
        }
    }

    public void applyKnockback(Vector2 knockbackVector)
    {
        if (!invulnerable)
        {
            rb.velocity = knockbackVector;
        }
    }

    private IEnumerator iFrameFlash()
    {
        InvokeRepeating("flashCharacter", 0f, 0.15f);
        yield return new WaitForSeconds(invincibilityDuration);
        CancelInvoke("flashCharacter");
        spriteRenderer.enabled = true;
        invulnerable = false;
    }

    void flashCharacter()
    {
        spriteRenderer.enabled = !spriteRenderer.enabled;
    }
}
