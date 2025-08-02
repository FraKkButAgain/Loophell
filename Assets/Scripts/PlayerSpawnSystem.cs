using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawnSystem : MonoBehaviour
{
    public static PlayerSpawnSystem Instance;
    private Vector3 currentSpawnPosition;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
            currentSpawnPosition = Vector3.zero;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UpdateSpawnPosition();
        PositionPlayer();
    }

    public void UpdateSpawnPosition()
    {
        GameObject spawnPoint = GameObject.FindWithTag("SpawnPoint");
        if (spawnPoint != null)
        {
            currentSpawnPosition = spawnPoint.transform.position;
        }
    }

    public void PositionPlayer()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            player.transform.position = currentSpawnPosition;
            player.SetActive(true);
        }
    }

    // This is the critical missing method
    public void SetSpawnPosition(Vector3 newPosition)
    {
        currentSpawnPosition = newPosition;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}