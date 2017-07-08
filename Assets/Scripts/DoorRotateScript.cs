using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 
 * Läggs till på dörrobjektet
 * 
 * Kräver att ni har DoorOpener skriptet tillagt
 * 
 */
[DisallowMultipleComponent]
public class DoorRotateScript : MonoBehaviour {

	// Vilken (lokal) rotation dörren har när den är öppen
	public Vector3 openRotation = new Vector3(0,90,0);

	// Hur många sekunder det tar för dörren att öppnas
	public float openTime = 1;

	// Kräver att öppnaren har /hasKey/=true
	public bool locked = false;

	[Header("Sound effects")]
	// (Frivilligt) Ljud som kan spelas då dörren öppnas
	public AudioSource soundOnOpen;
	// (Frivilligt) Ljud som kan spelas då dörren stängs
	public AudioSource soundOnClose;

	// "private" för den behöver inte kommas åt via unity inspektorn
	private Quaternion startValue;

	// Hur öppen dörren är, 0=stängd, 1=öppen
	private float percentage = 0;
	// Vart den är påväg
	private float target = 0;

	private void Start() {
		// Hämta startvärdet
		startValue = transform.localRotation;
	}

	// Egendefinierad funktion för att kolla om det är någon DoorOpener i närheten
	private void CalculateTarget() {
		bool anyNear = false;

		foreach (DoorOpener opener in FindObjectsOfType<DoorOpener>()) {
			// Jämför distansen mellan min och öppnarens positioner
			if (opener.NearEnough(transform.position) == true) {
				anyNear = true;

				// Kolla om öppnaren vill öppna dörren
				if (opener.WantToOpen(locked) == true) {
					// Dörren ska öppnas!
					target = 1;

					// Spela ljud
					if (soundOnOpen != null && soundOnOpen.clip != null) {
						soundOnOpen.Play();
					}

					// Kan avbryta funktionen
					return;
				}
			}
		}
		
		if (anyNear == false) {
			// Dörren ska stängas!
			target = 0;

			// Spela ljud
			if (soundOnClose != null && soundOnClose.clip != null) {
				soundOnClose.Play();
			}
		}
	}

	private void Update() {
		// Vilket värde /percentage/ ska röra sig mot
		CalculateTarget();

		// Öka/sänk percentage beroende på ifall det är någon nära eller inte
		percentage = Mathf.MoveTowards(percentage, target, Time.deltaTime / openTime);

		// Mjukna av övergången lite
		float eased = EasingInOut(percentage);
		// Applisera rotationen
		transform.localRotation = Quaternion.Lerp(startValue, Quaternion.Euler(openRotation), eased);
	}

	// Ganska mycket matte. Vad den åstakommer är så att värdet 0-1 går lite långsammare i början och i slutet.
	private float EasingInOut(float t) {
		t *= 2;
		if (t < 1) return 0.5f * t * t;
		t--;
		return -0.5f * (t * (t - 2) - 1);
	}

}
