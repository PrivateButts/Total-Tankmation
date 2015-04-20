using UnityEngine;
using System.Collections;

public class GameData : MonoBehaviour {
	private float TimerStart;
	private bool FirstTrans, SecondTrans;
	public float FirstTransTimer, SecondTransTimer;

	// This keeps the Object in the scene unless explictly destroyed
	void Awake() {
		DontDestroyOnLoad(transform.gameObject);
	}

	// These functions handle the menu transistion, pulling user data, and switching scenes
	public void OnPlayButtonClicked(){
		Debug.Log ("Play Hit");
	}

	public void OnGoButtonClicked(){
		Debug.Log ("Go Hit");
	}


	// This is were all the logic for setting a loaded level with the data in this object
	void OnLevelWasLoaded(int Level){
		// Main Game
		if (Level == 1) {
			// Spawn Tanks
		}
	}
}
