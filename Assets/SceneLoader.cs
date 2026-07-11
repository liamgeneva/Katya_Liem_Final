using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // Call this from the Play button's OnClick event in the Inspector
    public void LoadFirstLevel()
    {
        // Index 0 = main menu, Index 1 = FirstLevel
        SceneManager.LoadScene(1);
    }
}

