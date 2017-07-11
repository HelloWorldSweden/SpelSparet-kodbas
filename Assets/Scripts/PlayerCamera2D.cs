using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Endsat 1 kamera skript per object
[DisallowMultipleComponent]
public class PlayerCamera2D : MonoBehaviour {

	public Camera cam;
	public float speed = 5;
	[Header("Zoom")]
	public float zoomMultiplier = 0.5f;
	public float zoomSpeed = 2;
	
	private float startZoom;
	
	private void Start() {
		// Fanns det en kamera?
		if (cam != null) {
			// Spara start zoomen
			startZoom = GetCameraZoom();
		} else {
			// Det fanns inte en kamera så stäng av skriptet
			this.enabled = false;
		}
	}

	private float GetCameraZoom() {
		if (cam != null) {
			// Hämta kamerans zoom inställning, baserat på om den är i perspektiv eller inte
			if (cam.orthographic == true) {
				return cam.orthographicSize;
			} else {
				return cam.fieldOfView;
			}
		} else {
			return 0;
		}
	}

	private void SetCameraZoom(float zoom) {
		if (cam != null) {
			// Sätt kamerans zoom inställningar, baserat på om den är i perspektiv eller inte
			if (cam.orthographic == true) {
				cam.orthographicSize = zoom;
			} else {
				cam.fieldOfView = zoom;
			}
		}
	}

	// Kör på "LateUpdate" istället för "Update", detta kommer tillåta resterande
	// skript att förflytta sig innan, så det inte blir lagg effekt
	private void LateUpdate() {
		// Hämta våra positioner och spara i nya variabler så vi kan modifiera dem
		// utan att ändra på objektens riktiga positioner.
		Vector3 camPosition = cam.transform.position;
		Vector3 targetPosition = transform.position;

		// Sätt till samma Z-värde eftersom vi räknar med 2D
		targetPosition.z = camPosition.z;

		// Räkna ut distansen till spelaren
		float dist = Vector3.Distance(camPosition, targetPosition);

		// Hur mycket den ska förflytta sig denna "frame"
		float moveDelta = dist * (dist + speed) * Time.deltaTime;
		// Förflytta sig
		cam.transform.position = Vector3.MoveTowards(camPosition, targetPosition, moveDelta);

		if (zoomSpeed > 0) {
			// Räkna ut vilken zoom vi siktar på
			float currentZoom = GetCameraZoom();
			float zoomTarget = startZoom + dist * zoomMultiplier;

			// Ändra zoomen närmre det värdet
			float newZoom = Mathf.Lerp(currentZoom, zoomTarget, Time.deltaTime * zoomSpeed);
			SetCameraZoom(newZoom);
		}
	}
	
	private void OnValidate() {
		// Tvinga zoomOut att vara positivt eller 0
		zoomSpeed = Mathf.Max(zoomSpeed, 0);

		// Tvinga speed att vara positivt eller 0
		speed = Mathf.Max(speed, 0);
	}

}
