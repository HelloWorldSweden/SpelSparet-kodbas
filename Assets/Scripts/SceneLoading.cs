using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/* 
 * << VAD GÖR SCRIPTET ? >> 
 *		Innehåller endast 1 funktion, och det är att ladda en scen.
 * 
 * << VAR SÄTTER JAG SCRIPTET? >>
 *		Förslagsvis på ett eget spelobjekt. (se "VIKTIGT ATT NOTERA")
 * 
 * << SCRIPTET FUNKAR INTE UTAN... >>
 *		..att den scen som ska laddas är tillagd i "Build Settings".
 *		  Lägg till scener i "Build Settings" genom att gå in på
 *		  "File > Build Settings..."
 *		  och sedan dra in dem scener ni önskar kunna öppna via script (inklusive detta script).
 * 
 * << VIKTIGT ATT NOTERA >>
 *		Används primärt i "Example Level selector" scenen där UI knapparna har UnityEvent
 *		inställningar som är kopplade till att avfyra detta scripts /LoadScene/ funktion.
 */
public class SceneLoading : MonoBehaviour {

	[Header("Sound effects")]
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

	// Ladda nuvarande scenen
	public void ReloadScene() {
		LoadScene(SceneManager.GetActiveScene().name);
	}

	// Avsluta spelet
	public void ExitGame() {
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
	}

}
