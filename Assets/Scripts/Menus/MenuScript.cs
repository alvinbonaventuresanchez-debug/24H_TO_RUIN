using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{
    public GameObject panelOption;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        panelOption.SetActive(false);
    }

    public void StartGame(){
        Debug.Log("Start Game");
        SceneManager.LoadScene("Level1");
    }

    public void OpenOptions(){
        Debug.Log("Open Options");
        panelOption.SetActive(true);
    }
    public void QuitGame(){
        Debug.Log("Quit Game");
        Application.Quit();
    }

    public void CloseOptions(){
        Debug.Log("Close Options");
        panelOption.SetActive(false);
    }

}
