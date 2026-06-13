using UnityEngine;

public class WinningZone : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            Debug.Log("You won!");
    }
}
