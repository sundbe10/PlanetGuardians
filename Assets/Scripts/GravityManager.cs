﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityManager : MonoBehaviour {

	private static GravityManager instance = null;
	private static List<GravityBehavior> gravityObjects;
	public static float G = 6.67408E-11f * 10E10f;
	private static CharacterGravityBehavior character;
	private static CharacterMovementBehavior charMovement;

	public static GravityManager SharedInstance {
		get {
			if (instance == null) {
				instance = new GravityManager();
			}
			return instance;
		}
	}

	void Awake() {
		gravityObjects = new List<GravityBehavior>();
	}

	public static void RegisterGravityObject(GravityBehavior obj, bool isCharacter = false) {
		if (isCharacter)
		{
			character = (CharacterGravityBehavior)obj;
			charMovement = character.GetComponent<CharacterMovementBehavior>();
			return;
		}
		else if (!gravityObjects.Contains(obj))
		{
			gravityObjects.Add(obj);
		}
	}

	public static void UnregisterGravityObject(GravityBehavior obj) {
		if (gravityObjects.Contains(obj))
		{
			gravityObjects.Remove(obj);
		}
	}

	void Update() {
		Vector2 maxCharacterPullForce = Vector2.zero;
		GameObject charAttachedPlanet = charMovement.attachedPlanet;

		// Planet forces
		foreach(GravityBehavior giveObj in gravityObjects)
		{
			if (giveObj.hasGravity)
			{
				Collider2D col1 = giveObj.GetComponent<Collider2D>();
				foreach(GravityBehavior receiveObj in gravityObjects)
				{
					if (giveObj != receiveObj)
					{
						Collider2D col2 = receiveObj.GetComponent<Collider2D>();
						if (!col1.IsTouching(col2) && receiveObj.isAffectedByGravity)
						{
							ApplyPullForce(giveObj,receiveObj);
						}
					}
				}
				
				// Character-specific forces from each planet
				if (character != null && Vector2.Distance(character.GetFootPosition(), giveObj.GetPosition()) > giveObj.radius)
				{
					Vector2 force = ApplyCharacterPull(giveObj);
					if (force.sqrMagnitude > maxCharacterPullForce.sqrMagnitude)
					{
						maxCharacterPullForce = force;
						charAttachedPlanet = giveObj.gameObject;
					}
				}
			}
		}
		charMovement.attachedPlanet = charAttachedPlanet;
	}
		
	Vector2 ApplyPullForce(GravityBehavior obj1, GravityBehavior obj2)
	{
		Vector2 pos1 = obj1.GetPosition();
		Vector2 pos2 = obj2.GetPosition();

		float radius = Vector2.Distance(pos1, pos2);

		Vector2 force = (pos1 - pos2).normalized * (G * obj1.mass * obj2.mass) / (radius * radius);

		obj2.body.AddForce(force);
		return force;
	}

	Vector2 ApplyCharacterPull(GravityBehavior obj1)
	{
		Vector2 pos1 = obj1.GetPosition();
		Vector2 pos2 = character.GetPosition();

		float radius = Vector2.Distance(pos1, pos2);

		Vector2 force = (pos1 - pos2).normalized * (G * obj1.mass * character.mass) / (radius * radius);

		character.GetComponent<CharacterMovementBehavior>().Fall(force);
		return force;
	}

	void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawSphere(charMovement.attachedPlanet.transform.position, .5f);
	}
}
