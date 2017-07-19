using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 
 * << VAD GÖR SCRIPTET ? >> 
 *		Öppnar dörrscripten som är i närheten.
 * 
 * << VAR SÄTTER JAG SCRIPTET? >>
 *		På dem objekt som ska öppna dörrar när dem är i närheten av dem.
 *		Kan vara spelare och fiender.
 * 
 * << SCRIPTET FUNKAR INTE UTAN... >>
 *		..att det finns dörrscript ("DoorRotateScript" && "DoorMoveScript") utplacerade på andra spelobjekt.
 *		
 *		..att dörrscriptet som är i närheten inte är låst förutom ifall detta script har
 *		  /openLocke/ fältet icheckat.
 *		  
 *		..att /openWithKey/ knappen är klickad förutom ifall /waitForInput/ fältet är urcheckat.
 * 
 * << VIKTIGT ATT NOTERA >>
 *		De dörrscript som detta script öppnar är "DoorRotateScript" och "DoorMoveScript".
 *		
 *		Om /locked/ fältet är icheckat på dörrscriptet så kan dörren endast öppnas ifall
 *		det detta script har /openLocked/ fältet icheckat.
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
	void OnDrawGizmosSelected() {
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
	void OnValidate() {
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
