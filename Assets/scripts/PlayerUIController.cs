﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIController : MonoBehaviour {

	public Color32 successColor;
	public Color32 failureColor;

	Text score;
	RectTransform healthbar;
	GameObject healthBarObject;
	Image healthbarImage;

	// Use this for initialization
	void Start () {
		score = transform.Find("Score").GetComponent<Text>();
		healthBarObject = transform.Find("HealthContainer/HealthBar").gameObject;
		healthbar = healthBarObject.GetComponent<RectTransform>();
		healthbarImage = healthBarObject.GetComponent<Image>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void FixedUpdate(){
		score.text = GameManager.years + " Million Years";
		healthbar.localPosition = new Vector2(-500*GameManager.health/GameManager.maxHealth, 0);
		healthbarImage.color = Color32.Lerp(failureColor, successColor, GameManager.health/GameManager.maxHealth);
	}	
}
