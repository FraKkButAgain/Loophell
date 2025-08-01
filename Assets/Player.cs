using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;

    public GameObject swordObject; 
    public Animator swordAnimator;

    private Rigidbody2D rb;
    private Vector2 movement;
    private Animator animator;

    private bool isMoving = false;
    private int currentDirection = 0; 

    private bool canAttack = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Read input
        var gamepad = Gamepad.current;
        var keyboard = Keyboard.current;
        

        if (keyboard != null)
        {
            movement.x = (keyboard.aKey.isPressed ? -1f : 0f) + (keyboard.dKey.isPressed ? 1f : 0f);
            movement.y = (keyboard.sKey.isPressed ? -1f : 0f) + (keyboard.wKey.isPressed ? 1f : 0f);
        }

    if (Keyboard.current.zKey.wasPressedThisFrame && canAttack)
    {
        StartCoroutine(AttackSword());
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
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement.normalized * moveSpeed * Time.fixedDeltaTime);
    }

    private int GetDirectionFromInput(Vector2 input)
    {
        if (Mathf.Abs(input.y) > Mathf.Abs(input.x))
            return input.y > 0 ? 1 : 2;
        else
            return input.x > 0 ? 3 : 4;
    }


    private IEnumerator AttackSword()
    {
        canAttack = false;

        animator.SetTrigger("Attack");
        swordObject.SetActive(true);



        switch (currentDirection)
        {
            case 1:
                swordObject.transform.localPosition = new Vector2(0, 0.2f);
                swordObject.transform.localRotation = Quaternion.Euler(0, 0, 0);
                break;
            case 2:
                swordObject.transform.localPosition = new Vector2(0, -0.2f);
                swordObject.transform.localRotation = Quaternion.Euler(0, 0, 180);
                break;
            case 3:
                swordObject.transform.localPosition = new Vector2(0.2f, 0);
                swordObject.transform.localRotation = Quaternion.Euler(0, 0, 270);
                break;
            case 4:
                swordObject.transform.localPosition = new Vector2(-0.2f, 0);
                swordObject.transform.localRotation = Quaternion.Euler(0, 0, 90);
                break;
        }

        yield return new WaitForSeconds(0.5f);
        swordObject.SetActive(false);

        yield return new WaitForSeconds(0.5f); 
        canAttack = true;
    }
}