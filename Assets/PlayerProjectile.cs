using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int damage = 1;
    public float speed = 8f;
    public float maxLifetime = 5f;
    public Vector2 direction;

    private void Start()
    {
        Destroy(gameObject, maxLifetime); 
    }

    void Update()
    {
        transform.Translate(Vector2.up * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyAI enemy = other.GetComponent<EnemyAI>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }

            Destroy(gameObject); 
        }
        else if (!other.CompareTag("Player")) 
        {
            Destroy(gameObject);
        }
    }
    
}


