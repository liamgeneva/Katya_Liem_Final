using UnityEngine;
using UnityEngine.SceneManagement;

public class WinningZone : MonoBehaviour
{
    [Tooltip("Drag your lives ScriptableObject here so lives reset when going back to level 1")]
    public lives playerLives;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        int nextIndex = currentIndex + 1;

        // Total number of playable levels (FirstLevel=0, SecondLevel=1, ThirdLevel=2)
        int totalLevels = 3;

        if (nextIndex >= totalLevels)
        {
            // Completed all levels — reset lives and go back to level 1
            Debug.Log("You completed all levels! Going back to Level 1.");
            if (playerLives != null) playerLives.ResetLives();
            SceneManager.LoadScene(0);
        }
        else
        {
            Debug.Log("Level complete! Loading next level...");
            SceneManager.LoadScene(nextIndex);
        }
    }
}

