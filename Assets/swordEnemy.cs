using UnityEngine;

public class EnemySlash : MonoBehaviour
{
    public float lifetime = 0.5f;

    void Start()
    {
        // Destrói automaticamente após o tempo
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {


        if (other.CompareTag("Player"))
        {
            // Se o Body tiver o script, esse GetComponent vai funcionar
            PlayerMovement player = other.GetComponent<PlayerMovement>();
            Debug.Log("[EnemySlash] Acertou o Player!");
            if (player != null)
            {
                int direction = GetAttackDirectionFromRotation();
                Debug.Log($"[EnemySlash] Direção do ataque: {direction}");
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