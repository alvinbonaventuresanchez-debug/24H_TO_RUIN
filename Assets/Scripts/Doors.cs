using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Doors : MonoBehaviour
{
    public GameObject door; 
    public Vector3 openPosition = new Vector3(0, 120, 0);
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
            Open();
    }
    void Open()
    {
        door.transform.localEulerAngles = openPosition;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
            Close();
    }
    void Close()
    {
        door.transform.localEulerAngles = Vector3.zero;
    }
}
