using UnityEngine;

// Повесь этот скрипт на пустые объекты в углах лабиринта.
// Добавь BoxCollider с галочкой "Is Trigger".
// Размер коллайдера = ширина коридора.
public class TurnPoint : MonoBehaviour
{
    [Tooltip("Игрок может повернуть налево здесь")]
    public bool allowLeft = true;

    [Tooltip("Игрок может повернуть направо здесь")]
    public bool allowRight = true;

    // Визуализация в редакторе — жёлтый куб в Scene View
    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0.9f, 0f, 0.4f);
        Gizmos.DrawCube(transform.position, GetComponent<Collider>() != null
            ? GetComponent<Collider>().bounds.size
            : Vector3.one);

        // Стрелки — какие повороты разрешены
        Gizmos.color = Color.cyan;
        if (allowLeft)
            Gizmos.DrawRay(transform.position, -transform.right * 1.5f);
        if (allowRight)
            Gizmos.DrawRay(transform.position, transform.right * 1.5f);
    }
}
