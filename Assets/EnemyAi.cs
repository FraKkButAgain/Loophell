using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    // Configurações
    public GameObject swordPrefab;
    public GameObject bombPrefab;
    public GameObject arrowPrefab;

    public float tileSize = 2f;
    public int maxHealth = 3;
    public List<int> actionSequence = new List<int> { 1, 5, 3, 5, 1, 5, 4, 5, 2, 5, 5 };

    public Animator animator; // Referência pública ao Animator no filho
    public SpriteRenderer spriteRenderer; // Referência pública ao SpriteRenderer no filho

    private Rigidbody2D rb;
    private int currentHealth;
    private int currentDirection = 1;

    public float timeBetweenActions = 1.5f;
    private int currentActionIndex = 0;
    private bool isPerformingAction = false;
    private Dictionary<int, Vector2> directionToVector;

    void Start()
    {
        currentHealth = maxHealth;

        directionToVector = new Dictionary<int, Vector2>
        {
            { 1, Vector2.up },
            { 2, Vector2.down },
            { 3, Vector2.left },
            { 4, Vector2.right }
        };

        rb = GetComponent<Rigidbody2D>();
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
            case 0: // idle
                animator.SetBool("IsWalking", false);
                break;

            case 1: case 2: case 3: case 4:
                SetDirection(action);
                break;

            case 5:
                yield return MoveForward();
                break;

            case 6:
                Attack();
                break;

            case 7:
                ShootArrow();
                break;

            case 8:
                ThrowBomb();
                break;
        }

        isPerformingAction = false;
    }

    private IEnumerator MoveForward()
    {
        animator.SetBool("IsWalking", true);

        Vector2 moveVector = directionToVector[currentDirection];
        Vector2 startPos = transform.position;
        Vector2 targetPos = startPos + moveVector * tileSize;

        float elapsedTime = 0f;
        float duration = timeBetweenActions;

        while (elapsedTime < duration)
        {
            transform.position = Vector2.Lerp(startPos, targetPos, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos;
        animator.SetBool("IsWalking", false);
    }

    private void SetDirection(int direction)
    {
        currentDirection = direction;

        float dirValue = 0f;
        switch (direction)
        {
            case 3: // esquerda
                dirValue = -1f; 
                spriteRenderer.flipX = false;
                break;
            case 4: // direita
                dirValue = 1f;
                spriteRenderer.flipX = true;
                break;
            default:
                dirValue = 0f;
                break;
        }

        animator.SetFloat("Direction", dirValue);
    }

    private void Attack()
    {
        animator.SetTrigger("Attack"); 
        Vector2 spawnOffset = directionToVector[currentDirection] * 1f;
        Vector2 spawnPosition = (Vector2)transform.position + spawnOffset;

        GameObject sword = Instantiate(swordPrefab, spawnPosition, Quaternion.identity);

        float angle = 0f;
        switch (currentDirection)
        {
            case 1: angle = 0f; break;
            case 2: angle = 180f; break;
            case 3: angle = 90f; break;
            case 4: angle = -90f; break;
        }

        sword.transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void ShootArrow()
    {
        animator.SetTrigger("Shoot"); 

        Vector2 spawnOffset = directionToVector[currentDirection] * 1f;
        Vector2 spawnPosition = (Vector2)transform.position + spawnOffset;

        GameObject arrow = Instantiate(arrowPrefab, spawnPosition, Quaternion.identity);

        float angle = 0f;
        switch (currentDirection)
        {
            case 1: angle = 0f; break;
            case 2: angle = 180f; break;
            case 3: angle = 90f; break;
            case 4: angle = -90f; break;
        }

        arrow.transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void ThrowBomb()
    {
        animator.SetTrigger("Attack"); 
        Vector2 spawnOffset = directionToVector[currentDirection] * 1f;
        Vector2 spawnPosition = (Vector2)transform.position + spawnOffset;

        GameObject bomb = Instantiate(bombPrefab, spawnPosition, Quaternion.identity);

        Bomb bombScript = bomb.GetComponent<Bomb>();
        if (bombScript != null)
        {
            bombScript.PushInDirection(directionToVector[currentDirection], 2f);
        }
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