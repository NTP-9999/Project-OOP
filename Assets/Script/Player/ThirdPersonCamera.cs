using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("Target")]
    public Transform target; // usually the player
    public Vector3 offset = new Vector3(0f, 2f, -4f);

    [Header("Settings")]
    public float rotationSpeed = 3f;
    public float pitchMin = -20f;
    public float pitchMax = 60f;
    public float smoothFollow = 0.1f;

    [Header("Collision")]
    public LayerMask collisionMask;
    public float collisionRadius = 0.3f;
    public float minDistance = 0.5f;

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

        // --- Camera offset ---
        Vector3 desiredPos = cameraPivot.position + cameraPivot.rotation * offset;

        // --- Collision ---
        if (Physics.SphereCast(cameraPivot.position, collisionRadius,
            (desiredPos - cameraPivot.position).normalized, out RaycastHit hit, offset.magnitude, collisionMask))
        {
            desiredPos = cameraPivot.position + (desiredPos - cameraPivot.position).normalized * (hit.distance - minDistance);
        }

        cam.position = Vector3.SmoothDamp(cam.position, desiredPos, ref camVelocity, smoothFollow);
        cam.rotation = cameraPivot.rotation;
    }
}
