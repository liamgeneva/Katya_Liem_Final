using UnityEngine;

public class DeathZone : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        Debug.Log("Game Over! The player fell into the death zone.");
        other.gameObject.SetActive(false);
    }
}
