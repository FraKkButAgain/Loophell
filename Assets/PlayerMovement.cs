using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

public class PlayerMovement : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public float moveSpeed = 5f;

    public float actionCooldown = 1.2f; 
    private float lastActionTime = -999f;
    

    private List<int> actionQueue = new List<int> { 0, 1, 2 };

    public GameObject swordPrefab;
    public Animator swordAnimator;
    public Animator animator;
    public GameObject projectilePrefab;

    private Rigidbody2D rb;
    private Vector2 movement;

    
    private bool isMoving = false;
    private int currentDirection = 0; 



    private bool isDashing = false;
    public float dashSpeed = 10f;
    public float dashDuration = 0.3f;

    private Vector2 lastMoveDirection = Vector2.down;

    public int maxHealth = 3;
    private int currentHealth;

    private bool isInvincible = false;
    private bool isStunned = false;

    public float hurtForce = 5f;



    void Start()
    {
        spriteRenderer = transform.Find("Square").GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;

    }

    void Update()
    {
        // Read input
        var gamepad = Gamepad.current;
        var keyboard = Keyboard.current;
        

        if (keyboard != null)
        {
            movement.x = (keyboard.leftArrowKey.isPressed ? -1f : 0f) + (keyboard.rightArrowKey.isPressed ? 1f : 0f);
            movement.y = (keyboard.downArrowKey.isPressed ? -1f : 0f) + (keyboard.upArrowKey.isPressed ? 1f : 0f);
        }

    bool nowMoving = movement.sqrMagnitude > 0.01f;


        if (nowMoving != isMoving)
        {
            isMoving = nowMoving;
            animator.SetBool("IsMoving", isMoving);
        }


        if (isMoving)
        {
            int newDirection = GetDirectionFromInput(movement);

            if (newDirection != currentDirection)
            {
                currentDirection = newDirection;
                animator.SetFloat("Direction", (float)currentDirection);
            }
        }

        // movimentos aqui:
        // 

        if (Keyboard.current.zKey.wasPressedThisFrame)
        {
            HandleAction();
        }


    }

    void FixedUpdate()
    {
        if (isStunned || isDashing) return;
        {
            rb.MovePosition(rb.position + movement.normalized * moveSpeed * Time.fixedDeltaTime);
        }
    }

    private int GetDirectionFromInput(Vector2 input)
    {
        if (Mathf.Abs(input.y) > Mathf.Abs(input.x))
            return input.y > 0 ? 1 : 2;
        else
            return input.x > 0 ? 3 : 4;
    }

    // faz as ações acontecerem
    private void HandleAction()
    {
        if (Time.time < lastActionTime + actionCooldown)
            return; 

        lastActionTime = Time.time;

        if (actionQueue.Count == 0) return;

        int action = actionQueue[0];
        actionQueue.RemoveAt(0);
        actionQueue.Add(action);

        switch (action)
        {
            case 0:
                StartCoroutine(AttackSword());
                break;

            case 1:
                StartCoroutine(Dash());
                animator.SetTrigger("Dash");
                break;

            case 2:
                ShootProjectile();
                break;
        }
    }

    // movimentos aqui

    private IEnumerator Dash()
    {

        isDashing = true;

        Vector2 dashDirection = movement.sqrMagnitude > 0.01f ? movement.normalized : Vector2.zero;
        float dashTime = 0f;

        while (dashTime < dashDuration)
        {
            if (dashDirection != Vector2.zero)
            {
                rb.MovePosition(rb.position + dashDirection * dashSpeed * Time.fixedDeltaTime);
            }

            dashTime += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        isDashing = false;


    }

    private IEnumerator AttackSword()
    {

        animator.SetTrigger("Attack");

        


        GameObject sword = Instantiate(swordPrefab, transform);

        Animator swordAnimator = sword.GetComponent<Animator>();
        if (swordAnimator != null)
            swordAnimator.SetTrigger("Attack");


        switch (currentDirection)
        {
            case 1: // cima
                sword.transform.localPosition = new Vector2(0, 0.5f);
                sword.transform.localRotation = Quaternion.Euler(0, 0, 0);
                break;
            case 2: // baixo
                sword.transform.localPosition = new Vector2(0, -0.5f);
                sword.transform.localRotation = Quaternion.Euler(0, 0, 180);
                break;
            case 4: // esquerda
                sword.transform.localPosition = new Vector2(-0.5f, 0);
                sword.transform.localRotation = Quaternion.Euler(0, 0, 90);
                break;
            case 3: // direita
                sword.transform.localPosition = new Vector2(0.5f, 0);
                sword.transform.localRotation = Quaternion.Euler(0, 0, 270);
                break;
        }

        var playerCol = GetComponent<Collider2D>();
        var swordCol = sword.GetComponent<Collider2D>();
        if (playerCol && swordCol)
            Physics2D.IgnoreCollision(playerCol, swordCol, true);

        yield return new WaitForSeconds(0.5f);


        yield return new WaitForSeconds(0.3f);
        if (sword) Destroy(sword);
    }
    
    private void ShootProjectile()
    {
        Vector3 spawnPosition = transform.position;

        Quaternion rotation = Quaternion.identity;

        switch (currentDirection)
        {
            case 1: 
                rotation = Quaternion.Euler(0, 0, 0);
                break;
            case 2:
                rotation = Quaternion.Euler(0, 0, 180);
                break;
            case 3:
                rotation = Quaternion.Euler(0, 0, -90);
                break;
            case 4: 
                rotation = Quaternion.Euler(0, 0, 90);
                break;
        }
        

        GameObject projectile = Instantiate(projectilePrefab, spawnPosition, rotation);
        Physics2D.IgnoreCollision(projectile.GetComponent<Collider2D>(), GetComponent<Collider2D>());
    
    }

    public void TakeDamage(int attackDirection)
    {
        if (isInvincible || isDashing) return;

        currentHealth--;
        animator.SetTrigger("Hurt");

        StartCoroutine(ApplyKnockback(attackDirection));

        StartCoroutine(HurtRoutine());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

private IEnumerator HurtRoutine()
{
    isStunned = true;
    isInvincible = true;
    
    StartCoroutine(FlashWhileInvincible());

        yield return new WaitForSeconds(0.3f); 
        rb.linearVelocity = Vector2.zero;            
        yield return new WaitForSeconds(0.7f); 
        isStunned = false;

        yield return new WaitForSeconds(2f);  
        isInvincible = false;
    }

    private void Die()
    {
        Destroy(gameObject);

    }   
    private IEnumerator ApplyKnockback(int direction)
    {
        Vector2 knockbackDir = Vector2.zero;

        switch (direction)
        {
            case 1: knockbackDir = Vector2.up; break;
            case 2: knockbackDir = Vector2.down; break;
            case 3: knockbackDir = Vector2.left; break;
            case 4: knockbackDir = Vector2.right; break;
            default: knockbackDir = Vector2.zero; break;
        }

        Vector2 start = rb.position;
        Vector2 target = start + knockbackDir * 1f; 
        float duration = 0.2f; 
        float elapsed = 0f;

        while (elapsed < duration)
        {
            rb.MovePosition(Vector2.Lerp(start, target, elapsed / duration));
            elapsed += Time.deltaTime;
            yield return null;
        }

        rb.MovePosition(target);
    }
    
    private IEnumerator FlashWhileInvincible()
    {
        while (isInvincible)
        {
            spriteRenderer.enabled = false;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.enabled = true;
            yield return new WaitForSeconds(0.1f);
        }

        spriteRenderer.enabled = true;
    }


}