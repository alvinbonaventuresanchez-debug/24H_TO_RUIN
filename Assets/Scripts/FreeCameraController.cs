using UnityEngine;
using UnityEngine.InputSystem;

public class FreeCameraController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float lookSensitivity = 0.12f;
    [SerializeField] private float maxPitch = 80f;
    [SerializeField] private bool lockCursorOnStart = true;

    private float yaw;
    private float pitch;

    private void Start()
    {
        Vector3 currentRotation = transform.eulerAngles;
        yaw = currentRotation.y;
        pitch = NormalizeAngle(currentRotation.x);

        if (lockCursorOnStart)
        {
            LockCursor();
        }
    }

    private void Update()
    {
        HandleCursorState();
        HandleLook();
        HandleMovement();
    }

    private void HandleCursorState()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            UnlockCursor();
        }

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            LockCursor();
        }
    }

    private void HandleLook()
    {
        if (Mouse.current == null || Cursor.lockState != CursorLockMode.Locked)
        {
            return;
        }

        Vector2 mouseDelta = Mouse.current.delta.ReadValue();
        yaw += mouseDelta.x * lookSensitivity;
        pitch -= mouseDelta.y * lookSensitivity;
        pitch = Mathf.Clamp(pitch, -maxPitch, maxPitch);

        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }

    private void HandleMovement()
    {
        if (Keyboard.current == null)
        {
            return;
        }

        Vector3 input = Vector3.zero;

        if (Keyboard.current.upArrowKey.isPressed)
        {
            input += Vector3.forward;
        }

        if (Keyboard.current.downArrowKey.isPressed)
        {
            input += Vector3.back;
        }

        if (Keyboard.current.leftArrowKey.isPressed)
        {
            input += Vector3.left;
        }

        if (Keyboard.current.rightArrowKey.isPressed)
        {
            input += Vector3.right;
        }

        if (input.sqrMagnitude > 1f)
        {
            input.Normalize();
        }

        Vector3 flatForward = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
        Vector3 flatRight = Vector3.ProjectOnPlane(transform.right, Vector3.up).normalized;
        Vector3 movement = (flatForward * input.z) + (flatRight * input.x);

        transform.position += movement * moveSpeed * Time.deltaTime;
    }

    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private static float NormalizeAngle(float angle)
    {
        if (angle > 180f)
        {
            angle -= 360f;
        }

        return angle;
    }
}
