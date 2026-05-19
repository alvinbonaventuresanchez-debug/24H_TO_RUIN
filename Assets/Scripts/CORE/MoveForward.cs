using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveForward : MonoBehaviour
{
	private Transform player;
	public float speed = 0.1f;
	
	private float stop_distance = 3f;

    private CharacterController cc;

    void Start()
    {
        cc = GetComponent<CharacterController>();
        player = GameObject.FindWithTag("Player").transform;
    }

    void Update()
    {
        //Si le jeu est en pause, ou si on ne trouve pas le joueur, on ne fait rien
        if (HudManager.pause || player == null)
        {
            return;
        }

        //Si la distance entre le joueur et l'ennemi est inférieure à stop_distance
        float distance = Vector3.Distance(player.transform.position, transform.position);

        if (distance < stop_distance){
            //L'ennemi se déplace vers le joueur
            Vector3 moveDir = player.position - transform.position;
            moveDir = moveDir.normalized;
            moveDir.y = -2f; //Pour rester collé au sol
            cc.Move(moveDir * speed * Time.deltaTime);
        }
    }
}
