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
    public List<int> actionSequence = new List<int> { 1, 5, 3, 5, 1, 5, 4, 5, 2, 5, 5};

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
            case 0: //nada
            
                break;

            case 1: // move direção (cima)
            case 2: // baixo
            case 3: // esquerda
            case 4: // direita
                SetDirection(action);

                break;

            case 5: // move pra frente
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
            default:

                break;
        }

        isPerformingAction = false;
    }



    // ações
    private IEnumerator MoveForward()
    {
        Vector2 moveVector = directionToVector[currentDirection];
        Vector2 targetPos = (Vector2)transform.position + moveVector * tileSize;

        float elapsedTime = 0f;
        float moveDuration = timeBetweenActions;
        Vector2 startPos = transform.position;

        while (elapsedTime < moveDuration)
        {
            transform.position = Vector2.Lerp(startPos, targetPos, elapsedTime / moveDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos;
        yield break;
    }

    private void SetDirection(int direction)
    {
        currentDirection = direction;

        // colocar animações aqui depois #Frank
    }

    private void Attack()
    {
        Vector2 spawnOffset = directionToVector[currentDirection] * 1;
        Vector2 spawnPosition = (Vector2)transform.position + spawnOffset;


        GameObject sword = Instantiate(swordPrefab, spawnPosition, Quaternion.identity);


        float angle = 0f;
        switch (currentDirection)
        {
            case 1: angle = 0f; break;     // cima
            case 2: angle = 180f; break;   // baixo
            case 3: angle = 90f; break;    // esquerda
            case 4: angle = -90f; break;   // direita
        }

        sword.transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void ShootArrow()
    {
        Vector2 spawnOffset = directionToVector[currentDirection] * 1f;
        Vector2 spawnPosition = (Vector2)transform.position + spawnOffset;

        GameObject arrow = Instantiate(arrowPrefab, spawnPosition, Quaternion.identity);

        float angle = 0f;
        switch (currentDirection)
        {
            case 1: angle = 0f; break;     // cima
            case 2: angle = 180f; break;   // baixo
            case 3: angle = 90f; break;    // esquerda
            case 4: angle = -90f; break;   // direita
        }

        arrow.transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void ThrowBomb()
    {
        Vector2 spawnOffset = directionToVector[currentDirection] * 1f;
        Vector2 spawnPosition = (Vector2)transform.position + spawnOffset;

        GameObject bomb = Instantiate(bombPrefab, spawnPosition, Quaternion.identity);

        Bomb bombScript = bomb.GetComponent<Bomb>();
        if (bombScript != null)
        {
            bombScript.PushInDirection(directionToVector[currentDirection], 2f); 
        }
    }

    // bullshit
    private Vector3 GetDirectionVector()
    {
        return currentDirection switch
        {
            1 => Vector3.up,
            2 => Vector3.down,
            3 => Vector3.right,
            4 => Vector3.left,
            _ => Vector3.zero
        };
    }

    // Dano
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

