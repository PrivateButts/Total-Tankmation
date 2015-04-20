using UnityEngine;
using System.Collections;

public class GameData : MonoBehaviour {
	// This keeps the Object in the scene unless explictly destroyed
	void Awake() {
		DontDestroyOnLoad(transform.gameObject);
	}

	// This is were all the logic for setting a loaded level with the data in this object
	void OnLevelWasLoaded(int Level){
		// Main Game
		if (Level == 1) {
			// Spawn Tanks
		}
	}
}
