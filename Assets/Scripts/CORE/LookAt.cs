using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAt : MonoBehaviour
{
	private Transform player;
		
	void Start(){
		player = GameObject.FindWithTag("Player").transform;
	}
	
	void Update(){
		if (player == null) return;

		Vector3 direction = player.position - transform.position;
		direction.y = 0; //Le sprite reste droit
		transform.forward = -direction; //Le sprite regarde tout le temps le joueur
	}
    
}
