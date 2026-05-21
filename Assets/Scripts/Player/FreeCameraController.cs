using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class FreeCameraController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3.5f;
    [SerializeField] private float sprintMultiplier = 1.7f;
    [SerializeField] private float lookSensitivity = 0.12f;
    [SerializeField] private float maxPitch = 80f;
    [SerializeField] private bool lockCursorOnStart = true;
    [Header("References")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Transform leftHandTransform;
    [SerializeField] private Transform rightHandTransform;
    [Header("Gravity")]
    [SerializeField] private float gravity = -20f;
    [SerializeField] private float groundedForce = -2f;
    [SerializeField] private float jumpHeight = 0.5f;
    [Header("Vol")]
    [SerializeField] private float flightSpeed = 4f;
    [SerializeField] private float flightDoubleTapDelay = 0.25f;
    [Header("SIX Easter Egg")]
    [SerializeField] private bool enableSixEasterEgg = true;
    [SerializeField] private float easterEggDuration = 6.7f;
    [SerializeField] private float easterEggIntroDuration = 0.25f;
    [SerializeField] private float easterEggOutroDuration = 0.45f;
    [SerializeField] private float easterEggSequenceTimeout = 1f;
    [SerializeField] private AudioClip sixEasterEggClip;
    [SerializeField] [Range(0f, 1f)] private float sixEasterEggVolume = 0.9f;
    [SerializeField] private bool loopSixEasterEggClip = true;
    [SerializeField] private float easterEggMoveAmplitude = 0.12f;
    [SerializeField] private float easterEggMoveFrequency = 1.45f;
    [SerializeField] private Vector3 easterEggLiftAxis = Vector3.up;
    [SerializeField] private bool useAbsoluteLeftHandPalmUpPose = true;
    [SerializeField] private Vector3 leftHandPalmUpLocalPosition = new Vector3(-1.044f, 0.579f, -0.346f);
    [SerializeField] private Vector3 leftHandPalmUpLocalEulerAngles = new Vector3(-21.114f, -71.753f, 139.081f);
    [SerializeField] private bool useAbsoluteRightHandPalmUpPose = true;
    [SerializeField] private Vector3 rightHandPalmUpLocalPosition = new Vector3(-0.139f, 0.596f, 0.379f);
    [SerializeField] private Vector3 rightHandPalmUpLocalEulerAngles = new Vector3(-42.482f, 86.579f, -111.08f);
    [SerializeField] private Vector3 leftHandPalmUpEulerOffset = new Vector3(18f, 0f, 52f);
    [SerializeField] private Vector3 rightHandPalmUpEulerOffset = new Vector3(18f, 0f, -52f);
    [SerializeField] private Vector3 leftHandPalmUpPositionOffset = new Vector3(-0.015f, 0.035f, 0.015f);
    [SerializeField] private Vector3 rightHandPalmUpPositionOffset = new Vector3(0.015f, 0.035f, 0.015f);

    private float yaw;
    private float pitch;
    private float verticalVelocity;
    // VOL : indique si le joueur est en train de voler.
    private bool isFlying;
    // VOL : memorise le moment du dernier appui sur Espace pour detecter le double appui.
    private float lastSpacePressTime = -10f;
    private CharacterController characterController;
    private AudioSource sixEasterEggAudioSource;
    private int easterEggSequenceIndex;
    private float easterEggSequenceTimer;
    private float easterEggElapsed = -1f;
    private bool hasCachedHandPose;
    private bool warnedAboutMissingHands;
    private Vector3 leftHandBaseLocalPosition;
    private Vector3 rightHandBaseLocalPosition;
    private Vector3 leftHandPalmUpPosition;
    private Vector3 rightHandPalmUpPosition;
    private Quaternion leftHandBaseLocalRotation;
    private Quaternion rightHandBaseLocalRotation;
    private Quaternion leftHandPalmUpRotation;
    private Quaternion rightHandPalmUpRotation;

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

        ResolveHandTransforms();
        CacheHandPose();
        EnsureSixEasterEggAudioSource();
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
        HandleSixEasterEggInput();
        HandleLook();
        HandleMovement();
        UpdateSixEasterEggAnimation();
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
        // VOL : detecte un double appui rapide sur Espace pour activer ou couper le vol.
        bool flightToggledThisFrame = HandleFlightToggle();

        if (characterController != null)
        {
            if (isFlying)
            {
                // VOL : pendant le vol, la gravite est coupee.
                verticalVelocity = 0f;

                // VOL : maintenir Espace fait monter le joueur.
                if (IsFlightUpPressed())
                {
                    verticalVelocity += flightSpeed;
                }

                // VOL : maintenir Tab fait descendre le joueur.
                if (IsFlightDownPressed())
                {
                    verticalVelocity -= flightSpeed;
                }

                Vector3 flyingMovement = horizontalMovement * currentMoveSpeed;
                flyingMovement.y = verticalVelocity;
                characterController.Move(flyingMovement * Time.deltaTime);
                return;
            }

            bool isGrounded = characterController.isGrounded;

            if (isGrounded && verticalVelocity < 0f)
            {
                verticalVelocity = groundedForce;
            }

            if (!flightToggledThisFrame && isGrounded && WasJumpPressed())
            {
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
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

        if (isFlying)
        {
            // VOL : meme sans CharacterController, Espace fait monter et Tab fait descendre.
            float flightVerticalMovement = 0f;

            if (IsFlightUpPressed())
            {
                flightVerticalMovement += flightSpeed;
            }

            if (IsFlightDownPressed())
            {
                flightVerticalMovement -= flightSpeed;
            }

            horizontalMovement.y = flightVerticalMovement;
        }

        Vector3 fallbackMovement = horizontalMovement * currentMoveSpeed;
        fallbackMovement.y = horizontalMovement.y;
        transform.position += fallbackMovement * Time.deltaTime;
    }

    private void ApplyLookRotation()
    {
        transform.rotation = Quaternion.Euler(0f, yaw, 0f);

        if (cameraTransform != null && cameraTransform != transform)
        {
            cameraTransform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        }
    }

    private void HandleSixEasterEggInput()
    {
        if (!enableSixEasterEgg || Keyboard.current == null || IsSixEasterEggPlaying())
        {
            return;
        }

        if (!hasCachedHandPose)
        {
            ResolveHandTransforms();
            CacheHandPose();

            if (!hasCachedHandPose)
            {
                WarnAboutMissingHands();
                return;
            }
        }

        if (easterEggSequenceIndex > 0)
        {
            easterEggSequenceTimer -= Time.deltaTime;

            if (easterEggSequenceTimer <= 0f)
            {
                ResetSixEasterEggSequence();
            }
        }

        if (easterEggSequenceIndex == 0)
        {
            if (IsSprintPressed() && WasPressed(Keyboard.current.sKey))
            {
                easterEggSequenceIndex = 1;
                easterEggSequenceTimer = easterEggSequenceTimeout;
            }

            return;
        }

        if (easterEggSequenceIndex == 1)
        {
            if (WasPressed(Keyboard.current.iKey))
            {
                easterEggSequenceIndex = 2;
                easterEggSequenceTimer = easterEggSequenceTimeout;
                return;
            }

            if (Keyboard.current.anyKey.wasPressedThisFrame)
            {
                ResetSixEasterEggSequence();
            }

            return;
        }

        if (WasPressed(Keyboard.current.xKey))
        {
            ResetSixEasterEggSequence();
            StartSixEasterEgg();
            return;
        }

        if (Keyboard.current.anyKey.wasPressedThisFrame)
        {
            ResetSixEasterEggSequence();
        }
    }

    private void UpdateSixEasterEggAnimation()
    {
        if (!IsSixEasterEggPlaying())
        {
            return;
        }

        if (!hasCachedHandPose || leftHandTransform == null || rightHandTransform == null)
        {
            StopSixEasterEgg();
            return;
        }

        easterEggElapsed += Time.deltaTime;

        float duration = Mathf.Max(0.1f, easterEggDuration);
        float clampedTime = Mathf.Min(easterEggElapsed, duration);
        float poseBlend = EvaluateSixEasterEggBlend(clampedTime, duration);
        Vector3 liftAxis = easterEggLiftAxis.sqrMagnitude > 0f ? easterEggLiftAxis.normalized : Vector3.up;
        float wave = Mathf.Sin(clampedTime * easterEggMoveFrequency * Mathf.PI * 2f);
        float verticalOffset = wave * easterEggMoveAmplitude * poseBlend;
        Vector3 leftPosePosition = Vector3.Lerp(leftHandBaseLocalPosition, leftHandPalmUpPosition, poseBlend);
        Vector3 rightPosePosition = Vector3.Lerp(rightHandBaseLocalPosition, rightHandPalmUpPosition, poseBlend);

        leftHandTransform.localPosition = leftPosePosition + (liftAxis * verticalOffset);
        leftHandTransform.localRotation = Quaternion.Slerp(leftHandBaseLocalRotation, leftHandPalmUpRotation, poseBlend);

        rightHandTransform.localPosition = rightPosePosition - (liftAxis * verticalOffset);
        rightHandTransform.localRotation = Quaternion.Slerp(rightHandBaseLocalRotation, rightHandPalmUpRotation, poseBlend);

        if (easterEggElapsed >= duration)
        {
            StopSixEasterEgg();
        }
    }

    private void StartSixEasterEgg()
    {
        CacheHandPose();

        if (!hasCachedHandPose)
        {
            WarnAboutMissingHands();
            return;
        }

        // Lance le son de l'easter egg lorsque la séquence est validée.
        PlaySixEasterEggAudio();
        easterEggElapsed = 0f;
    }

    private void StopSixEasterEgg()
    {
        easterEggElapsed = -1f;
        StopSixEasterEggAudio();

        if (!hasCachedHandPose)
        {
            return;
        }

        leftHandTransform.localPosition = leftHandBaseLocalPosition;
        leftHandTransform.localRotation = leftHandBaseLocalRotation;
        rightHandTransform.localPosition = rightHandBaseLocalPosition;
        rightHandTransform.localRotation = rightHandBaseLocalRotation;
    }

    private void ResetSixEasterEggSequence()
    {
        easterEggSequenceIndex = 0;
        easterEggSequenceTimer = 0f;
    }

    private void ResolveHandTransforms()
    {
        if (cameraTransform == null)
        {
            return;
        }

        if (leftHandTransform == null)
        {
            leftHandTransform = FindDescendantByName(cameraTransform, "main gauche");
        }

        if (rightHandTransform == null)
        {
            rightHandTransform = FindDescendantByName(cameraTransform, "main droite");
        }
    }

    private void CacheHandPose()
    {
        if (leftHandTransform == null || rightHandTransform == null)
        {
            hasCachedHandPose = false;
            return;
        }

        leftHandBaseLocalPosition = leftHandTransform.localPosition;
        rightHandBaseLocalPosition = rightHandTransform.localPosition;
        leftHandBaseLocalRotation = leftHandTransform.localRotation;
        rightHandBaseLocalRotation = rightHandTransform.localRotation;
        leftHandPalmUpPosition = useAbsoluteLeftHandPalmUpPose
            ? leftHandPalmUpLocalPosition
            : leftHandBaseLocalPosition + leftHandPalmUpPositionOffset;
        leftHandPalmUpRotation = useAbsoluteLeftHandPalmUpPose
            ? Quaternion.Euler(leftHandPalmUpLocalEulerAngles)
            : leftHandBaseLocalRotation * Quaternion.Euler(leftHandPalmUpEulerOffset);
        rightHandPalmUpPosition = useAbsoluteRightHandPalmUpPose
            ? rightHandPalmUpLocalPosition
            : rightHandBaseLocalPosition + rightHandPalmUpPositionOffset;
        rightHandPalmUpRotation = useAbsoluteRightHandPalmUpPose
            ? Quaternion.Euler(rightHandPalmUpLocalEulerAngles)
            : rightHandBaseLocalRotation * Quaternion.Euler(rightHandPalmUpEulerOffset);
        hasCachedHandPose = true;
        warnedAboutMissingHands = false;
    }

    private void EnsureSixEasterEggAudioSource()
    {
        if (sixEasterEggAudioSource != null)
        {
            return;
        }

        sixEasterEggAudioSource = gameObject.AddComponent<AudioSource>();
        sixEasterEggAudioSource.playOnAwake = false;
        sixEasterEggAudioSource.loop = loopSixEasterEggClip;
        sixEasterEggAudioSource.spatialBlend = 0f;
        sixEasterEggAudioSource.volume = sixEasterEggVolume;
    }

    private void PlaySixEasterEggAudio()
    {
        if (sixEasterEggClip == null)
        {
            Debug.LogWarning("SIX easter egg: clip audio manquant.", this);
            return;
        }

        EnsureSixEasterEggAudioSource();
        // Redémarre le clip depuis le début pour chaque activation de l'easter egg.
        sixEasterEggAudioSource.Stop();
        sixEasterEggAudioSource.clip = sixEasterEggClip;
        sixEasterEggAudioSource.loop = loopSixEasterEggClip;
        sixEasterEggAudioSource.volume = sixEasterEggVolume;
        sixEasterEggAudioSource.time = 0f;
        sixEasterEggAudioSource.Play();
    }

    private void StopSixEasterEggAudio()
    {
        // Coupe le son si l'easter egg est arrêté prématurément ou à la fin.
        if (sixEasterEggAudioSource != null && sixEasterEggAudioSource.isPlaying)
        {
            sixEasterEggAudioSource.Stop();
        }
    }

    private void WarnAboutMissingHands()
    {
        if (warnedAboutMissingHands)
        {
            return;
        }

        warnedAboutMissingHands = true;
        Debug.LogWarning("SIX easter egg: impossible de trouver \"main gauche\" et/ou \"main droite\".", this);
    }

    private static Transform FindDescendantByName(Transform root, string targetName)
    {
        if (root == null)
        {
            return null;
        }

        if (root.name == targetName)
        {
            return root;
        }

        for (int i = 0; i < root.childCount; i++)
        {
            Transform foundChild = FindDescendantByName(root.GetChild(i), targetName);

            if (foundChild != null)
            {
                return foundChild;
            }
        }

        return null;
    }

    private bool IsSixEasterEggPlaying()
    {
        return easterEggElapsed >= 0f;
    }

    private float EvaluateSixEasterEggBlend(float elapsed, float duration)
    {
        float introDuration = Mathf.Clamp(easterEggIntroDuration, 0.01f, duration * 0.5f);
        float outroDuration = Mathf.Clamp(easterEggOutroDuration, 0.01f, duration * 0.5f);

        if (elapsed <= introDuration)
        {
            return Mathf.SmoothStep(0f, 1f, elapsed / introDuration);
        }

        if (elapsed >= duration - outroDuration)
        {
            float outroProgress = (elapsed - (duration - outroDuration)) / outroDuration;
            return Mathf.SmoothStep(1f, 0f, outroProgress);
        }

        return 1f;
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

    private bool HandleFlightToggle()
    {
        if (!WasJumpPressed())
        {
            return false;
        }

        float currentTime = Time.time;

        // VOL : si Espace est presse deux fois tres vite, on active ou on coupe le vol.
        if (currentTime - lastSpacePressTime <= flightDoubleTapDelay)
        {
            isFlying = !isFlying;

            // VOL : on remet la vitesse verticale a zero pour demarrer ou couper le vol proprement.
            verticalVelocity = 0f;
            lastSpacePressTime = -10f;
            return true;
        }

        // VOL : premier appui sur Espace, on attend un second appui rapide.
        lastSpacePressTime = currentTime;
        return false;
    }

    private bool IsFlightUpPressed()
    {
        // VOL : Espace maintenu sert a monter pendant le vol.
        return IsPressed(Keyboard.current.spaceKey);
    }

    private bool IsFlightDownPressed()
    {
        // VOL : Tab maintenu sert a descendre pendant le vol.
        return IsPressed(Keyboard.current.tabKey);
    }

    private bool WasJumpPressed()
    {
        return WasPressed(Keyboard.current.spaceKey);
    }

    private static bool IsPressed(KeyControl key)
    {
        return key != null && key.isPressed;
    }

    private static bool WasPressed(KeyControl key)
    {
        return key != null && key.wasPressedThisFrame;
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
