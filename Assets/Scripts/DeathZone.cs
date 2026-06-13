using UnityEngine;

public class DeathZone : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        LifeSystem lives = other.GetComponent<LifeSystem>();
        if (lives != null)
            lives.TakeDamage();
    }
}
