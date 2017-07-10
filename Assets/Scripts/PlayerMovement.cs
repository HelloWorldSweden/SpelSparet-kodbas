using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

	// Vilka tangenter styr spelaren med?
	public Scheme control = Scheme.WASD;

	// Hur lång tid det tar spelaren att uppnå top hastighet i sekunder
	public float accelerationTime = 1;

	// Snabbaste hastigheten spelaren kan uppnå från att röra på sig, i enheter per sekund
	public float topSpeed = 5;

	// enum = en grupp av bestämda val. Värdet kan inte vara något annat än det som listas här
	public enum Scheme {
		WASD,
		ArrowKeys,
	}
}
