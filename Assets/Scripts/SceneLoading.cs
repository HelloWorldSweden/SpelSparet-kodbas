using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
 * Som exempel objekt för att ladda en annan scen.
 * Samt kan Unity UI buttons länkas till LoadScene funktionen
 */
public class SceneLoading : MonoBehaviour {

	// (Frivilligt) Ljud som kan spelas då skriptet byter scen
	public AudioSource soundOnLoadScene;

	public void LoadScene(string name) {
		// Ska ett ljud spelas?
		if (soundOnLoadScene != null && soundOnLoadScene.clip != null) {
			// Spela upp ljudet
			soundOnLoadScene.Play();

			// Ändra parent
			soundOnLoadScene.transform.SetParent(null);

			// Se till så den inte tas bort
			DontDestroyOnLoad(soundOnLoadScene);

			// Förstör ljudet efter den spelat om den inte ska repeteras
			if (!soundOnLoadScene.loop) {
				Destroy(soundOnLoadScene.gameObject, soundOnLoadScene.clip.length);
			}
		}

		// Ladda scenen
		SceneManager.LoadScene(name);
	}

}
