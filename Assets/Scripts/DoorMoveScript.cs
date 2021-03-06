﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 
 * << VAD GÖR SCRIPTET ? >> 
 *		Öppnar en dörr genom att förflytta den (skjutdörr varianten).
 * 
 * << VAR SÄTTER JAG SCRIPTET? >>
 *		På dörren. Räcker med det objekt som ska förflytta sig.
 * 
 * << SCRIPTET FUNKAR INTE UTAN... >>
 *		..att det finns dörröppnarscript ("DoorOpener") utplacerade på andra spelobjekt.
 *		
 *		..att dörröppnarscriptet som är i närheten kan öppna låsta dörrar ifall detta
 *		  scripts /locked/ fält är icheckat.
 * 
 * << VIKTIGT ATT NOTERA >>
 *		Dörren öppnas av ett annat script som heter "DoorOpener".
 * 
 *		Om /locked/ fältet är icheckat så kan dörren endast öppnas ifall
 *		det "DoorOpener" script som är i närheter har /openLocked/ icheckat.
 * 
 *		Finns ett systerscript vid namn "DoorRotateScript" som gör liknande grej.
 */
[DisallowMultipleComponent]
public class DoorMoveScript : MonoBehaviour {

	// Vilken (lokal) position dörren förflyttas när den är öppen
	public Vector3 openOffset = new Vector3(0,3,0);

	// Hur många sekunder det tar för dörren att öppnas
	public float openTime = 1;

	// Kräver att öppnaren har /openLocked/=true
	public bool locked = false;

	[Header("Sound effects")]
	// (Frivilligt) Ljud som kan spelas då dörren öppnas
	public AudioSource soundOnOpen;
	// (Frivilligt) Ljud som kan spelas då dörren stängs
	public AudioSource soundOnClose;

	// "private" för den behöver inte kommas åt via unity inspektorn
	Vector3 startValue;

	// Hur öppen dörren är, 0=stängd, 1=öppen
	float percentage = 0;
	// Vart den är påväg
	int target = 0;

	void Start() {
		// Hämta startvärdet. eulerAngles=rotation i grader (0° till 360°) och x,y,z form
		startValue = transform.localPosition;
	}

	public int CalculateTarget() {
		bool isOpen = target != 0;
		// Eftersom dörren förflyttas så kan vi inte använda dess egna position utan istället ett start värde
		Vector3 startPosition = transform.parent ? transform.parent.TransformPoint(startValue) : startValue;

		// Loopa igenom alla "dörröppnare"
		foreach (DoorOpener opener in FindObjectsOfType<DoorOpener>()) {
			// Jämför distansen mellan min och öppnarens positioner
			if (opener.NearEnough(startPosition) == true) {

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
	void CalculateAndPlaySoundEffects() {
		AudioSource audio = CalculateTargetAndAudio();

		// Finns ljudet?
		if (audio != null && audio.clip != null) {
			// Spela ljud
			audio.Play();
		}
	}

	void Update() {
		// Vilket värde /percentage/ ska röra sig mot + spela öppningsljud
		CalculateAndPlaySoundEffects();

		// Öka/sänk percentage beroende på ifall det är någon nära eller inte
		percentage = Mathf.MoveTowards(percentage, target, Time.deltaTime / openTime);

		// Mjukna av övergången lite
		float eased = EasingInOut(percentage);
		// Applisera rotationen
		transform.localPosition = Vector3.Lerp(startValue, startValue + openOffset, eased);
	}

	// Ganska mycket matte. Vad den åstakommer är så att värdet 0-1 går lite långsammare i början och i slutet.
	float EasingInOut(float t) {
		t *= 2;
		if (t < 1) return 0.5f * t * t;
		t--;
		return -0.5f * (t * (t - 2) - 1);
	}

}
