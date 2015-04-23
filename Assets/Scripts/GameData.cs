using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameData : MonoBehaviour {
	public List<PlayerData> tanks = new List<PlayerData>();
	public Text PlayersIn, AIsIn;
	public int Players, AIs;
	public float Seed, Scale, MinHeight;


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

		for (int i = 0; i < Players; i++) {
			PlayerData player = new PlayerData();
			player.IsAI = false;
			player.Name = GenerateName();
			player.Score = 0;
			player.Model = "default";

			tanks.Add (player);
		}

		for (int i = 0; i < AIs; i++) {
			PlayerData player = new PlayerData();
			player.IsAI = true;
			player.Name = GenerateName();
			player.Score = 0;
			player.Model = "default";
			
			tanks.Add (player);
		}
		Application.LoadLevel (1);
	}

	void Update(){

		if(Input.GetKeyUp(KeyCode.Escape)){
			Application.Quit ();
		}
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


	private string GenerateName(){
		string[] names = System.IO.File.ReadAllLines("names.txt");
		List<string> NameList = new List<string>(names);

		return NameList [Random.Range (0, 199)];
	}

	public class PlayerData{
		public bool IsAI; 
		public string Name; 
		public float Score; 
		public string Model;
	}
}
