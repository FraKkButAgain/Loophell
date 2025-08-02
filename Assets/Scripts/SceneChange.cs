using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            int currentIndex = SceneManager.GetActiveScene().buildIndex;
            int nextIndex = (currentIndex + 1) % SceneManager.sceneCountInBuildSettings;

            // Debug info to check in Console
            Debug.Log($"Current: {currentIndex} | Next: {nextIndex} | Total Scenes: {SceneManager.sceneCountInBuildSettings}");

            SceneManager.LoadScene(nextIndex);
        }
    }
}