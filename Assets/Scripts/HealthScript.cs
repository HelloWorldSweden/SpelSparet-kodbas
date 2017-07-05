using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Högst 1 HealthScript per GameObjekt
[DisallowMultipleComponent]
public class HealthScript : MonoBehaviour {

	// Fiendens liv
	public int health = 20;
	// Max liv. Kan användas för att göra healthbars
	public int maxHealth = 20;

	// gör något när livet är 0 eller lägre
	public ZeroHealthAction atZeroHealth = ZeroHealthAction.destroySelf;
	
	private void TakeDamage(int damage) {
		// Sänk livet med värdet från /damage/ samt lås värdet inom intervallet 0 till /maxHealth/
		health = Mathf.Clamp(health - damage, 0, maxHealth);

		// Jämför o se om livet är för lågt
		if (health <= 0) {
			// Kolla vilket /enum/ värde som är valt
			if (atZeroHealth == ZeroHealthAction.destroySelf) {
				// Förstör spelobjektet detta script tillhör
				Destroy(gameObject);
			} else if (atZeroHealth == ZeroHealthAction.resetScene) {
				// Nollställ scenen genom att ladda om den
				SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
			}
		}
	}

	private void OnValidate() {
		// Tvinga health att vara positivt eller 0
		health = Mathf.Max(health, 0);
		// Öka maxHealth om health > maxHealth
		maxHealth = Mathf.Max(maxHealth, health);
	}

	// enum = Grupp av val
	public enum ZeroHealthAction {
		doNothing,		// Gör inget
		destroySelf,	// Förstörs via Destroy()
		resetScene,		// Laddar om scenen
	}

}
