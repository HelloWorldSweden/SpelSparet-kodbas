using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
 * Som exempel objekt för att ladda en annan scen.
 * Samt kan Unity UI buttons länkas till LoadScene funktionen
 */
public class SceneLoading : MonoBehaviour {

	public void LoadScene(string name) {
		SceneManager.LoadScene(name);
	}

}
