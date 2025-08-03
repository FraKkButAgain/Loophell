using UnityEngine;

public class EnemySlash : MonoBehaviour
{
    public float lifetime = 0.5f;

    void Start()
    {

        Destroy(gameObject, lifetime);
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
    }

    private int GetAttackDirectionFromRotation()
    {
        float zRot = transform.rotation.eulerAngles.z;
        zRot = Mathf.Round(zRot);

        if (zRot == 0f) return 1;
        if (zRot == 180f) return 2;
        if (zRot == 90f) return 3;
        if (zRot == 270f || zRot == -90f) return 4;

        return 0; // fallback seguro
    }
}