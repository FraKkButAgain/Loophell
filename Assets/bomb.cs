using UnityEngine;

public class Bomb : MonoBehaviour
{
    public float fuseTime = 5f;
    public float selfDestructDelay = 0.5f;
    public GameObject explosionObject;

    private void Start()
    {
        Invoke("Explode", fuseTime);
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

        if (explosionObject != null)
        {
            explosionObject.SetActive(true);
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