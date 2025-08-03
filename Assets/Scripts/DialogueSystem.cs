using UnityEngine;
using TMPro;

[RequireComponent(typeof(Collider2D))]
public class SpeechBubbleSystem : MonoBehaviour
{
    [Header("Speech Bubble Settings")]
    [SerializeField] private Canvas speechBubbleCanvas;
    [SerializeField] private TMP_Text speechText;
    [SerializeField] private Vector2 bubbleOffset = new Vector2(0, 1.5f); // Changed to Vector2
    [SerializeField] private float interactionDistance = 3f;
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    [Header("Dialogue Content")]
    [TextArea(3, 5)][SerializeField] private string[] dialogueLines;

    private Transform player;
    private int currentLine = 0;
    private bool initialized = false;

    void Start()
    {
        Initialize();
    }

    void Initialize()
    {
        if (speechBubbleCanvas == null || speechText == null)
        {
            Debug.LogError("UI components not assigned!", this);
            enabled = false;
            return;
        }

        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null)
        {
            Debug.LogError("Player not found! Tag your player object.", this);
            enabled = false;
            return;
        }

        // Configure canvas
        speechBubbleCanvas.renderMode = RenderMode.WorldSpace;
        speechBubbleCanvas.worldCamera = Camera.main;
        speechBubbleCanvas.gameObject.SetActive(false);

        initialized = true;
    }

    void Update()
    {
        if (!initialized) return;

        // Pure 2D position calculations
        Vector2 npcPos = transform.position;
        Vector2 playerPos = player.position;
        float distance = Vector2.Distance(npcPos, playerPos);

        bool inRange = distance <= interactionDistance;
        speechBubbleCanvas.gameObject.SetActive(inRange);

        if (inRange)
        {
            // Set position using pure 2D coordinates + offset
            Vector3 bubblePos = new Vector3(
                transform.position.x + bubbleOffset.x,
                transform.position.y + bubbleOffset.y,
                transform.position.z // Maintain original Z for rendering order
            );
            speechBubbleCanvas.transform.position = bubblePos;
            speechBubbleCanvas.transform.rotation = Camera.main.transform.rotation;

            if (Input.GetKeyDown(interactKey))
            {
                PlayNextLine();
            }
        }
    }

    void PlayNextLine()
    {
        if (dialogueLines.Length == 0) return;

        speechText.text = dialogueLines[currentLine];
        currentLine = (currentLine + 1) % dialogueLines.Length;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
        Gizmos.DrawSphere((Vector2)transform.position + bubbleOffset, 0.1f);
    }
}