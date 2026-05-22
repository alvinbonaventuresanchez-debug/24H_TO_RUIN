using UnityEngine;
using UnityEngine.SceneManagement;

public class DebutJeu : MonoBehaviour
{
    public bool cinematic;


        public void goToGame(){
        cinematic = true;
		SceneManager.LoadScene("Gameplay");
	}

}
