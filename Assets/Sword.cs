using UnityEngine;

public class Sword : MonoBehaviour
{
    public int damage = 1;
    public float duration = 0.25f; 

    private void Start()
    {
        Destroy(gameObject, duration);
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
        }
        else if (other.CompareTag("Bomb"))
        {

            Bomb bomb = other.GetComponent<Bomb>();
            if (bomb != null)
            {
                Vector2 direction = GetDirectionFromRotation();
                bomb.PushInDirection(direction, 3f);
            }
        }
    }
    
 
        private Vector2 GetDirectionFromRotation()
    {
        float zRot = transform.rotation.eulerAngles.z;
        zRot = Mathf.Round(zRot);

        if (zRot == 0f) return Vector2.up;
        if (zRot == 180f) return Vector2.down;
        if (zRot == 90f) return Vector2.left;
        if (zRot == 270f || zRot == -90f) return Vector2.right;

        return Vector2.zero;
    }
}