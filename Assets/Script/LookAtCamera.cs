using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        // ถ้าไม่ได้ตั้งค่า Camera ให้ใช้ Main Camera
        if (mainCamera == null)
            mainCamera = Camera.main;

        // หมุน Canvas ให้หันไปทางกล้อง
        transform.LookAt(mainCamera.transform);

        // หมุน 180 องศา ถ้า Canvas กลับหัว
        transform.Rotate(0, 180, 0);
    }
}
