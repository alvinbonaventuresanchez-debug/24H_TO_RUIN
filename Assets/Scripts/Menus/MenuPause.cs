using UnityEngine;
using UnityEngine.InputSystem;

public class MenuPause : MonoBehaviour
{
    public GameObject panelPause;
    void Start()
    {
        panelPause.SetActive(false);
    }
//fonctionalités des boutons (options,retour au menu,etc)
/*
    public void OpenOptions(){
    Debug.Log("Open options");
    panelOption.SetActive(true);
    }
*/


//afficher/cacher le menu pause 
    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Debug.Log("menu pause");
            panelPause.SetActive(!panelPause.activeInHierarchy);
            //à ajouter : faire pause sur le gameplay, le timer, etc 
        }
    }
}
