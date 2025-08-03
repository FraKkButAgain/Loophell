using UnityEngine;

public class SwordAttack : MonoBehaviour
{
    public GameObject warningArea;
    public GameObject hitbox;
    public float warningDuration = 1.5f;
    public float hitDuration = 0.5f;

    private void Start()
    {
        warningArea.SetActive(true);
        hitbox.SetActive(false);

        Invoke(nameof(ActivateHitbox), warningDuration);
    }

    private void ActivateHitbox()
    {
        warningArea.SetActive(false);
        hitbox.SetActive(true);

        Invoke(nameof(DestroySelf), hitDuration);
    }

    private void DestroySelf()
    {
        Destroy(gameObject);
    }
}
