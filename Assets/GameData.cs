using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameData : MonoBehaviour {
	public Text PlayersIn, AIsIn;
	public int Players, AIs;


	// This keeps the Object in the scene unless explictly destroyed
	void Awake() {
		DontDestroyOnLoad(transform.gameObject);

	}

	
	// Grabbing Data and Switching Scenes
	public void OnGoButtonClicked(){

	}


	// This is were all the logic for setting a loaded level with the data in this object
	void OnLevelWasLoaded(int Level){
		// Main Game
		if (Level == 1) {
			// Spawn Tanks
		}
	}
}
