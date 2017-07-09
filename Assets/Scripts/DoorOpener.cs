using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 
 * Skriptet som läggs på dem GameObject som ska kunna öppna dörrar.
 * Dörrar öppnas automatiskt när man är nära nog
 */
public class DoorOpener : MonoBehaviour {

	// Distansen som dörrar öppnas ifrån
	public float range = 7;

	// Kan objektet öppna låsta dörrar?
	public bool openLocked = false;

	[Space]

	// Ska man öppna via knapp? Annars öppnar den automatiskt
	public bool waitForInput = false;

	/* Vilken knapp som kommer få scriptet att öppna dörren
	 * Exempel värden ni kan skriva i inspektorn (utan citationstecken ""):
	 * 
	 * "mouse 0"		vänster musknapp
	 * "mouse 1"		mitten musknapp
	 * "mouse 2"		höger musknapp
	 * "e"				bokstaven e
	 * "left ctrl"		vänster kontrol (Ctrl)
	 * "right shift"	⇧ höger shift
	 * "left"			← vänster piltangent
	 * "right"			→ höger piltangent
	 * "return"			↵ Retur, enter (ny rad)
	 * "space"			Mellanslag
	 * 
	 * osv.
	 */
	public string openWithKey = "";

	// Ritar lite i editorn, men inte i själva spelet
	private void OnDrawGizmosSelected() {
		Gizmos.color = new Color(1, 0, 1, 0.5f);
		Gizmos.DrawWireSphere(transform.position, range);
	}

	// DoorScript frågar den ifall den är nära nog
	public bool NearEnough(Vector3 position) {
		return Vector3.Distance(transform.position, position) <= range;
	}

	// DoorScript frågar den ifall den vill öppna
	public bool WantToOpen(bool doorLocked) {
		// Öppna inte om det krävs nyckel men har ingen
		if (doorLocked == true && this.openLocked == false) {
			return false;
		}

		// Öppna bara om en knapp blev tryckt
		if (waitForInput == true) {
			return Input.GetKeyDown(openWithKey);
		}

		// Om ingen annan stoppar mig så svarar vi "Ja, öppna dörren!"
		return true;
	}

	// Validerar det man skriver in i inspektorn
	// Överkurs, oroa er inte om detta
	private void OnValidate() {
		// Ta bort överflödiga mellanslag och gör till små bokstäver
		openWithKey = openWithKey.Trim().ToLower();
		
		try {
			// Testa om Input.GetKey funkar med den tangenten
			Input.GetKey(openWithKey);
			waitForInput = true;
		} catch {
			// Om inte så återställ värdet
			openWithKey = "";
			waitForInput = false;
		}
	}

}
