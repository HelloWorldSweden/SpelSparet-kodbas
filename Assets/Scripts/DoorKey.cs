using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 
 * << VAD GÖR SCRIPTET ? >> 
 *		Tillåter en dörröppnare ("DoorOpener") att kunna öppna låsta dörrar.
 * 
 * << VAR SÄTTER JAG SCRIPTET? >>
 *		På ett objekt som kommer representera nyckeln.
 * 
 * << SCRIPTET FUNKAR INTE UTAN... >>
 *		..att det finns dörröppnarscript ("DoorOpener") utplacerade på andra spelobjekt.
 * 
 * << VIKTIGT ATT NOTERA >>
 *		Detta script är baserat på avstånd till dörröppnarna ("DoorOpener") och INTE triggers.
 */
public class DoorKey : MonoBehaviour {

	// true = kräver att "dörröppnaren" (DoorOpener) har samma tagg som nyckeln
	// false = alla kan plocka upp nyckeln
	public bool requireSameTag = false;

	// true = förstör det spelobjekt som skriptet är fast på när nyckeln blir upplockad
	public bool destroyOnPickup = false;

	[Header("Sound effects")]
	// (Frivilligt) Ljud som kan spelas då objektet plockas upp
	public AudioSource soundOnPickup;

	void Update() {
		// Iterera genom varje dörröppnare... En gång per objekt gå igenom följande block
		// och spara just den iterationens dörröppnarscript ("DoorOpener") i variabeln /opener/
		foreach (DoorOpener opener in FindObjectsOfType<DoorOpener>()) {
			// Om inte kräver, eller om kräver och är samma
			if (requireSameTag == false || opener.tag == this.tag) {
				// Öppnaren är nära nog och har inte redan nyckel
				if (opener.NearEnough(transform.position) == true && opener.openLocked == false) {
					// "Ge nyckeln till objektet" - säg att den kan öppna låsta dörrar
					opener.openLocked = true;

					// Spela ljud?
					if (soundOnPickup != null && soundOnPickup.clip != null) {
						soundOnPickup.Play();

						// Rädda ljudet så den inte blir förstörd
						if (destroyOnPickup == true) {
							// Ändra parent
							soundOnPickup.transform.SetParent(this.transform.parent, worldPositionStays:true);
						}
					}

					if (destroyOnPickup == true) {
						// Förstör mig själv :C
						Destroy(gameObject);
					} else {
						// Avaktivera nyckeln (detta kommer göra så Update inte körs mer)
						this.enabled = false;
					}

				}
			}
		}// slut på foreach loopen

	}//slut på Update funktionen

}//slut på DoorKey klassen
