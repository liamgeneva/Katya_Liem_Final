using UnityEngine;
using System.Collections.Generic;

public class MonsterMovement : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Drag the Player object here")]
    public Transform player;

    [Tooltip("The LifeSystem on the Player")]
    public LifeSystem playerLifeSystem;

    [Header("Movement")]
    [Tooltip("How fast the enemy moves")]
    public float speed = 0.8f;

    [Tooltip("How many units behind the player the enemy stays on the trail")]
    public float trailDistance = 3f;

    [Tooltip("Record a new trail point every N units of player movement")]
    public float recordStep = 0.3f;

    [Header("Contact")]
    [Tooltip("How close the enemy must get to count as a hit")]
    public float touchDistance = 0.8f;

    [Tooltip("Seconds between hits")]
    public float hitCooldown = 1f;

    [Tooltip("Seconds after scene load before the enemy can damage the player")]
    public float startupGrace = 2f;

    // ── Private ───────────────────────────────────────────────────────────
    private List<Vector3> trail = new List<Vector3>();
    private Vector3 lastRecordedPos;

    private Vector3 startPosition;
    private Quaternion startRotation;
    private float lastHitTime = -999f;
    private bool isFrozen = false;
    private float startTime;

    // ── Init ──────────────────────────────────────────────────────────────
    void Start()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;
        startTime = Time.time;
        InitTrail();
    }

    void InitTrail()
    {
        trail.Clear();
        if (player != null)
        {
            lastRecordedPos = player.position;
            // Pre-fill the trail starting at the player so the enemy
            // begins following right from the start
            trail.Add(player.position);
        }
    }

    // ── Main loop ─────────────────────────────────────────────────────────
    void Update()
    {
        if (isFrozen || player == null) return;

        RecordTrail();
        FollowTrail();
        CheckContact();
    }

    // ── Record the player's exact path as breadcrumbs ─────────────────────
    void RecordTrail()
    {
        if (Vector3.Distance(player.position, lastRecordedPos) < recordStep) return;

        trail.Add(player.position);
        lastRecordedPos = player.position;
        TrimOldPoints();
    }

    // ── Move along the recorded trail (stays in corridors automatically) ──
    void FollowTrail()
    {
        // Measure total trail length
        float totalLength = 0f;
        for (int i = trail.Count - 1; i > 0; i--)
            totalLength += Vector3.Distance(trail[i], trail[i - 1]);

        // Wait until the player is far enough ahead before chasing
        if (totalLength < trailDistance) return;

        // Walk backwards along the trail to find the target point
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

        // Move toward the target point
        Vector3 prevPos = transform.position;
        transform.position = Vector3.MoveTowards(
            transform.position,
            new Vector3(targetPos.x, transform.position.y, targetPos.z),
            speed * Time.deltaTime
        );

        // Rotate to face the direction of movement
        Vector3 moved = transform.position - prevPos;
        if (moved.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(moved.normalized);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation, targetRot, 480f * Time.deltaTime);
        }
    }

    // ── Remove old points the enemy has already passed ────────────────────
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

    // ── Check if the enemy has caught the player ──────────────────────────
    void CheckContact()
    {
        if (Time.time - startTime < startupGrace) return;

        float dist = Vector3.Distance(transform.position, player.position);
        if (dist <= touchDistance && Time.time - lastHitTime >= hitCooldown)
        {
            lastHitTime = Time.time;
            Debug.Log("Enemy caught the player! Losing a life.");
            if (playerLifeSystem != null)
                playerLifeSystem.TakeDamage();
        }
    }

    // ── Freeze before scene reload ────────────────────────────────────────
    public void Freeze()
    {
        isFrozen = true;
    }

    // ── Respawn at starting position ──────────────────────────────────────
    public void Respawn()
    {
        isFrozen = false;
        lastHitTime = -999f;
        transform.position = startPosition;
        transform.rotation = startRotation;
        InitTrail();
    }
}
