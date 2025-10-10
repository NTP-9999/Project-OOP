using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("Target")]
    public Transform target; // usually the player
    public Vector3 offset = new(0f, 2f, -4f);

    [Header("Settings")]
    public float rotationSpeed = 3f;
    public float pitchMin = -20f;
    public float pitchMax = 60f;
    public float smoothFollow = 0.1f;

    [Header("Collision")]
    public LayerMask collisionMask;
    public float collisionRadius = 0.3f;
    public float minDistance = 0.5f;
    public float groundOffset = 0.2f; // ป้องกันกล้องจมพื้น

    private float yaw;
    private float pitch;
    private Vector3 camVelocity;

    [SerializeField] private Transform cameraPivot; // drag CameraPivot here
    private Transform cam;

    void Awake()
    {
        cam = Camera.main.transform;
    }

    void Start()
    {
        if (target == null && Player.Instance != null)
            target = Player.Instance.transform;
    }

    void LateUpdate()
    {
        if (target == null || cam == null || cameraPivot == null) return;

        // --- Input ---
        float mouseX = Input.GetAxis("Mouse X") * rotationSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed;

        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, pitchMin, pitchMax);

        // --- Rotate pivot ---
        cameraPivot.rotation = Quaternion.Euler(pitch, yaw, 0f);
        cameraPivot.position = Vector3.Lerp(cameraPivot.position, target.position, smoothFollow);

        // --- Desired camera position ---
        Vector3 desiredPos = cameraPivot.position + cameraPivot.rotation * offset;

        // --- Collision check ---
        Vector3 direction = (desiredPos - cameraPivot.position).normalized;
        float distance = offset.magnitude;

        if (Physics.SphereCast(cameraPivot.position, collisionRadius, direction, out RaycastHit hit, distance, collisionMask, QueryTriggerInteraction.Ignore))
        {
            float hitDist = Mathf.Max(hit.distance - minDistance, 0.05f);
            desiredPos = cameraPivot.position + direction * hitDist;
        }

        // --- ป้องกันกล้องจมพื้น ---
        if (desiredPos.y < target.position.y + groundOffset)
        {
            desiredPos.y = target.position.y + groundOffset;
        }

        // --- Smooth follow ---
        cam.position = Vector3.SmoothDamp(cam.position, desiredPos, ref camVelocity, smoothFollow);
        cam.rotation = Quaternion.Lerp(cam.rotation, cameraPivot.rotation, Time.deltaTime * 15f);
    }
}
