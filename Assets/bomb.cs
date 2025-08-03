using UnityEngine;

public class Bomb : MonoBehaviour
{
    public float fuseTime = 5f;
    public float selfDestructDelay = 0.5f;
    public GameObject explosionObject;

    private Explosion explosion;

    private void Start()
    {
        if (explosionObject != null)
        {
            explosion = explosionObject.GetComponent<Explosion>();
            explosionObject.SetActive(false); 
        }

        Invoke(nameof(Explode), fuseTime);
    }

    private void Explode()
{

    Rigidbody2D rb = GetComponent<Rigidbody2D>();
    if (rb != null)
    {
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.bodyType = RigidbodyType2D.Static;
    }


    SpriteRenderer sr = GetComponent<SpriteRenderer>();
    if (sr != null)
    {
        sr.enabled = false;
    }

    Collider2D col = GetComponent<Collider2D>();
    if (col != null)
    {
        col.enabled = false;
    }


    if (explosionObject != null)
    {
        explosionObject.SetActive(true); 

        if (explosion != null)
        {
            explosion.Explode(); 
        }
    }

    Destroy(gameObject, selfDestructDelay);
}

    public void PushInDirection(Vector2 direction, float distance)
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.linearVelocity = direction.normalized * distance * 5f;
        }
    }
}
