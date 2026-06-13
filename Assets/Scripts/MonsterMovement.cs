using UnityEngine;
using System.Collections.Generic;

public class MonsterMovement : MonoBehaviour
{
    [Header("References")]
    public Transform player;

    [Header("Settings")]
    public float speed = 6f;

    [Tooltip("How many units behind the player the monster stays on the trail")]
    public float trailDistance = 6f;

    [Tooltip("Record a new trail point every N units")]
    public float recordStep = 0.3f;

    // ── Private ───────────────────────────────────────────────────────────
    private List<Vector3> trail = new List<Vector3>();
    private Vector3 lastRecordedPos;

    private Vector3 startPosition;
    private Quaternion startRotation;

    // ── Init ──────────────────────────────────────────────────────────────
    void Start()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;
        InitTrail();
    }

    void InitTrail()
    {
        trail.Clear();
        if (player != null)
        {
            lastRecordedPos = player.position;
            trail.Add(player.position);
        }
    }

    public void Respawn()
    {
        transform.position = startPosition;
        transform.rotation = startRotation;
        InitTrail();
    }

    // ── Main loop ─────────────────────────────────────────────────────────
    void Update()
    {
        if (player == null) return;

        RecordTrail();
        FollowTrail();
    }

    // ── Record player path ────────────────────────────────────────────────
    void RecordTrail()
    {
        if (Vector3.Distance(player.position, lastRecordedPos) < recordStep) return;

        trail.Add(player.position);
        lastRecordedPos = player.position;
        TrimOldPoints();
    }

    // ── Follow the recorded path ──────────────────────────────────────────
    void FollowTrail()
    {
        // Measure total trail length
        float totalLength = 0f;
        for (int i = trail.Count - 1; i > 0; i--)
            totalLength += Vector3.Distance(trail[i], trail[i - 1]);

        // Stay still until the player is far enough ahead on the trail.
        // This prevents the monster from oscillating at the start or after respawn.
        if (totalLength < trailDistance) return;

        // Walk backwards along the trail to find the point at trailDistance
        float remaining = trailDistance;
        Vector3 targetPos = trail[0];

        for (int i = trail.Count - 1; i > 0; i--)
        {
            float seg = Vector3.Distance(trail[i], trail[i - 1]);
            if (remaining <= seg)
            {
                targetPos = Vector3.Lerp(trail[i], trail[i - 1], remaining / seg);
                break;
            }
            remaining -= seg;
        }

        // Move toward target
        Vector3 prevPos = transform.position;
        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

        // Rotate based on actual movement direction, not the target vector
        // (prevents spinning when already at the target point)
        Vector3 moved = transform.position - prevPos;
        if (moved.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(moved.normalized);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, 480f * Time.deltaTime);
        }
    }

    // ── Trim points the monster has already passed ────────────────────────
    void TrimOldPoints()
    {
        float totalLength = 0f;
        float keepLength = trailDistance + 4f;

        for (int i = trail.Count - 1; i > 0; i--)
        {
            totalLength += Vector3.Distance(trail[i], trail[i - 1]);
            if (totalLength > keepLength)
            {
                trail.RemoveRange(0, i);
                break;
            }
        }
    }
}
