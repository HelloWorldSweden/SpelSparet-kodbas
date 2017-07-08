using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Högst 1 DamageOnCollision per GameObjekt
[DisallowMultipleComponent]
public class DamageOnCollision : MonoBehaviour {

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
		if (damageIfSameTag == true || other.CompareTag(gameObject.tag) == false) {
			// Skicka 'skada' meddelande till det den kolliderade med
			other.SendMessage("TakeDamage", damage, SendMessageOptions.DontRequireReceiver);

			// Förstör sig själv
			if (destroySelf == true) {
				Destroy(gameObject);
			}
		}
	}

	// Kollisions metod för 2D
	private void OnCollisionStay2D(Collision2D collision) {
		// Hämta main objektet att skicka meddelande till
		GameObject main = collision.gameObject;
		if (collision.rigidbody != null) {
			main = collision.rigidbody.gameObject;
		}

		// Annan funktion så man slipper skriva om
		DealDamage(main);
	}

	// Kollisions metod för 3D
	private void OnCollisionStay(Collision collision) {
		// Hämta main objektet att skicka meddelande till
		GameObject main = collision.gameObject;
		if (collision.rigidbody != null) {
			main = collision.rigidbody.gameObject;
		}

		// Annan funktion så man slipper skriva om
		DealDamage(main);
	}

	private void OnValidate() {
		// Tvinga damage variabelns värde att vara positivt eller 0
		damage = Mathf.Max(damage, 0);
	}

}
