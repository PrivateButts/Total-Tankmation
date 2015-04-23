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
		Players = int.Parse(PlayersIn.text);
		AIs = int.Parse(AIsIn.text);

		if (Players < 0 || AIs < 0) {
			// Show Error Message
			Debug.LogError("Can not have negative players or AIs");
			return;
		}

		Application.LoadLevel (1);
	}


	// This is were all the logic for setting a loaded level with the data in this object
	void OnLevelWasLoaded(int Level){
		// Main Game
		if (Level == 1) {
			TurnController TC = GameObject.FindGameObjectWithTag("TurnController").GetComponent<TurnController>();
			TC.numberofplayers = Players;
			TC.numberofAIplayers = AIs;
		}
	}
}
