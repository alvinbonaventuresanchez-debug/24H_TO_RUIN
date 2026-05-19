using UnityEngine;

public class HandFloatAnimation : MonoBehaviour
{
    [SerializeField] private float moveAmplitude = 0.08f;
    [SerializeField] private float moveSpeed = 1.5f;
    [SerializeField] private Vector3 moveAxis = Vector3.up;

    private Vector3 baseLocalPosition;

    private void Start()
    {
        baseLocalPosition = transform.localPosition;
    }

    private void Update()
    {
        Vector3 normalizedAxis = moveAxis.sqrMagnitude > 0f ? moveAxis.normalized : Vector3.up;
        float offset = Mathf.Sin(Time.time * moveSpeed) * moveAmplitude;

        transform.localPosition = baseLocalPosition + normalizedAxis * offset;
    }
}
