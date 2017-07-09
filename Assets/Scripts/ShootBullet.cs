using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Viktigt att tänka på för detta script:
 * 
 *	- Om kulan ni avfyrar kan kollidera med spelaren så 
 *	kan det vara orsaken till att den flyger iväg konstigt.
 * 
 */

/* För att använda flera tangenter kan ni
 * lägga till flera ShootBullet komponenter
 */
public class ShootBullet : MonoBehaviour {

	/* Vilken knapp som kommer få scriptet att avfyra
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
	public string fireWithKey = "mouse 0";
	// Måste ställas in i inspektorn
	public GameObject bulletPrefab;
	// Kraften framåt för kulan
	public float bulletForce = 20;
	// Förstör skjutna klonen efter så många sekunder
	// Sätt till 0 (noll) för att avaktivera
	public float killCloneAfter = 5;

	[Header("Sound effects")]
	// (Frivilligt) Ljud som kan spelas när skriptet skjuter
	public AudioSource soundOnFire;
	
	private void Start() {
		// Ge error meddelande för att lättare kunna debugga
		if (bulletPrefab == null) {
			Debug.LogError("Field bulletPrefab cannot be null! (GameObject \"" + this.name + "\")");
		}
	}

	private void Update() {
		// Avbryter funktionen om bulletPrefab inte är satt
		if (bulletPrefab == null) {
			return;
		}

		/* Kolla om knappen blev precis nedtryckt.
		* 
		* Finns tre stycken funktioner man kan använda:
		* 
		* Input.GetKeyUp(KeyCode key)			Blev knappen precis nedtryckt? (Aktiveras endast 1 gång per knapptryck)
		* Input.GetKeyDown(KeyCode key)			Blev knappen precis släppt? (Aktiveras endast 1 gång per knapptryck)
		* Input.GetKey(KeyCode key)				Är knappen nedtryckt? (Aktiveras flera gånger per sekund)
		*/
		if (Input.GetKeyDown(fireWithKey) == true) {
			// Räkna ut värden som kommer användas
			Vector3 position = transform.position;
			Quaternion rotation = transform.rotation;

			// Skapa kopia av prefaben
			GameObject clone = Instantiate(bulletPrefab, position, rotation);

			// Döda klonen efter viss tid
			if (killCloneAfter > 0)
				Destroy(clone, killCloneAfter);

			// Hämta Rigidbody komponenten från kopian
			Rigidbody body3d = clone.GetComponentInChildren<Rigidbody>();
			// Kolla så att det fanns en först
			if (body3d != null) {
				// Applisera kraften
				Vector3 force3d = Vector3.forward * bulletForce;
				body3d.AddRelativeForce(force3d, ForceMode.Impulse);
			}
			
			// Gör samma som ovan fast för 2D Rigidbodies
			Rigidbody2D body2d = clone.GetComponentInChildren<Rigidbody2D>();
			// Kolla så att det fanns en först
			if (body2d != null) {
				// Applisera kraften
				Vector2 force2d = Vector2.right * bulletForce;
				body2d.AddRelativeForce(force2d, ForceMode2D.Impulse);
			}

			// Spela upp ljud
			if (soundOnFire != null) {
				soundOnFire.Play();
			}

		}
	}
	
	// Validerar det man skriver in i inspektorn
	// Överkurs, oroa er inte om detta
	private void OnValidate() {
		// Lås killCloneAfter så det inte blir några negativa värden
		if (killCloneAfter < 0) {
			killCloneAfter = 0;
		}

		// Ta bort överflödiga mellanslag och gör till små bokstäver
		fireWithKey = fireWithKey.Trim().ToLower();

		try {
			// Testa om Input.GetKey funkar med den tangenten
			Input.GetKey(fireWithKey);
		} catch {
			// Om inte så återställ värdet
			fireWithKey = "";
		}
	}

}
