using UnityEngine;

public class ButtonDeactivateDoor : MonoBehaviour
{
    public GameObject porta;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && porta != null)
        {
            porta.SetActive(false);

            // Escurece o botão: 70% da luminosidade original
            spriteRenderer.color = originalColor * 0.7f;
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, originalColor.a); // preserva alpha
        }
    }
}