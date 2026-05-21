using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
	public GameObject panelOption;
	public GameObject panelCredits;

	void Start(){
		panelOption.SetActive(false);
		panelCredits.SetActive(false);
	}
	
	public void PlayGame(){
		Debug.Log("Lancer la partie");
		SceneManager.LoadScene("Gameplay");
	}
	
	public void OpenOptions(){
		panelOption.SetActive(true);
	}

	public void OpenCredits(){
		Debug.Log("Ouvre les crédits");
		panelCredits.SetActive(true);
	}
	
	public void CloseCredits(){
		Debug.Log("Ferme les crédits");
		panelCredits.SetActive(false);
	}
	
	public void CloseOptions(){
		panelOption.SetActive(false);
	}
	
	public void QuitGame(){
		Debug.Log("Quitter le jeu");
		Application.Quit();
	}
}
