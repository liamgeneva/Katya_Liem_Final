using UnityEngine;

public class DeathZone : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        LifeSystem lifeSystem = other.GetComponent<LifeSystem>();
        if (lifeSystem != null)
            lifeSystem.TakeDamage();
    }
}

