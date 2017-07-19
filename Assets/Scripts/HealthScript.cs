using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/* 
 * << VAD GÖR SCRIPTET ? >> 
 *		Basic liv script för att hålla koll på objektets liv.
 * 
 * << VAR SÄTTER JAG SCRIPTET? >>
 *		Helst så "högt" som möjligt i parent-trappan (inte högt upp i listan utan "längre vänster" i hiarkin).
 *		På ett objekt som kan ha liv. T.ex. spelaren eller fiender.
 * 
 * << VIKTIGT ATT NOTERA >>
 *		Fältet /maxHealth/ definierar endast hur högt livet kan gå vid livpaket
 *		(t.ex vid DamageOnCollision som har negativ skada, dvs helar)
 */
[DisallowMultipleComponent]
public class HealthScript : MonoBehaviour {

	// Fiendens liv
	public int health = 20;
	// Max liv. Kan användas för att göra healthbars
	public int maxHealth = 20;

	// gör något när livet är 0 eller lägre
	public ZeroHealthAction atZeroHealth = ZeroHealthAction.destroySelf;

	[Header("Sound effects")]
	// (Frivilligt) Ljud som kan spelas då skriptet tar skada
	public AudioSource soundOnDamage;
	// (Frivilligt) Ljud som kan spelas då skriptet får 0 eller mindre liv
	public AudioSource soundOnDeath;

	void TakeDamage(int damage) {
		// Sänk livet med värdet från /damage/ samt lås värdet inom intervallet 0 till /maxHealth/
		health = Mathf.Clamp(health - damage, 0, maxHealth);

		// Jämför o se om livet är för lågt
		if (health <= 0) {
			// Kolla vilket /enum/ värde som är valt
			if (atZeroHealth == ZeroHealthAction.destroySelf) {
				// Spela ljud?
				if (soundOnDeath != null && soundOnDeath.clip != null) {
					// Spela ljud!
					soundOnDeath.Play();

					// Flytta längst upp i heirarkin
					soundOnDeath.transform.SetParent(null);

					// Förstör efter ljudets längd om den inte ska repetera
					if (soundOnDeath.loop == false) {
						Destroy(soundOnDeath.gameObject, soundOnDeath.clip.length);
					}
				}

				// Förstör spelobjektet detta script tillhör
				Destroy(gameObject);
			} else if (atZeroHealth == ZeroHealthAction.resetScene) {
				// Ska ett ljud spelas?
				if (soundOnDeath != null && soundOnDeath.clip != null) {
					// Spela upp ljudet
					soundOnDeath.Play();

					// Ändra parent
					soundOnDeath.transform.SetParent(null);

					// Se till så den inte tas bort
					DontDestroyOnLoad(soundOnDeath);

					// Förstör ljudet efter den spelat om den inte ska repeteras
					if (!soundOnDeath.loop) {
						Destroy(soundOnDeath.gameObject, soundOnDeath.clip.length);
					}
				}

				// Nollställ scenen genom att ladda om den
				SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
			}
		} else {
			// Dog inte, så spela ljud!
			if (soundOnDamage != null && soundOnDamage.clip != null) {
				soundOnDamage.Play();
			}
		}
	}

	void OnValidate() {
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
