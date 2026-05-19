using UnityEngine;

public class HandFloatAnimation : MonoBehaviour
{
    [SerializeField] private float idleAmplitude = 0.03f;
    [SerializeField] private float idleSpeed = 1.35f;
    [SerializeField] private float movingAmplitude = 0.09f;
    [SerializeField] private float movingSpeed = 2.4f;
    [SerializeField] private float sprintAmplitudeMultiplier = 1.08f;
    [SerializeField] private float sprintSpeedMultiplier = 1.25f;
    [SerializeField] private float sprintTiltMultiplier = 1.08f;
    [SerializeField] private float sidewaysAmplitude = 0.006f;
    [SerializeField] private float rollAngle = 1.4f;
    [SerializeField] private float pitchAngle = 2.4f;
    [SerializeField] private Vector3 verticalAxis = Vector3.up;
    [SerializeField] private Vector3 sidewaysAxis = Vector3.right;
    [SerializeField] private float transitionSpeed = 8f;

    private Vector3 baseLocalPosition;
    private Quaternion baseLocalRotation;
    private FreeCameraController cameraController;
    private float bobTime;
    private float currentAmplitude;
    private float currentSpeed;
    private float currentSidewaysAmplitude;

    private void Start()
    {
        baseLocalPosition = transform.localPosition;
        baseLocalRotation = transform.localRotation;
        cameraController = GetComponentInParent<FreeCameraController>();
        currentAmplitude = idleAmplitude;
        currentSpeed = idleSpeed;
        currentSidewaysAmplitude = 0f;
    }

    private void Update()
    {
        Vector3 normalizedVerticalAxis = verticalAxis.sqrMagnitude > 0f ? verticalAxis.normalized : Vector3.up;
        Vector3 normalizedSidewaysAxis = sidewaysAxis.sqrMagnitude > 0f ? sidewaysAxis.normalized : Vector3.right;
        float movementBlend = cameraController != null ? cameraController.MovementBlend : 0f;
        float sprintBlend = cameraController != null ? cameraController.SprintBlend : 0f;
        float sprintAmplitudeBoost = Mathf.Lerp(1f, sprintAmplitudeMultiplier, sprintBlend);
        float sprintSpeedBoost = Mathf.Lerp(1f, sprintSpeedMultiplier, sprintBlend);
        float sprintTiltBoost = Mathf.Lerp(1f, sprintTiltMultiplier, sprintBlend);
        float targetAmplitude = Mathf.Lerp(idleAmplitude, movingAmplitude, movementBlend) * sprintAmplitudeBoost;
        float targetSpeed = Mathf.Lerp(idleSpeed, movingSpeed, movementBlend) * sprintSpeedBoost;
        float targetSidewaysAmplitude = sidewaysAmplitude * movementBlend;

        currentAmplitude = Mathf.Lerp(currentAmplitude, targetAmplitude, transitionSpeed * Time.deltaTime);
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, transitionSpeed * Time.deltaTime);
        currentSidewaysAmplitude = Mathf.Lerp(currentSidewaysAmplitude, targetSidewaysAmplitude, transitionSpeed * Time.deltaTime);
        bobTime += Time.deltaTime * currentSpeed;

        float swayWave = Mathf.Sin(bobTime);
        float stepWave = Mathf.Sin(bobTime * 2f);
        float verticalOffset = stepWave * currentAmplitude;
        float sidewaysOffset = swayWave * currentSidewaysAmplitude;
        float rollOffset = swayWave * rollAngle * movementBlend * sprintTiltBoost;
        float pitchOffset = -Mathf.Abs(stepWave) * pitchAngle * movementBlend * sprintTiltBoost;

        transform.localPosition = baseLocalPosition
            + normalizedVerticalAxis * verticalOffset
            + normalizedSidewaysAxis * sidewaysOffset;
        transform.localRotation = baseLocalRotation * Quaternion.Euler(pitchOffset, 0f, rollOffset);
    }
}
