using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Doors : MonoBehaviour
{
    public GameObject door;
    public Vector3 openPosition = new Vector3(0, 150, 0);

    private bool active = true;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && active)
        {
            Open();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && active)
        {
            Close();
        }
    }

    void Open()
    {
        if (door == null)
        {
            Debug.LogError("Doors: aucune référence de porte assignée dans l'inspecteur.", gameObject);
            return;
        }

        door.transform.localEulerAngles = openPosition;
    }

    void Close()
    {
        if (door == null)
        {
            Debug.LogError("Doors: aucune référence de porte assignée dans l'inspecteur.", gameObject);
            return;
        }

        door.transform.localEulerAngles = Vector3.zero;
    }
}
