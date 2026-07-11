using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelCompleteUI : MonoBehaviour
{
    [Tooltip("Drag the Panel GameObject here (the popup with text and button)")]
    public GameObject panel;

    private int nextSceneIndex;

    void Start()
    {
        // Make sure the panel is hidden at the start of each level
        if (panel != null)
            panel.SetActive(false);
    }

    // Called by WinningZone when the player touches it
    public void ShowPanel(int nextIndex)
    {
        nextSceneIndex = nextIndex;

        if (panel != null)
            panel.SetActive(true);

        // Freeze time so the player stops moving
        Time.timeScale = 0f;
    }

    // Attach this to the Continue button's OnClick event
    public void OnContinuePressed()
    {
        // Unfreeze time before loading
        Time.timeScale = 1f;
        SceneManager.LoadScene(nextSceneIndex);
    }
}
