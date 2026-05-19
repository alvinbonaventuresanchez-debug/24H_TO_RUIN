using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class FreeCameraController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 4.5f;
    [SerializeField] private float sprintMultiplier = 1.7f;
    [SerializeField] private float lookSensitivity = 0.12f;
    [SerializeField] private float maxPitch = 80f;
    [SerializeField] private bool lockCursorOnStart = true;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float gravity = -20f;
    [SerializeField] private float groundedForce = -2f;

    private float yaw;
    private float pitch;
    private float verticalVelocity;
    private CharacterController characterController;

    public float MovementBlend { get; private set; }
    public float SprintBlend { get; private set; }

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();

        if (cameraTransform == null)
        {
            Camera childCamera = GetComponentInChildren<Camera>();

            if (childCamera != null)
            {
                cameraTransform = childCamera.transform;
            }
        }
    }

    private void Start()
    {
        yaw = NormalizeAngle(transform.eulerAngles.y);

        if (cameraTransform != null && cameraTransform != transform)
        {
            pitch = NormalizeAngle(cameraTransform.localEulerAngles.x);
        }
        else
        {
            pitch = NormalizeAngle(transform.eulerAngles.x);
        }

        ApplyLookRotation();

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

        ApplyLookRotation();
    }

    private void HandleMovement()
    {
        if (Keyboard.current == null)
        {
            MovementBlend = Mathf.MoveTowards(MovementBlend, 0f, 8f * Time.deltaTime);
            SprintBlend = Mathf.MoveTowards(SprintBlend, 0f, 10f * Time.deltaTime);
            return;
        }

        Vector2 input = Vector2.zero;

        if (IsForwardPressed())
        {
            input.y += 1f;
        }

        if (IsBackwardPressed())
        {
            input.y -= 1f;
        }

        if (IsLeftPressed())
        {
            input.x -= 1f;
        }

        if (IsRightPressed())
        {
            input.x += 1f;
        }

        bool hasMovementInput = input.sqrMagnitude > 0f;
        bool isSprinting = hasMovementInput && IsSprintPressed();

        MovementBlend = Mathf.MoveTowards(MovementBlend, hasMovementInput ? 1f : 0f, 8f * Time.deltaTime);
        SprintBlend = Mathf.MoveTowards(SprintBlend, isSprinting ? 1f : 0f, 10f * Time.deltaTime);

        if (input.sqrMagnitude > 1f)
        {
            input.Normalize();
        }

        float currentMoveSpeed = moveSpeed * Mathf.Lerp(1f, sprintMultiplier, SprintBlend);
        Vector3 horizontalMovement = (transform.forward * input.y) + (transform.right * input.x);
        horizontalMovement.y = 0f;

        if (characterController != null)
        {
            if (characterController.isGrounded && verticalVelocity < 0f)
            {
                verticalVelocity = groundedForce;
            }
            else
            {
                verticalVelocity += gravity * Time.deltaTime;
            }

            Vector3 movement = horizontalMovement * currentMoveSpeed;
            movement.y = verticalVelocity;
            characterController.Move(movement * Time.deltaTime);
            return;
        }

        transform.position += horizontalMovement * currentMoveSpeed * Time.deltaTime;
    }

    private void ApplyLookRotation()
    {
        transform.rotation = Quaternion.Euler(0f, yaw, 0f);

        if (cameraTransform != null && cameraTransform != transform)
        {
            cameraTransform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        }
    }

    private bool IsForwardPressed()
    {
        return IsPressed(Keyboard.current.zKey)
            || IsPressed(Keyboard.current.wKey)
            || IsPressed(Keyboard.current.upArrowKey);
    }

    private bool IsBackwardPressed()
    {
        return IsPressed(Keyboard.current.sKey)
            || IsPressed(Keyboard.current.downArrowKey);
    }

    private bool IsLeftPressed()
    {
        return IsPressed(Keyboard.current.qKey)
            || IsPressed(Keyboard.current.aKey)
            || IsPressed(Keyboard.current.leftArrowKey);
    }

    private bool IsRightPressed()
    {
        return IsPressed(Keyboard.current.dKey)
            || IsPressed(Keyboard.current.rightArrowKey);
    }

    private bool IsSprintPressed()
    {
        return IsPressed(Keyboard.current.leftShiftKey)
            || IsPressed(Keyboard.current.rightShiftKey);
    }

    private static bool IsPressed(KeyControl key)
    {
        return key != null && key.isPressed;
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
