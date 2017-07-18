using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 
 * Detta skript lägger du till på kamera objektet.
 * 
 * För FPS sätter du /keepDistance/ till 0.
 * För tredjepersons vy sätter du /keepDistance/ till t.ex. 15.
 */
public class PlayerCamera3D : MonoBehaviour {

	// Distansen som kameran kommer försöka hållas på
	public float keepDistance = 0;

	// true = objektet kommer försöka vara framför väggar (så man inte ser igenom dem)
	public bool stopAtWall = true;

	// Endast relevant om stopAtWall = true. Bestämmer vilka lager som kameran kommer stanna framför.
	public LayerMask wallLayerMask = 1;

	[Header("Sensitivity")]

	/* 
	 * Negativa värden inverterar. Dvs: upp blir ned & ned blir upp
	 * Gäller för /verticalSpeed/, /horizontalSpeed/ samt /zoomSpeed/
	 */

	// Vertikal = upp & ned, norr & syd
	public float verticalSpeed = -4;
	// Horisontell = vänster & höger, väst & öst
	public float horizontalSpeed = 4;
	// Hur snabbt man zoomar. En 0'a betyder ingen zoom
	public float zoomSpeed = 1;

	// Vinkeln för kameran
	private Vector2 angle;
	// Avståndet för kameran
	private float distance;

	private void Start() {
		// Hämta start värden
		angle = transform.eulerAngles;
		distance = keepDistance;

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
		distance += Input.mouseScrollDelta.y * zoomSpeed;
		
		// Modifiera värdena så dem passar bättre
		angle.y = Mathf.DeltaAngle(0, angle.y);
		angle.x = Mathf.Clamp(angle.x, -40f, 80f);

		if (Mathf.Approximately(zoomSpeed, 0f)) {
			// Ingen zoom
			distance = keepDistance;
		} else {
			// Lås zoom mellan 25% och 150% av keepDistance
			distance = Mathf.Clamp(distance, keepDistance * 0.25f, keepDistance * 1.5f);
		}

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
			Ray ray = new Ray(transform.parent.position, -distance * transform.forward);
			RaycastHit hit;

			// Ta lite närmre så att man inte ser genom väggar lika enkelt
			float subtractDistance = distance * 0.15f;

			// Skapa en "stråle" som kollar efter kollision, kör if-satsen om den kolliderar med något
			if (Physics.Raycast(ray, out hit, distance, wallLayerMask)) {

				if (hit.distance < subtractDistance) {
					// Tillräckligt nära, ta bara punkten
					return hit.point;
				} else {
					// Ta den punkten + lite närmre mitten
					return hit.point + transform.forward * subtractDistance;
				}
			} else {
				return ray.origin + transform.forward * (subtractDistance - distance);
			}
		}

		// Ta bara fast avstånd ifrån parent
		return transform.parent.position - distance * transform.forward;
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
