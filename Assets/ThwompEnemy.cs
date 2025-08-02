using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyThwomp : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float timeBetweenActions = 1f;
    public int maxHealth = 6;
    public List<int> actionSequence = new List<int> { 1, 2, 3, 4 };

    private Rigidbody2D rb;
    private int currentActionIndex = 0;
    private bool isMoving = false;
    private Dictionary<int, Vector2> directionToVector;
    private int currentDirection = 0;
    private int currentHealth;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;

        directionToVector = new Dictionary<int, Vector2>
        {
            { 1, Vector2.up },
            { 2, Vector2.down },
            { 3, Vector2.left },
            { 4, Vector2.right }
        };

        StartCoroutine(ActionLoop());
    }

    private IEnumerator ActionLoop()
    {
        while (true)
        {
            if (!isMoving)
            {
                int action = actionSequence[currentActionIndex];
                currentActionIndex = (currentActionIndex + 1) % actionSequence.Count;

                SetDirection(action);
                yield return StartCoroutine(MoveUntilCollision());
                yield return new WaitForSeconds(timeBetweenActions);
            }

            yield return null;
        }
    }

    private void SetDirection(int direction)
    {
        currentDirection = direction;
    }

    private IEnumerator MoveUntilCollision()
    {
        isMoving = true;
        Vector2 direction = directionToVector[currentDirection];

        while (true)
        {
            RaycastHit2D hit = Physics2D.Raycast(
                (Vector2)transform.position + direction * 1f,
                direction,
                0.3f
            );

            if (hit.collider != null && hit.collider.gameObject != gameObject)
            {
                string tag = hit.collider.tag;

                if (tag != "Player" && tag != "Enemy")
                {
                    break;
                }
            }

            Vector2 nextPos = rb.position + direction * moveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(nextPos);

            yield return new WaitForFixedUpdate();
        }

        isMoving = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMovement player = other.GetComponent<PlayerMovement>();
            if (player != null)
            {
                player.TakeDamage(GetDamageDirection());
            }


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

    private int GetDamageDirection()
    {
        switch (currentDirection)
        {
            case 1: return 3; // Cima → dano vem da esquerda
            case 2: return 4; // Baixo → dano vem da direita
            case 3: // Esquerda
            case 4: // Direita
            default: return 2; // dano de baixo
        }
    }
}
