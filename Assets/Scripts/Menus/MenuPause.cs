using UnityEngine;
using UnityEngine.InputSystem;

public class MenuPause : MonoBehaviour
{
    public GameObject panelPause;
    public GameObject panelOption;
    void Start()
    {
        panelPause.SetActive(false);
        panelOption.SetActive(false);
    }
//ouvrir et fermer les options
    public void OpenOptions(){
    Debug.Log("Open options");
    panelOption.SetActive(true);
    }

    public void CloseOptions(){
    Debug.Log("Close options");
    panelOption.SetActive(false);
    }

//reprendre la partie (sans appuyer sur échap)
    public void Resume(){
    Debug.Log("Resume");
    panelPause.SetActive(!panelPause.activeInHierarchy);
    }

//Quitter la partie
    public void leave(){
    Debug.Log("Quitter la partie");
    //changement de scène à ajouter !
    }





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
