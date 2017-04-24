﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager> {

	public enum State{
		START,
		ACTIVE,
		END
	}

	public delegate void OnGameStateChangeDelegate (State state);
	public static event OnGameStateChangeDelegate OnGameStateChange;

	static public int years = 1;
	static public float health = 100;
	static public float maxHealth = 100;

	int yearCounter;
	State state;



	void Awake() {
		if (Instance != this) {
			Destroy(this.gameObject);
		}
	}

	// Use this for initialization
	void Start () {
		PlayerBehavior.OnHealthChange += HealthChange;
		PlayerBehavior.OnSetHealth += SetHealth;
		ChangeState(State.START);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void FixedUpdate(){
		switch(state){
		case State.START:
			WaitForStart();
			break;
		case State.ACTIVE:
			IncrementYears();
			break;
		case State.END:
			break;
		}
	}

	void WaitForStart(){
		if(Input.GetKeyDown("Confirm")){
			ChangeState(State.ACTIVE);
		}
	}

	void IncrementYears(){
		yearCounter++;
		if(yearCounter == 10){
			years++;
			yearCounter = 0;
		}
	}

	void SetHealth(float health){
		maxHealth = health;
	}

	void HealthChange(float newHealth){
		health = newHealth;
		if(health <= 0 ){
			ChangeState(State.END);
		}
	}

	void ChangeState(State newState){
		OnGameStateChange(state);
		state = newState;
	}
}
