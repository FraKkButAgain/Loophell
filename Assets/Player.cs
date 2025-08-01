using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    

    private List<int> actionQueue = new List<int> { 0, 1, 2 };

    public GameObject swordObject; 
    public Animator swordAnimator;
    public Animator animator;
    public GameObject projectilePrefab;

    private Rigidbody2D rb;
    private Vector2 movement;

    
    private bool isMoving = false;
    private int currentDirection = 0; 

    private bool canAct = true;

    private bool isDashing = false;
    public float dashSpeed = 10f;
    public float dashDuration = 0.3f;

    private Vector2 lastMoveDirection = Vector2.down;



    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

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
        if (!isDashing)
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
        if (actionQueue.Count == 0) return;

        int action = actionQueue[0];
        actionQueue.RemoveAt(0);    
        actionQueue.Add(action);      

        switch (action)
        {
            case 0:
                if (canAct)
                    StartCoroutine(AttackSword());
                break;

            case 1:
                if (canAct)
                    StartCoroutine(Dash());
                    animator.SetTrigger("Dash");
                break;

            case 2:
                if (canAct)
                ShootProjectile();
                break;
        }
    }

    // movimentos aqui

    private IEnumerator Dash()
    {
        canAct = false;
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

        yield return new WaitForSeconds(0.3f); 
        canAct = true;
    }

    private IEnumerator AttackSword()
    {
        canAct = false;

        animator.SetTrigger("Attack");
        swordObject.SetActive(true);

        switch (currentDirection)
        {
            case 1:
                swordObject.transform.localPosition = new Vector2(0, 0.5f);
                swordObject.transform.localRotation = Quaternion.Euler(0, 0, 0);
                break;
            case 2:
                swordObject.transform.localPosition = new Vector2(0, -0.5f);
                swordObject.transform.localRotation = Quaternion.Euler(0, 0, 180);
                break;
            case 3:
                swordObject.transform.localPosition = new Vector2(0.5f, 0);
                swordObject.transform.localRotation = Quaternion.Euler(0, 0, 270);
                break;
            case 4:
                swordObject.transform.localPosition = new Vector2(-0.5f, 0);
                swordObject.transform.localRotation = Quaternion.Euler(0, 0, 90);
                break;
        }
                
        
        yield return new WaitForSeconds(0.3f);
        canAct = true;

        yield return new WaitForSeconds(0.3f);
        swordObject.SetActive(false);


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


}