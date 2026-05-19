using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
	public GameObject panelOption;
	
	void Start(){
		panelOption.SetActive(false);
	}
	
	public void PlayGame(){
		Debug.Log("Lancer la partie");
		SceneManager.LoadScene("Level1");
	}
	
	public void OpenOptions(){
		panelOption.SetActive(true);
	}
	
	public void CloseOptions(){
		panelOption.SetActive(false);
	}
	
	public void QuitGame(){
		Debug.Log("Quitter le jeu");
		Application.Quit();
	}
}
