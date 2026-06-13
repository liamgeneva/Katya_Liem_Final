using UnityEngine;

public class SimpleCameraController : MonoBehaviour
{
    [Tooltip("Перетащи сюда объект Player (или найдётся автоматически)")]
    public Transform target;

    [Tooltip("Плавность движения камеры")]
    public float smoothSpeed = 8f;

    [Tooltip("Плавность поворота камеры")]
    public float rotationSmoothSpeed = 6f;

    // Смещение и поворот рассчитываются один раз из текущей позиции в сцене
    private Vector3 localOffset;
    private Quaternion rotationOffset;

    void Start()
    {
        if (target == null)
        {
            GameObject obj = GameObject.Find("Player");
            if (obj != null) target = obj.transform;
        }

        if (target == null)
        {
            Debug.LogWarning("CameraController: объект Player не найден.");
            return;
        }

        // Сохраняем текущее смещение камеры относительно игрока в его локальном пространстве.
        // Это значит: где бы камера ни стояла в сцене — именно это расстояние и угол сохранятся.
        localOffset = Quaternion.Inverse(target.rotation) * (transform.position - target.position);
        rotationOffset = Quaternion.Inverse(target.rotation) * transform.rotation;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Желаемая позиция = позиция игрока + смещение, повёрнутое вместе с игроком
        Vector3 desiredPosition = target.position + target.rotation * localOffset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        // Желаемый поворот = поворот игрока + исходный угол камеры относительно него
        Quaternion desiredRotation = target.rotation * rotationOffset;
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, rotationSmoothSpeed * Time.deltaTime);
    }
}
