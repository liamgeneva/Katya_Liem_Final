using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LifeSystem : MonoBehaviour
{
    [Tooltip("Drag your lives ScriptableObject asset here")]
    public lives playerLives;

    [Tooltip("Drag the Enemy object here")]
    public MonsterMovement enemy;

    // Cooldown flag — prevents double-triggering on the same death
    private bool isRespawning = false;

    void Start()
    {
        if (playerLives == null)
            Debug.LogWarning("LifeSystem: Assign the lives ScriptableObject in the Inspector!", this);
    }

    public void TakeDamage()
    {
        if (isRespawning) return;
        if (playerLives == null) return;

        isRespawning = true;

        // Freeze the player immediately so it stops moving
        SimpleSphereMove playerMove = GetComponent<SimpleSphereMove>();
        if (playerMove != null) playerMove.enabled = false;

        CharacterController cc = GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        // Freeze the enemy immediately so it stops moving
        if (enemy != null) enemy.Freeze();

        playerLives.LoseLife();

        if (playerLives.IsGameOver)
            playerLives.ResetLives();

        // Short delay so the freeze is visible, then reload
        StartCoroutine(ReloadAfterDelay(0.5f));
    }

    IEnumerator ReloadAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}


