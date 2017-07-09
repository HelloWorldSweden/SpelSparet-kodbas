using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementFPS : MonoBehaviour {

	public Camera headCamera;

	[Header("Input settings")]

	// Vad för tangenter styr spelaren?
	public Scheme control = Scheme.WASD;

	// Vertikal = upp & ned, norr & syd
	// Negativa värden inverterar den. Dvs: upp blir ned & ned blir upp
	public float verticalSpeed = -1;
	// Horisontell = vänster & höger, väst & öst
	public float horizontalSpeed = 1;

	// Vinkeln för kameran
	private Vector2 angle;

	private void Start() {
		angle = transform.eulerAngles;

		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}

	private void Update() {
		// Uppdatera från användarens rörelse
		angle.x += Input.GetAxis("Mouse Y") * verticalSpeed;
		angle.y += Input.GetAxis("Mouse X") * horizontalSpeed;

		// Omvandla värdet till intervallet mellan 
		angle.y = Mathf.DeltaAngle(0, angle.y);
		// Lås så man inte kan kolla upp förbi himmelen
		angle.x = Mathf.Clamp(angle.x, -89.5f, 89.5f);

		// Uppdatera kamerans rotation
		if (headCamera) {
			transform.eulerAngles = angle;
		}
	}

	// enum = en grupp av bestämda val. Värdet kan inte vara något annat än det som listas här
	public enum Scheme {
		WASD,
		ArrowKeys,
	}

}
