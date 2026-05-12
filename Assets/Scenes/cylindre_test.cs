using UnityEngine;

public class cylindre_test : MonoBehaviour
{
    public Vector3 rotationSpeed = new Vector3(0f, 90f, 0f);
    public float growthRate = 0.5f;
    public float maxScale = 3f;

    void Update()
    {
        // Rotation continue
        transform.Rotate(rotationSpeed * Time.deltaTime);

        // Grossissement progressif
        if (transform.localScale.x < maxScale)
        {
            transform.localScale += Vector3.one * growthRate * Time.deltaTime;
            if (transform.localScale.x > maxScale)
            {
                transform.localScale = Vector3.one * maxScale;
            }
        }
    }
}
