using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

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
        OpenPause(!panelPause.activeInHierarchy);
    }

//Quitter la partie
    public void leave(){
    Debug.Log("Quitter la partie");
    SceneManager.LoadScene("MainMenu");
    }





//afficher/cacher le menu pause 
    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Debug.Log("menu pause");
            bool newState = !panelPause.activeInHierarchy;
            OpenPause(newState);
            Debug.Log("Close options");
            panelOption.SetActive(false);

            //à ajouter : faire pause sur le gameplay, le timer, etc 
        }
    }

    void OpenPause(bool newState)
    {
            Cursor.lockState = newState ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = newState;
            panelPause.SetActive(newState);
            Time.timeScale = newState ? 0 : 1; 
    }
}
