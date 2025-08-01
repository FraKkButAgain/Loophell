using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 8f;
    public float maxLifetime = 5f;

    private void Start()
    {
        Destroy(gameObject, maxLifetime); 
    }

    void Update()
    {
        transform.Translate(Vector2.up * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Destroy(gameObject);
    }
}