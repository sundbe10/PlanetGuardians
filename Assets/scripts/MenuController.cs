﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetButtonDown("Submit") && SceneManager.GetActiveScene().name != "game"){
			GoToScene("game");
		}
	}

	public void GoToScene(string scene){
		Debug.Log(scene);
		SceneLoader.GoToScene(scene);
	}
}
