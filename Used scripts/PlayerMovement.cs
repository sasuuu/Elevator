using UnityEngine;

public class PlayerMovement : MonoBehaviour{

    [Range(0.1f, 100f)] public float mouseSensitivity;
    new GameObject camera;
    float verticalSpeed = 2f;
    float horizontalSpeed = 1f;
    float verticalMovement = 0f;
    float horizontalMovement = 0f;
    float mouseVertical = 0.0f;
    float mouseHorizontal = 0.0f;
    float maxMouseVertical = 90.0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        camera = GetComponentInChildren<Camera>().gameObject;
    }

    void Update()
    {
        Move();
        Mouse();
    }

    void Move()
    {
        verticalMovement = Input.GetAxis("Vertical") * verticalSpeed;
        horizontalMovement = Input.GetAxis("Horizontal") * horizontalSpeed;
        Vector3 move = new Vector3(horizontalMovement, 0, verticalMovement);
        transform.Translate(move * Time.deltaTime, Space.Self);
    }

    void Mouse()
    {
        mouseHorizontal += Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        transform.localRotation = Quaternion.Euler(0, mouseHorizontal, 0);

        mouseVertical -= Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        mouseVertical = Mathf.Clamp(mouseVertical, -maxMouseVertical, maxMouseVertical);

        camera.transform.localRotation = Quaternion.Euler(mouseVertical, 0, 0);
    }
}
