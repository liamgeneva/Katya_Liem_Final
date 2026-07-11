using UnityEngine;

[CreateAssetMenu(fileName = "lives", menuName = "Scriptable Objects/lives")]
public class lives : ScriptableObject
{
    [Tooltip("How many lives the player starts with")]
    public int maxLives = 3;

    // static = survives scene reloads. -1 means "not started yet".
    private static int _currentLives = -1;

    // Any script can read the current lives without needing an Inspector reference
    public static int Current => _currentLives < 0 ? 3 : _currentLives;

    public int currentLives => _currentLives;

    // Called by Unity each time the scene loads.
    // Only resets lives on the very first load (when _currentLives is -1).
    private void OnEnable()
    {
        if (_currentLives < 0)
            _currentLives = maxLives;
    }

    // Is the player out of lives?
    public bool IsGameOver => _currentLives <= 0;

    // Call this whenever the player gets hit
    public void LoseLife()
    {
        if (IsGameOver) return;

        _currentLives--;

        if (IsGameOver)
            Debug.Log("Game Over! No lives remaining.");
        else
            Debug.Log("Life lost! Lives remaining: " + _currentLives);
    }

    // Fully resets lives (call this on a proper Game Over before reloading)
    public void ResetLives()
    {
        _currentLives = maxLives;
    }
}

