using UnityEngine;

public class EnemyArrow : MonoBehaviour
{
    public float speed = 10f;
    public float lifetime = 5f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.Translate(Vector3.up * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMovement player = other.GetComponent<PlayerMovement>();
            if (player != null)
            {
                int direction = GetAttackDirectionFromRotation();
                player.TakeDamage(direction);
            }
        }


        if (!other.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }
    }

    private int GetAttackDirectionFromRotation()
    {
        float zRot = Mathf.Round(transform.rotation.eulerAngles.z);

        if (zRot == 0f) return 1;
        if (zRot == 180f) return 2;
        if (zRot == 90f) return 3;
        if (zRot == 270f || zRot == -90f) return 4;

        return 0;
    }
}
