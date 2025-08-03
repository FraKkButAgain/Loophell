using UnityEngine;
using UnityEngine.UI;

public class SpeechBubble : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float maxDistance = 3f; // Fade starts at this distance
    [SerializeField] private float minOpacity = 0f; // Minimum visibility at maxDistance

    [Header("References")]
    [SerializeField] private Transform player; // Assign PLAYER GameObject in Inspector
    [SerializeField] private Image bubbleImage;
    [SerializeField] private Text bubbleText;

    private void Update()
    {
        if (player == null || bubbleImage == null || bubbleText == null)
        {
            Debug.LogWarning("Missing references!");
            return;
        }

        // Get actual world positions (not local)
        Vector3 bubblePos = transform.position;
        Vector3 playerPos = player.position;

        float distance = Vector3.Distance(bubblePos, playerPos);
        float opacity = CalculateOpacity(distance);

        ApplyOpacity(opacity);

        Debug.Log($"Distance: {distance} | Opacity: {opacity}"); // Verify in Console
    }

    private float CalculateOpacity(float distance)
    {
        // Returns 1 when close, minOpacity when at maxDistance
        return Mathf.Clamp(1 - (distance / maxDistance), minOpacity, 1f);
    }

    private void ApplyOpacity(float alpha)
    {
        SetAlpha(bubbleImage, alpha);
        SetAlpha(bubbleText, alpha);
    }

    private void SetAlpha(Graphic graphic, float alpha)
    {
        Color color = graphic.color;
        color.a = alpha;
        graphic.color = color;
    }

    // Visual debug in Scene view
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, maxDistance);
    }
}