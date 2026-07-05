using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class SimpleSphereMove : MonoBehaviour
{
    [Header("Движение")]
    [Tooltip("Максимальная скорость бега")]
    public float runSpeed = 4f;

    [Tooltip("Плавность разгона (меньше = медленнее набирает скорость)")]
    public float acceleration = 2f;

    [Tooltip("Скорость визуального поворота (градусов в секунду)")]
    public float turnSpeed = 600f;

    [Header("Центрирование в коридоре")]
    [Tooltip("Сила притяжения к центру коридора")]
    public float centeringSpeed = 8f;

    [Tooltip("Дальность луча поиска стен — поставь чуть больше половины ширины коридора")]
    public float wallRayLength = 1.5f;

    // ── Приватные переменные ──────────────────────────────────────────────
    private CharacterController cc;
    private Vector3 moveDirection = Vector3.forward;
    private Quaternion targetRotation;
    private float verticalVelocity = 0f;
    private float currentSpeed = 0f;
    private bool isDead = false;

    private TurnPoint currentTurnPoint = null;

    public bool IsDead => isDead;

    // ── Инициализация ─────────────────────────────────────────────────────
    void Awake()
    {
        cc = GetComponent<CharacterController>();
        targetRotation = transform.rotation;
    }

    // ── Главный цикл ──────────────────────────────────────────────────────
    void Update()
    {
        if (isDead) return;

        HandleInput();
        RotateSmooth();
        MoveForward();
        CenterInCorridor();
    }

    // ── Обработка ввода ───────────────────────────────────────────────────
    void HandleInput()
    {
        if (currentTurnPoint == null) return;
        if (Quaternion.Angle(transform.rotation, targetRotation) > 5f) return;

        var kb = Keyboard.current;
        if (kb == null) return;

        if (kb.aKey.wasPressedThisFrame || kb.leftArrowKey.wasPressedThisFrame)
        {
            if (currentTurnPoint.allowLeft)
                ExecuteTurn(-90f);
        }
        else if (kb.dKey.wasPressedThisFrame || kb.rightArrowKey.wasPressedThisFrame)
        {
            if (currentTurnPoint.allowRight)
                ExecuteTurn(90f);
        }
    }

    void ExecuteTurn(float degrees)
    {
        Vector3 snap = currentTurnPoint.transform.position;
        transform.position = new Vector3(snap.x, transform.position.y, snap.z);

        Quaternion turn = Quaternion.Euler(0f, degrees, 0f);
        moveDirection = turn * moveDirection;
        targetRotation = turn * targetRotation;
    }

    // ── Плавный поворот ───────────────────────────────────────────────────
    void RotateSmooth()
    {
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRotation,
            turnSpeed * Time.deltaTime
        );
    }

    // ── Движение + гравитация ─────────────────────────────────────────────
    void MoveForward()
    {
        if (cc.isGrounded)
            verticalVelocity = -2f;
        else
            verticalVelocity -= 9.81f * Time.deltaTime;

        currentSpeed = Mathf.Lerp(currentSpeed, runSpeed, acceleration * Time.deltaTime);

        Vector3 velocity = moveDirection * currentSpeed;
        velocity.y = verticalVelocity;
        cc.Move(velocity * Time.deltaTime);
    }

    // ── Центрирование в коридоре ──────────────────────────────────────────
    void CenterInCorridor()
    {
        Vector3 right = Vector3.Cross(Vector3.up, moveDirection).normalized;
        Vector3 origin = transform.position + Vector3.up * 0.5f;

        RaycastHit leftHit, rightHit;
        bool hasLeft  = Physics.Raycast(origin, -right, out leftHit,  wallRayLength);
        bool hasRight = Physics.Raycast(origin,  right, out rightHit, wallRayLength);

        if (!hasLeft || !hasRight) return;

        Vector3 midPoint = (leftHit.point + rightHit.point) * 0.5f;
        float lateralOffset = Vector3.Dot(midPoint - transform.position, right);
        transform.position += right * lateralOffset * centeringSpeed * Time.deltaTime;
    }

    // ── Вход / выход из точки поворота ───────────────────────────────────
    void OnTriggerEnter(Collider other)
    {
        TurnPoint tp = other.GetComponent<TurnPoint>();
        if (tp != null)
            currentTurnPoint = tp;
    }

    void OnTriggerExit(Collider other)
    {
        TurnPoint tp = other.GetComponent<TurnPoint>();
        if (tp != null && currentTurnPoint == tp)
            currentTurnPoint = null;
    }

    // ── Столкновение с препятствием ───────────────────────────────────────
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Obstacle"))
            Die();
    }

    // ── Смерть ────────────────────────────────────────────────────────────
    public void Die()
    {
        if (isDead) return;
        isDead = true;
        GetComponent<LifeSystem>()?.TakeDamage();
    }

    // ── Сброс после respawn ───────────────────────────────────────────────
    public void ResetPlayer()
    {
        isDead = true;
        currentSpeed = 0f;
        verticalVelocity = 0f;
        moveDirection = transform.forward;
        targetRotation = transform.rotation;
        currentTurnPoint = null;

        CancelInvoke(nameof(StartMoving));
        Invoke(nameof(StartMoving), 2f);
    }

    void StartMoving()
    {
        isDead = false;
        currentSpeed = runSpeed;
    }
}
