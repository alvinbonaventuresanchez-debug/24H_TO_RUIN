using UnityEngine;
using UnityEngine.InputSystem;

public class MenuPause : MonoBehaviour
{
    public GameObject panelPause;
    void Start()
    {
        panelPause.SetActive(false);
    }

    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Debug.Log("menu pause");
            panelPause.SetActive(!panelPause.activeInHierarchy);
        }
    }
}
