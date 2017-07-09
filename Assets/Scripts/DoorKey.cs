using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 
 * Läggs till på dörrobjektet
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

	private void Update() {
		// Sök efter "dörröppnare" och kolla om dem kan plocka upp den här nyckeln
		foreach (DoorOpener opener in FindObjectsOfType<DoorOpener>()) {
			// Om inte kräver, eller om kräver och är samma
			if (requireSameTag == false || opener.CompareTag(this.tag)) {
				// Öppnaren är nära nog och har inte redan nyckel
				if (opener.NearEnough(transform.position) == true && opener.openLocked == false) {
					// "Ge nyckeln till objektet" - säg att den kan öppna låsta dörrar
					opener.openLocked = true;

					// Spela ljud?
					if (soundOnPickup != null && soundOnPickup.clip != null) {
						soundOnPickup.Play();

						// Rädda ljudet så den inte blir förstörd
						if (destroyOnPickup) {
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
