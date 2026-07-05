using UnityEngine;

public class LifeSystem : MonoBehaviour
{
    [Tooltip("Number of lives at the start")]
    public int maxLives = 3;

    [Tooltip("Drag the Enemy object here")]
    public MonsterMovement enemy;

    private int currentLives;
    private Vector3 startPosition;
    private Quaternion startRotation;
    private SimpleSphereMove playerMove;
    private CharacterController cc;

    // Cooldown flag — prevents double-triggering right after respawn
    private bool isRespawning = false;

    void Start()
    {
        currentLives = maxLives;
        startPosition = transform.position;
        startRotation = transform.rotation;
        playerMove = GetComponent<SimpleSphereMove>();
        cc = GetComponent<CharacterController>();
    }

    public void TakeDamage()
    {
        if (isRespawning) return;

        currentLives--;

        if (currentLives <= 0)
        {
            Debug.Log("Game Over");
            gameObject.SetActive(false);
            return;
        }

        Debug.Log(currentLives == 1 ? "1 life left" : currentLives + " lives left");
        Respawn();
    }

    void Respawn()
    {
        isRespawning = true;

        // Teleport player — CharacterController must be disabled to change position
        cc.enabled = false;
        transform.position = startPosition;
        transform.rotation = startRotation;
        cc.enabled = true;

        playerMove.ResetPlayer();

        if (enemy != null)
            enemy.Respawn();

        // Allow damage again after a short delay so the player can move away from the zone
        Invoke(nameof(ClearRespawn), 0.5f);
    }

    void ClearRespawn() => isRespawning = false;
}
