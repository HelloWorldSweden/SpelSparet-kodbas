using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 
 * Läggs till på dörrobjektet
 * 
 * Kräver att ni har DoorOpener skriptet tillagt i projektet
 * (alltså bland filerna i projekt mappen, och inte bland spelobjekten i scenen)
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
	// Vart den är påväg, (0 eller 1)
	private int target = 0;

	private void Start() {
		// Hämta startvärdet
		startValue = transform.localRotation;
	}

	public int CalculateTarget() {
		bool isOpen = target != 0;

		// Loopa igenom alla "dörröppnare"
		foreach (DoorOpener opener in FindObjectsOfType<DoorOpener>()) {
			// Jämför distansen mellan min och öppnarens positioner
			if (opener.NearEnough(transform.position) == true) {
				
				if (isOpen) {
					// Är öppen, fortsätt vara öppen
					return target = 1;
				} else {
					// Är stängd, kolla om öppnaren vill öppna dörren
					if (opener.WantToOpen(this.locked)) {
						// Öppna dörren
						return target = 1;
					}
				}

			}
		}

		// Ingen "dörröppnare" var nära nog eller ville öppna, så stäng dörren
		return target = 0;
	}

	public AudioSource CalculateTargetAndAudio() {
		int oldTarget = target;
		CalculateTarget();

		// Skedde en förändring?
		if (target != oldTarget) {
			// Öppning eller stängning?
			if (target == 0) {
				// Öppningsljud
				return soundOnClose;
			} else {
				// Stängningsljud
				return soundOnOpen;
			}
		}

		// Ingendera
		return null;
	}

	// Egendefinierad funktion för att kolla om det är någon DoorOpener i närheten
	private void CalculateAndPlaySoundEffects() {
		AudioSource audio = CalculateTargetAndAudio();
		
		// Finns ljudet?
		if (audio != null && audio.clip != null) {
			// Spela ljud
			audio.Play();
		}
	}

	private void Update() {
		// Vilket värde /percentage/ ska röra sig mot + spela öppningsljud
		CalculateAndPlaySoundEffects();

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
