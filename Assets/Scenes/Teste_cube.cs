using UnityEngine;

public class Teste_cube : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Faire grossir et rapetisser le cube
        float scale = 1 + 0.5f * Mathf.Sin(Time.time * 2);
        transform.localScale = Vector3.one * scale;

        // Faire tourner le cube sur lui-même
        transform.Rotate(Vector3.up * 50 * Time.deltaTime);
    }
}


//coucou

