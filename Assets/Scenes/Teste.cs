using UnityEngine;

public class Teste : MonoBehaviour
{
    [SerializeField] private Vector3 rotationSpeed = new Vector3(60f, 90f, 45f);
    [SerializeField] private float scaleAmplitude = 0.25f;
    [SerializeField] private float scaleSpeed = 2f;

    private Vector3 baseScale;

    void Start()
    {
        baseScale = transform.localScale;
    }

    void Update()
    {
        transform.Rotate(rotationSpeed * Time.deltaTime, Space.Self);

        float scaleOffset = 1f + Mathf.Sin(Time.time * scaleSpeed) * scaleAmplitude;
        transform.localScale = baseScale * scaleOffset;
    }
}
