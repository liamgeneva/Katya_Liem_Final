using UnityEngine;
using System.Collections.Generic;

public class MonsterMovement : MonoBehaviour
{
    [Header("Ссылки")]
    [Tooltip("Перетащи сюда объект Player")]
    public Transform player;

    [Header("Настройки")]
    [Tooltip("Скорость монстра")]
    public float speed = 6f;

    [Tooltip("На сколько единиц сзади по пути держится монстр")]
    public float trailDistance = 6f;

    [Tooltip("Записывать новую точку пути каждые N единиц расстояния")]
    public float recordStep = 0.3f;

    // ── Приватные переменные ──────────────────────────────────────────────
    private List<Vector3> trail = new List<Vector3>(); // записанный путь игрока
    private Vector3 lastRecordedPos;

    private SimpleSphereMove playerController;

    // ── Инициализация ─────────────────────────────────────────────────────
    void Start()
    {
        if (player != null)
        {
            playerController = player.GetComponent<SimpleSphereMove>();
            lastRecordedPos = player.position;
            trail.Add(player.position);
        }
    }

    // ── Главный цикл ──────────────────────────────────────────────────────
    void Update()
    {
        if (player == null || trail.Count == 0) return;

        RecordPlayerTrail();
        FollowTrail();
    }

    // ── Запись пути игрока ────────────────────────────────────────────────
    void RecordPlayerTrail()
    {
        // Добавляем точку только если игрок прошёл достаточно далеко
        if (Vector3.Distance(player.position, lastRecordedPos) >= recordStep)
        {
            trail.Add(player.position);
            lastRecordedPos = player.position;

            // Убираем старые точки, которые монстр уже давно прошёл
            TrimOldPoints();
        }
    }

    // ── Движение по записанному пути ──────────────────────────────────────
    void FollowTrail()
    {
        // Ищем точку на пути, которая находится на trailDistance позади игрока
        float remaining = trailDistance;
        Vector3 targetPos = trail[0];

        for (int i = trail.Count - 1; i > 0; i--)
        {
            float segmentLength = Vector3.Distance(trail[i], trail[i - 1]);

            if (remaining <= segmentLength)
            {
                // Точка внутри этого отрезка пути
                float t = remaining / segmentLength;
                targetPos = Vector3.Lerp(trail[i], trail[i - 1], t);
                break;
            }

            remaining -= segmentLength;
        }

        // Двигаемся к целевой точке
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPos,
            speed * Time.deltaTime
        );

        // Поворачиваемся в сторону движения
        Vector3 dir = (targetPos - transform.position).normalized;
        if (dir.sqrMagnitude > 0.01f)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRot,
                360f * Time.deltaTime
            );
        }
    }

    // ── Удаление лишних старых точек ──────────────────────────────────────
    void TrimOldPoints()
    {
        // Оставляем только столько точек, сколько нужно для trailDistance + небольшой запас
        float totalLength = 0f;
        float keepLength = trailDistance + 5f;

        for (int i = trail.Count - 1; i > 0; i--)
        {
            totalLength += Vector3.Distance(trail[i], trail[i - 1]);
            if (totalLength > keepLength)
            {
                // Всё до этого индекса можно удалить
                trail.RemoveRange(0, i);
                break;
            }
        }
    }
}
