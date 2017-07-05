using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Högst 1 DamageOnTrigger per GameObjekt
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
		if (damageIfSameTag || !other.CompareTag(gameObject.tag)) {
			// Skicka 'skada' meddelande till det den kolliderade med
			other.SendMessage("TakeDamage", damage, SendMessageOptions.DontRequireReceiver);

			// Förstör sig själv
			if (destroySelf) {
				Destroy(gameObject);
			}
		}
	}

	// Kollisions metod för 2D
	private void OnTriggerStay2D(Collider2D collider) {
		// Hämta main objektet att skicka meddelande till
		GameObject main = collider.gameObject;
		if (collider.attachedRigidbody) {
			main = collider.attachedRigidbody.gameObject;
		}

		// Annan funktion så man slipper skriva om
		DealDamage(main);
	}

	// Kollisions metod för 3D
	private void OnTriggerStay(Collider collider) {
		// Hämta main objektet att skicka meddelande till
		GameObject main = collider.gameObject;
		if (collider.attachedRigidbody) {
			main = collider.attachedRigidbody.gameObject;
		}

		// Annan funktion så man slipper skriva om
		DealDamage(main);
	}

	private void OnValidate() {
		// Tvinga damage variabelns värde att vara positivt eller 0
		damage = Mathf.Max(damage, 0);
	}

}
