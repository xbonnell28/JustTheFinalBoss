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
    [SerializeField] private float lerpAmount;
    [SerializeField] private float groundMovementAcceleration;
    [SerializeField] private float groundMovementDeceleration;
    [SerializeField] private float maxGroundMovementSpeed;
    [SerializeField] private float airMovementAcceleration;
    [SerializeField] private float airMovementDeceleration;
    [SerializeField] private float maxAirMovementSpeed;
    [SerializeField] private bool doConserveMomentum;
    [SerializeField] private float jumpHangTimeThreshold;
    [SerializeField] private float jumpHangAccelerationMult;
    [SerializeField] private float jumpHangMaxSpeedMult;
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
    private float coyoteJumpWindow;
    public float jumpBufferTime;
    private float jumpBufferCounter;

    [Header("Weapons")]
    public Weapon weaponRight;
    public Weapon weaponUp;
    public Weapon weaponDown;

    private Rigidbody2D rb;
    public bool isGrounded;
    public bool isWallSliding;
    public Boolean wasOnWall;
    public float currentJumpTime;
    public int direction = 1; // -1 is left 1 is right
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

        float targetSpeed;
        if (isGrounded)
        {
            targetSpeed = horizontalInput * maxGroundMovementSpeed;
        }
        else
        {
            targetSpeed = horizontalInput * maxAirMovementSpeed;
        }

        targetSpeed = Mathf.Lerp(rb.velocity.x, targetSpeed, lerpAmount);

        float accelRate;

        if(isGrounded)
        {
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? groundMovementAcceleration : groundMovementDeceleration;
        }
        else
        {
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? groundMovementAcceleration * airMovementAcceleration : groundMovementDeceleration * airMovementDeceleration;
        }

        // Air acceleration
        if (hasJumped && Mathf.Abs(rb.velocity.y) < jumpHangTimeThreshold)
        {
            accelRate *= jumpHangAccelerationMult;
            targetSpeed *= jumpHangMaxSpeedMult;
        }

        // Momentum Conservation
        if (doConserveMomentum && Mathf.Abs(rb.velocity.x) > Mathf.Abs(targetSpeed) && Mathf.Sign(rb.velocity.x) == Mathf.Sign(targetSpeed) && Mathf.Abs(targetSpeed) > 0.01f && !isGrounded)
        {
            //Prevent any deceleration from happening, or in other words conserve are current momentum
            //You could experiment with allowing for the player to slightly increae their speed whilst in this "state"
            accelRate = 0;
        }

        float speedDif = targetSpeed - rb.velocity.x;

        float movement = speedDif * accelRate;

        rb.AddForce(movement * Vector2.right, ForceMode2D.Force);
    }

    private void jumpCheck()
    {
        if (!isGrounded)
        {
            coyoteJumpTimer += Time.deltaTime;
        }
        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBufferTime;
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
                WallJump(-direction);
                isWallSliding = false;
                wasOnWall = true;
            }
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
            if (isGrounded && jumpBufferCounter > 0 && !hasJumped)
            {
                jump(Vector2.up * jumpForce);
            }
        }
    }

    private void jump(Vector2 jumpVector)
    {
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(jumpVector, ForceMode2D.Impulse);
        currentJumpTime = 0;
        hasJumped = true;
    }

    private void WallJump(int dir)
    {
        Vector2 force = new Vector2(wallJumpHorizontalForce, wallJumpVerticalForce);
        force.x *= dir; //apply force in opposite direction of wall

        if (Mathf.Sign(rb.velocity.x) != Mathf.Sign(force.x))
            force.x -= rb.velocity.x;

        if (rb.velocity.y < 0) //checks whether player is falling, if so we subtract the velocity.y (counteracting force of gravity). This ensures the player always reaches our desired jump force or greater
            force.y -= rb.velocity.y;

        //Unlike in the run we want to use the Impulse mode.
        //The default mode will apply are force instantly ignoring masss
        rb.AddForce(force, ForceMode2D.Impulse);
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
