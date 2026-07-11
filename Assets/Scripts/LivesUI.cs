using UnityEngine;
using UnityEngine.UI;

public class LivesUI : MonoBehaviour
{
    [Tooltip("Drag the 3 heart Image objects here (in order: heart1, heart2, heart3)")]
    public Image[] heartImages;

    void Start()
    {
        UpdateHearts();
    }

    void UpdateHearts()
    {
        if (heartImages == null) return;

        // Uses the static Current property directly from the lives class
        int currentLives = lives.Current;

        for (int i = 0; i < heartImages.Length; i++)
        {
            if (heartImages[i] != null)
                // Show the heart if its index is within the current lives count
                heartImages[i].gameObject.SetActive(i < currentLives);
        }
    }
}
