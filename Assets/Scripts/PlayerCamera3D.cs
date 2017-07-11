using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera3D : MonoBehaviour {

	// Distansen som kameran kommer försöka hållas på
	public float keepDistance = 0;

	// true = objektet kommer försöka vara framför väggar (så man inte ser igenom dem)
	public bool stopAtWall = true;

	// Endast relevant om stopAtWall = true. Bestämmer vilka lager som kameran kommer stanna framför.
	public LayerMask wallLayerMask = 1;

	[Header("Sensitivity")]

	// Vertikal = upp & ned, norr & syd
	// Negativa värden inverterar den. Dvs: upp blir ned & ned blir upp
	public float verticalSpeed = -4;
	// Horisontell = vänster & höger, väst & öst
	public float horizontalSpeed = 4;

	// Vinkeln för kameran
	private Vector2 angle;

	private void Start() {
		// Hämta start vinkeln
		angle = transform.eulerAngles;

		// Lås och göm musen (går att visa igen med Esc knappen)
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}

	// Reset sker vid tillägning av skriptet samt vid 
	private void Reset() {
		if (transform.parent) {
			keepDistance = Vector3.Distance(transform.position, transform.parent.position);
		}
	}

	private void Update() {
		// Uppdatera från användarens rörelse
		angle.x += Input.GetAxis("Mouse Y") * verticalSpeed;
		angle.y += Input.GetAxis("Mouse X") * horizontalSpeed;

		// Omvandla värdet till intervallet mellan 
		angle.y = Mathf.DeltaAngle(0, angle.y);
		// Lås så man inte kan kolla upp förbi himmelen
		angle.x = Mathf.Clamp(angle.x, -40f, 80f);

		// Uppdatera kamerans rotation
		transform.eulerAngles = angle;

		// Uppdatera kamerans position
		transform.position = CalculatePosition();
	}

	public Vector3 CalculatePosition() {
		// Ingen parent; ingen uträkning. Ta bara samma sen tidigare
		if (transform.parent == null) return transform.position;

		if (stopAtWall == true) {
			/*
			 * Kolla efter objekt som kan vara ivägen
			 */
			Ray ray = new Ray(transform.parent.position, -keepDistance * transform.forward);
			RaycastHit hit;

			// Ta lite närmre så att man inte ser genom väggar lika enkelt
			float subtractDistance = keepDistance * 0.15f;

			// Skapa en "stråle" som kollar efter kollision, kör if-satsen om den kolliderar med något
			if (Physics.Raycast(ray, out hit, keepDistance, wallLayerMask)) {

				if (hit.distance < subtractDistance) {
					// Tillräckligt nära, ta bara punkten
					return hit.point;
				} else {
					// Ta den punkten + lite närmre mitten
					return hit.point + transform.forward * subtractDistance;
				}
			} else {
				return ray.origin + transform.forward * (subtractDistance - keepDistance);
			}
		}

		// Ta bara fast avstånd ifrån parent
		return transform.parent.position - keepDistance * transform.forward;
	}

	private void OnDrawGizmosSelected() {
		if (transform.parent) {
			Vector3 parent = transform.parent.position;
			Vector3 target = CalculatePosition();

			// Parent - target
			Gizmos.color = new Color(0, 1, 1, 0.5f);
			Gizmos.DrawLine(parent, target);

			// Sfär @ target
			Gizmos.color = Color.cyan;
			Gizmos.DrawSphere(target, 0.5f);

			// Target - self
			Gizmos.color = new Color(1, 0.1f, 0.1f, 0.5f);
			Gizmos.DrawLine(target, transform.position);
		}
	}

}
