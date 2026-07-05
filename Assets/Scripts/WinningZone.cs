using UnityEngine;

public class WinningZone : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        Debug.Log("You Won! The player reached the winning zone!");
        other.gameObject.SetActive(false);
    }
}
