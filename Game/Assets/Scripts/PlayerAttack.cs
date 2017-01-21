﻿using UnityEngine;
using System.Collections;

public class PlayerAttack : MonoBehaviour {
	public float activeDoorAttackTime;
	private float doorTimer;
	enum STATE{
		COOKING,
		OPEN,
		INVINCIBLE
	}

	STATE playerState;

	[SerializeField] int collectedEnemyType;
	[SerializeField] int timer;
	bool scored = true;
	GameObject door;

	// Use this for initialization
	void Start () {
		playerState = STATE.OPEN;
		GameObject.Find("DoorHinge").GetComponent<DoorController>().FinishCook();
		door = GameObject.Find ("Door");
	}
	
	// Update is called once per frame
	void Update () {
		if (GetComponent<PlayerHealth>().invincibility > 0){
			playerState = STATE.INVINCIBLE;
		}
		if (GetComponent<playerTimer>().cTimer <= 0){
			playerState = STATE.OPEN;
			if (!scored){
				scored = true;
				GameObject.Find("UI").GetComponent<Score>().increaseScore((collectedEnemyType+1)*10);
				GameObject.Find("DoorHinge").GetComponent<DoorController>().FinishCook();
				doorTimer = activeDoorAttackTime;
				door.tag = "DoorActive";
				StartCoroutine ("DoorAttackTimer");
			}
		}
	}

	IEnumerable DoorAttackTimer(){
		if (doorTimer > 0.0f) {
			doorTimer -= Time.deltaTime;
			if (doorTimer <= 0.0f) 
			{
				door.tag = "Player";
			}
		}			
		yield return null;
	}

	void OnCollisionEnter (Collision collision){
		if (collision.gameObject.tag == "Enemy"){
			EnemyCollision(collision.gameObject);
		}
	}

	void EnemyCollision(GameObject enemy){
		switch(playerState){
		case STATE.OPEN:
			collectedEnemyType = enemy.GetComponent<EnemyController>().type;
			playerState = STATE.COOKING;
			GameObject.Find("DoorHinge").GetComponent<DoorController>().Eat();
			switch(collectedEnemyType){
			case 0:
				GetComponent<playerTimer>().setCookingTimer(3.0f);
				scored = false;
				break;
			case 1:
				GetComponent<playerTimer>().setCookingTimer(6.0f);
				scored = false;
				break;
			case 2:
				GetComponent<PlayerHealth>().respawn();
				break;
			}
			Destroy(enemy);
			break;
		case STATE.COOKING:
			GetComponent<PlayerHealth>().respawn();
			playerState = STATE.INVINCIBLE;
			break;
		case STATE.INVINCIBLE:
			Destroy(enemy);
			break;
		}
	}

	public float GetAttackPower()
	{
		return doorTimer;
	}
}
