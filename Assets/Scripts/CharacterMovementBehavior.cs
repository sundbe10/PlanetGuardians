﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovementBehavior : MonoBehaviour {

	float acceleration = 5f;
	float maxjumpForce = 100f;
	float pushForce = 1f;
	float jumpForce;
	float angVelocity;
	Vector2 gravVelocity;
	float maxSpeed = 2f;
	[HideInInspector] public bool isGrounded;
	[HideInInspector] public GameObject attachedPlanet;
	float characterGravityConstant = 10E-3f;
	bool spawning;
	public float airSupply;
	ParticleSystem blowParticles;
	Vector2 jumpVec;

	// Use this for initialization
	void Awake () {
		angVelocity = 0;
		gravVelocity = Vector2.zero;
		isGrounded = false;
		jumpForce = 0f;
		spawning = false;
		airSupply = 1f;
		blowParticles = GetComponentInChildren<ParticleSystem>();
		jumpVec = Vector2.zero;
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		// Jumping
		if (Input.GetButton("Jump") && jumpForce > 0f)
		{
			if (isGrounded)
			{
				jumpVec = (((Vector2)transform.position - (Vector2)attachedPlanet.transform.position));
				gravVelocity += attachedPlanet.GetComponent<Rigidbody2D>().velocity;
				isGrounded = false;
				transform.parent = null;
			}

			Fall(jumpVec*jumpForce);
			jumpForce = Mathf.Lerp(jumpForce,0,15f*Time.deltaTime);
		}

		Vector3 newPos = (Vector2)transform.position + gravVelocity*Time.deltaTime;
		transform.position = newPos;

		if (!isGrounded && !Input.GetButton("Jump") && jumpForce > 0)
			jumpForce = 0f;

		// Walking
			if (Input.GetAxis("Horizontal") < 0 && angVelocity >  -maxSpeed)
			{
				angVelocity = Mathf.Lerp(angVelocity, -maxSpeed, acceleration*Time.deltaTime);
			}
			else if (Input.GetAxis("Horizontal") > 0 && angVelocity < maxSpeed)
			{
				angVelocity = Mathf.Lerp(angVelocity, maxSpeed, acceleration*Time.deltaTime);
			}
			else if (angVelocity != 0)
				angVelocity = Mathf.Lerp(angVelocity, 0f, acceleration*Time.deltaTime);
			else
				angVelocity = 0f;

		if (attachedPlanet != null)
		{
			PlanetBehavior planetBehavior = attachedPlanet.GetComponent<PlanetBehavior>();
			float radius = Vector2.Distance((Vector2)transform.position, attachedPlanet.transform.position) - planetBehavior.size;
			transform.RotateAround(attachedPlanet.transform.position, Vector3.forward, -angVelocity/(radius*radius));

			
			// Auto-turning
			Vector3 distanceVec = transform.position - attachedPlanet.transform.position;
			transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(Vector3.forward,(distanceVec)), Mathf.Pow(5f/distanceVec.magnitude,3f));

		}

		if (!isGrounded)
			transform.rotation *= Quaternion.AngleAxis(-Mathf.Clamp(angVelocity*3f, -20f, 20f), Vector3.forward);
		
		// Blowing
		if (Input.GetButton("Fire3") && airSupply > 0f)
		{
			blowParticles.Play();
			if (isGrounded)
			{
				Vector2 force = (attachedPlanet.transform.position - transform.position).normalized*pushForce;
				attachedPlanet.GetComponent<Rigidbody2D>().AddForce(force);
			}
			else
			{
				gravVelocity -= (Vector2)transform.up * 0.2f;
				airSupply = airSupply > 0.0f ? airSupply -Time.deltaTime/3f : 0.0f;
			}
		}
		else
			blowParticles.Stop();


		// Spawning offspring
		if (isGrounded)
		{
			airSupply = airSupply < 1.0f ? airSupply +Time.deltaTime : 1.0f;
			
			if (!spawning)
			{
				spawning = true;
				StartCoroutine(SpawnOffspring(0.5f));
			}
		}
		else if (spawning)
		{
			StopAllCoroutines();
			spawning = false;
		}
	}

	public void Fall(Vector2 force)
	{
		gravVelocity += force*characterGravityConstant;
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		if (col.gameObject.tag == "Planet")
		{
			gravVelocity = Vector2.zero;
			isGrounded = true;
			jumpForce = maxjumpForce;
			transform.parent = attachedPlanet.transform;
		}
	}

	IEnumerator SpawnOffspring(float habitability){
		PlanetBehavior planet = attachedPlanet.GetComponent<PlanetBehavior>();

		while (spawning) {
			float waitTimer = Random.Range(1f,2f)/habitability;
			yield return new WaitForSeconds(waitTimer);
			planet.SpawnOffspring();
		}
	}
}
