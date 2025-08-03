using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    public GameObject swordObject;
    public Animator swordAnimator;
    public Animator animator;

    [Header("Settings")]
    public float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Vector2 movement;
    private bool isMoving = false;
    private int currentDirection = 0;
    private bool canAttack = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb) rb.simulated = false;
    }

    private void Start()
    {
        if (PlayerSpawnSystem.Instance)
        {
            PlayerSpawnSystem.Instance.PositionPlayer();
            if (rb) rb.simulated = true;
        }
    }

    void Update()
    {
        HandleMovementInput();
        HandleAttackInput();
        UpdateAnimations();
    }

    void FixedUpdate()
    {
        if (rb && rb.simulated)
            rb.MovePosition(rb.position + movement.normalized * moveSpeed * Time.fixedDeltaTime);
    }

    private void HandleMovementInput()
    {
        movement = Vector2.zero;
        if (Keyboard.current.aKey.isPressed) movement.x -= 1;
        if (Keyboard.current.dKey.isPressed) movement.x += 1;
        if (Keyboard.current.sKey.isPressed) movement.y -= 1;
        if (Keyboard.current.wKey.isPressed) movement.y += 1;
    }

    private void HandleAttackInput()
    {
        if (Keyboard.current.zKey.wasPressedThisFrame && canAttack)
            StartCoroutine(AttackSword());
    }

    private void UpdateAnimations()
    {
        bool nowMoving = movement.sqrMagnitude > 0.01f;
        if (nowMoving != isMoving)
        {
            isMoving = nowMoving;
            animator.SetBool("IsMoving", isMoving);
        }
        if (isMoving) animator.SetFloat("Direction", GetDirectionFromInput(movement));
    }

    private int GetDirectionFromInput(Vector2 input)
    {
        if (Mathf.Abs(input.y) > Mathf.Abs(input.x))
            return input.y > 0 ? 1 : 2;
        return input.x > 0 ? 3 : 4;
    }

    private IEnumerator AttackSword()
    {
        canAttack = false;
        animator.SetTrigger("Attack");
        swordObject.SetActive(true);

        switch (currentDirection)
        {
            case 1: SetSwordPosition(0, 0.5f, 0); break;
            case 2: SetSwordPosition(0, -0.5f, 180); break;
            case 3: SetSwordPosition(0.5f, 0, 270); break;
            case 4: SetSwordPosition(-0.5f, 0, 90); break;
        }

        yield return new WaitForSeconds(0.3f);
        swordObject.SetActive(false);
        yield return new WaitForSeconds(0.3f);
        canAttack = true;
    }

    private void SetSwordPosition(float x, float y, float rotation)
    {
        swordObject.transform.localPosition = new Vector2(x, y);
        swordObject.transform.localRotation = Quaternion.Euler(0, 0, rotation);
    }
}