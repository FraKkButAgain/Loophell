using UnityEngine;

public class EnemyArrow : MonoBehaviour
{
    public float speed = 10f;
    public float lifetime = 5f;
    private bool wasReflected = false;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.Translate(transform.up * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Reflete se colidir com o escudo
        if (other.CompareTag("Shield") && !wasReflected)
        {
            Reflect();
            return;
        }

        // Acerta o jogador
        if (other.CompareTag("Player") && !wasReflected)
        {
            PlayerMovement player = other.GetComponent<PlayerMovement>();
            if (player != null)
            {
                int direction = GetAttackDirectionFromRotation();
                player.TakeDamage(direction);
            }

            Destroy(gameObject);
            return;
        }

        if (other.CompareTag("Enemy") && wasReflected)
        {
            EnemyAI enemy = other.GetComponent<EnemyAI>();
            if (enemy != null)
            {
                enemy.TakeDamage(1);
                Destroy(gameObject);
            }
            return;
        }

        if (!other.CompareTag("Enemy") && !other.CompareTag("Shield"))
        {
            Destroy(gameObject);
        }
    }

    private void Reflect()
    {
        wasReflected = true;

        transform.rotation = Quaternion.Euler(0f, 0f, transform.rotation.eulerAngles.z + 180f);

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = -rb.linearVelocity;
        }
    }

    private int GetAttackDirectionFromRotation()
    {
        float zRot = Mathf.Round(transform.rotation.eulerAngles.z) % 360f;

        if (zRot == 0f) return 1;
        if (zRot == 180f) return 2;
        if (zRot == 90f) return 3;
        if (zRot == 270f || zRot == -90f) return 4;

        return 0;
    }
}