using UnityEngine;

public class SphereTest : MonoBehaviour
{
    public float speed = 1f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MoveSphere(){
        transform.position += new Vector3(1, 0, 0)* Time.deltaTime * speed;
    }
}
