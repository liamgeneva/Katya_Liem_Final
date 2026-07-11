using UnityEngine;
using UnityEngine.SceneManagement;

public class WinningZone : MonoBehaviour
{
    [Tooltip("Drag your lives ScriptableObject here so lives reset after the last level")]
    public lives playerLives;

    [Tooltip("Drag the LevelCompleteUI component here (from the Canvas in this scene)")]
    public LevelCompleteUI levelCompleteUI;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        int nextIndex = currentIndex + 1;

        // Index 0 = main menu, 1 = FirstLevel, 2 = SecondLevel, 3 = ThirdLevel
        int totalLevels = 4;

        if (nextIndex >= totalLevels)
        {
            // Last level completed — reset lives and go to main menu
            if (playerLives != null) playerLives.ResetLives();
            nextIndex = 0; // main menu
        }

        // Find the LevelCompleteUI automatically if not set in the Inspector
        if (levelCompleteUI == null)
        {
            // Try modern Unity API first
            levelCompleteUI = FindFirstObjectByType<LevelCompleteUI>();
            
            // Fall back to older Unity API if modern isn't available/found
            if (levelCompleteUI == null)
            {
#pragma warning disable CS0618
                levelCompleteUI = FindObjectOfType<LevelCompleteUI>();
#pragma warning restore CS0618
            }
        }

        // Show the Level Complete panel if it exists, otherwise load directly
        if (levelCompleteUI != null)
        {
            levelCompleteUI.ShowPanel(nextIndex);
        }
        else
        {
            SceneManager.LoadScene(nextIndex);
        }
    }
}


