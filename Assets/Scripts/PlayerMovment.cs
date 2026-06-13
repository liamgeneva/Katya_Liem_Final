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
    private float currentSpeed = 0f;   // текущая скорость плавно нарастает до runSpeed
    private bool isDead = false;

    // Текущая точка поворота (null = игрок не в углу лабиринта)
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
        // Повернуть можно только стоя в точке поворота
        if (currentTurnPoint == null) return;

        // Не принимать новый поворот, пока предыдущий не завершён
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

    // Поворот + прижатие к центру коридора (чтобы не съехать в стену)
    void ExecuteTurn(float degrees)
    {
        // Снапаем позицию к центру точки поворота (по X и Z)
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

        // Плавный разгон до runSpeed
        currentSpeed = Mathf.Lerp(currentSpeed, runSpeed, acceleration * Time.deltaTime);

        Vector3 velocity = moveDirection * currentSpeed;
        velocity.y = verticalVelocity;
        cc.Move(velocity * Time.deltaTime);
    }

    // ── Центрирование в коридоре ──────────────────────────────────────────
    void CenterInCorridor()
    {
        // Перпендикуляр к направлению движения — это и есть "боковая" ось коридора
        Vector3 right = Vector3.Cross(Vector3.up, moveDirection).normalized;

        // Лучи пускаем с высоты пояса, чтобы не цеплять пол/потолок
        Vector3 origin = transform.position + Vector3.up * 0.5f;

        RaycastHit leftHit, rightHit;
        bool hasLeft  = Physics.Raycast(origin, -right, out leftHit,  wallRayLength);
        bool hasRight = Physics.Raycast(origin,  right, out rightHit, wallRayLength);

        if (!hasLeft || !hasRight) return;

        // Середина между двумя стенами
        Vector3 midPoint = (leftHit.point + rightHit.point) * 0.5f;

        // Смещение только по боковой оси (не трогаем направление движения и высоту)
        float lateralOffset = Vector3.Dot(midPoint - transform.position, right);
        transform.position += right * lateralOffset * centeringSpeed * Time.deltaTime;
    }

    // ── Вход в точку поворота ─────────────────────────────────────────────
    void OnTriggerEnter(Collider other)
    {
        TurnPoint tp = other.GetComponent<TurnPoint>();
        if (tp != null)
            currentTurnPoint = tp;
    }

    // ── Выход из точки поворота ───────────────────────────────────────────
    void OnTriggerExit(Collider other)
    {
        TurnPoint tp = other.GetComponent<TurnPoint>();
        if (tp != null && currentTurnPoint == tp)
            currentTurnPoint = null;
    }

    // ── Столкновение со стеной/препятствием ──────────────────────────────
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
        Debug.Log("You died!");
        gameObject.SetActive(false);
        Destroy(gameObject);
    }
}
