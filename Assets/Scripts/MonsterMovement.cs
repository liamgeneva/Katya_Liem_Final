using UnityEngine;

public class MonsterMovement : MonoBehaviour
{
    public Transform player;
    public float speed = 3f;

    void Update()
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            player.position,
            speed * Time.deltaTime
        );
    }
}
