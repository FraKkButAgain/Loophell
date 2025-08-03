using UnityEngine;

public class Explosion : MonoBehaviour
{
    public Vector2 explosionSize = new Vector2(2f, 2f);

    public void Explode()
    {
        Vector2 center = transform.position;

        Collider2D[] hits = Physics2D.OverlapBoxAll(center, explosionSize, 0f);

        foreach (Collider2D hit in hits)
        {


            if (hit.CompareTag("Enemy"))
            {
                EnemyAI enemy = hit.GetComponent<EnemyAI>();
                if (enemy != null)
                    enemy.TakeDamage(3);
            }
            else if (hit.CompareTag("Player"))
            {
                PlayerMovement player = hit.GetComponent<PlayerMovement>();
                if (player != null)
                    player.TakeDamage(0);
            }
            else if (hit.CompareTag("Breakable"))
            {
                Destroy(hit.gameObject);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, explosionSize);
    }
}
