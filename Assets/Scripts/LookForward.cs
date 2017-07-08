﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 
 * Roterar spelobjektet framåt. Alltså dit den rör sig
 */
public class LookForward : MonoBehaviour
{
	// true=höger/vänster, false=3D
	public bool is2D = false;
	// Upp för världen
	public Vector3 worldUp = Vector3.up;

	// Används för att jämföra hur objektet förflyttade sig
	private Vector3 oldPosition;

	private void Start() {
		oldPosition = transform.position;
	}

	private void Update()
	{
		// Normalisera /worldUp/ och nollställ ifall nära 0
		if (worldUp.sqrMagnitude < Vector3.kEpsilon) {
			worldUp = Vector3.up;
		} else {
			worldUp.Normalize();
		}

		// Försök hitta /forward/ från olika komponenter
		Object body;
		Vector3 forward;

		if (body = GetComponentInChildren<Rigidbody>())
		{
			// 3D Rigidbody
			forward = (body as Rigidbody).velocity;
		}
		else if (body = GetComponentInChildren<CharacterController>())
		{
			// 3D CharacterController
			forward = (body as CharacterController).velocity;
		}
		else if (body = GetComponentInChildren<Rigidbody2D>())
		{
			// 2D Rigidbody
			forward = (body as Rigidbody2D).velocity;
		}
		else
		{
			// Räkna ut jämförelse med förra steget
			forward = transform.position - oldPosition;
		}

		// 2D specifika uträkningar
		if (is2D == true) {
			worldUp = new Vector3(worldUp.x, worldUp.y).normalized;
			forward.z = 0;
		}
		
		// Platta till
		forward = Vector3.ProjectOnPlane(forward, worldUp);

		if (is2D == true) {
			forward = Quaternion.AngleAxis(-90, worldUp) * forward;
		}

		// Rör sig snabbt nog?
		if (forward.sqrMagnitude > Vector3.kEpsilon) {
			forward.Normalize();
			//print(Quaternion.LookRotation(forward, worldUp).eulerAngles.z);

			transform.rotation = Quaternion.LookRotation(forward, worldUp);
		}

		oldPosition = transform.position;
	}

}
