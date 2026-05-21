using UnityEngine;
using UnityEngine.SceneManagement;

public class LinkMenu : MonoBehaviour
{

    void Start()
    {
        
    }
    public void BackToMenu(){
    Debug.Log("Menu");
    SceneManager.LoadScene("MainMenu");
    }

    public void Relaunch(){
    Debug.Log("Relancer");
    SceneManager.LoadScene("Gameplay");
    }


    void Update()
    {
        
    }
}
