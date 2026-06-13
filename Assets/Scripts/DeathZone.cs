using UnityEngine;

public class DeathZone : MonoBehaviour
{
    void OnTriggerEnter(Collider other) => Kill(other);
    void OnTriggerStay(Collider other)  => Kill(other);

    void Kill(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        Debug.Log("You died!");
        other.gameObject.SetActive(false);
        Destroy(other.gameObject);
    }
}
