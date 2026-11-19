using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public float lookSensitivity = 0.1f;
    public float moveSpeed = 1.0f;
    float yaw = 0.0f;
    float pitch = 0.0f;

    void Start()
    {
        yaw = transform.eulerAngles.y;
        pitch = transform.eulerAngles.x;
    }
    void Update()
    {
        HandleMovement();

        if(Keyboard.current != null && Keyboard.current.escapeKey.isPressed) Application.Quit();
    }

    void HandleMovement()
    {
        if (Mouse.current != null && !Mouse.current.rightButton.isPressed) return;

        Vector2 mouseDelta = Mouse.current.delta.ReadValue();
        yaw += mouseDelta.x * lookSensitivity;
        pitch = Mathf.Clamp(pitch - mouseDelta.y * lookSensitivity, -89f, 89f);

        transform.rotation = Quaternion.Euler(pitch,yaw,0f);

        float x = (Keyboard.current != null && Keyboard.current.dKey.isPressed ? 1f : 0f) - (Keyboard.current.aKey.isPressed ? 1f : 0f);
        float z = (Keyboard.current != null && Keyboard.current.wKey.isPressed ? 1f : 0f) - (Keyboard.current.sKey.isPressed ? 1f : 0f);
        float y = (Keyboard.current != null && Keyboard.current.eKey.isPressed ? 1f : 0f) - (Keyboard.current.qKey.isPressed ? 1f : 0f);

        Vector3 moveVector = (transform.right * x + transform.forward * z + Vector3.up * y).normalized;

        float currentSpeed = moveSpeed * (Keyboard.current != null && Keyboard.current.leftShiftKey.isPressed ? 2.5f : 1f);

        transform.position += moveVector * currentSpeed * Time.deltaTime;
    }

}