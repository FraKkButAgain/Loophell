using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemyAI : MonoBehaviour
{
    public float tileSize = 2f;
    public float timeBetweenActions = 1.5f;
    public int maxHealth = 3;
    public List<int> actionSequence = new List<int> { 1, 0, 2, 0, 3, 0 };

    public GameObject areaAttackPrefab;

    private int currentHealth;
    private int currentActionIndex = 0;
    private bool isPerformingAction = false;
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private int currentDirection = 4; // Começa virado pra direita (4)

    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        StartCoroutine(EnemyLoop());
    }

    private IEnumerator EnemyLoop()
    {
        float nextActionTime = Time.time;

        while (currentHealth > 0)
        {
            if (!isPerformingAction && Time.time >= nextActionTime)
            {
                int action = actionSequence[currentActionIndex];
                currentActionIndex = (currentActionIndex + 1) % actionSequence.Count;

                StartCoroutine(PerformAction(action));
                nextActionTime = Time.time + timeBetweenActions;
            }

            yield return null;
        }

        Die();
    }

    private IEnumerator PerformAction(int action)
    {
        isPerformingAction = true;

        switch (action)
        {
            case 0:
                animator.SetBool("IsWalking", false);
                yield return new WaitForSeconds(2f);
                break;

            case 1:
                SetDirection(3);
                yield return MoveInDirection(Vector2.left);
                break;

            case 2: 
                SetDirection(4);
                yield return MoveInDirection(Vector2.right);
                break;

            case 3: 
                yield return PerformAnimatedAttack();
                break;
        }

        isPerformingAction = false;
    }

    private IEnumerator MoveInDirection(Vector2 direction)
    {
        animator.SetBool("IsWalking", true);

        Vector2 start = transform.position;
        Vector2 end = start + direction * tileSize;
        float duration = timeBetweenActions;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            transform.position = Vector2.Lerp(start, end, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = end;
        animator.SetBool("IsWalking", false);
    }

    private void SetDirection(int direction)
    {
        currentDirection = direction;

        float dirValue = 0f;
        switch (direction)
        {
            case 3:
                dirValue = -1f;
                spriteRenderer.flipX = false; 
                break;

            case 4: 
                dirValue = 1f;
                spriteRenderer.flipX = true; 
                break;

            default:
                dirValue = 0f;
                break;
        }

        animator.SetFloat("Direction", dirValue);
    }

    private IEnumerator PerformAnimatedAttack()
    {
        animator.SetTrigger("Attack");


        GameObject attackInstance = Instantiate(areaAttackPrefab, transform.position, Quaternion.identity);


        yield return new WaitForSeconds(2f);
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            StopAllCoroutines();
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}