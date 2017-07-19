using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 
 * << VAD GÖR SCRIPTET ? >> 
 *		Gör skada på HealthScript.
 * 
 * << VAR SÄTTER JAG SCRIPTET? >>
 *		På ett objekt som har en trigger (både 2D och 3D funkar).
 * 
 * << SCRIPTET FUNKAR INTE UTAN... >>
 *		..att det finns en collider med /isTrigger/ icheckat. Alltså en trigger!
 *		
 *		..att objektet som kolliderades med har annan tagg ifall /damageIfSameTag/ fältet är urcheckat.
 * 
 * << VIKTIGT ATT NOTERA >>
 *		Ni kan även göra livpaket genom att sätta /damage/ fältet till ett negativt värde.
 *		
 *		Finns ett systerscript vid namn "DamageOnCollision" som gör liknande grej.
 */
[DisallowMultipleComponent]
public class DamageOnTrigger : MonoBehaviour {

	// Skadan som ska utges till objektet som kolliderar med
	// 0 skada gör inget
	public int damage = 5;

	// true = gör även skada till objektet oavsett vilken tagg varken har
	public bool damageIfSameTag = false;

	// Ska objektet förstöra sig själv vid kollision?
	public bool destroySelf = false;

	public void DealDamage(GameObject other) {
		// Avgör ifall ska skada objektet som kolliderades
		// Om damageIfSameTag == true, eller om objektet som kolliderades med har en annan tagg
		if (damageIfSameTag == true || other.tag != gameObject.tag) {
			// Skicka 'skada' meddelande till det den kolliderade med
			other.SendMessage("TakeDamage", damage, SendMessageOptions.DontRequireReceiver);

			// Förstör sig själv
			if (destroySelf == true) {
				Destroy(gameObject);
			}
		}
	}

	// Kollisions metod för 2D
	void OnTriggerEnter2D(Collider2D collider) {
		// Hämta main objektet att skicka meddelande till
		GameObject main = collider.gameObject;
		if (collider.attachedRigidbody != null) {
			main = collider.attachedRigidbody.gameObject;
		}

		// Annan funktion så man slipper skriva om
		DealDamage(main);
	}

	// Kollisions metod för 3D
	void OnTriggerEnter(Collider collider) {
		// Hämta main objektet att skicka meddelande till
		GameObject main = collider.gameObject;
		if (collider.attachedRigidbody != null) {
			main = collider.attachedRigidbody.gameObject;
		}

		// Annan funktion så man slipper skriva om
		DealDamage(main);
	}
	
}
