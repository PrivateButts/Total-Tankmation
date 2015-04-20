using UnityEngine;
using System.Collections;

public class GameData : MonoBehaviour {
	private float TimerStart;
	private bool FirstTrans, SecondTrans;

	public float FirstTransTime, SecondTransTime;
	public Vector3 TransStart, TransMiddle, TransEnd;
	public Quaternion TransRStart, TransREnd;
	public Transform Camera;


	// This keeps the Object in the scene unless explictly destroyed
	void Awake() {
		DontDestroyOnLoad(transform.gameObject);

	}


	// These functions handle the menu transistion, pulling user data, and switching scenes
	public void OnPlayButtonClicked(){
		TimerStart = Time.time;
		FirstTrans = true;
	}

	public void OnGoButtonClicked(){

	}

	void Update(){
		if (FirstTrans) {
			if (Camera.position != TransMiddle) {
				Camera.position = Vector3.Lerp (TransStart, TransMiddle, (Time.time - TimerStart) / FirstTransTime);
			} else {
				TimerStart = Time.time;
				FirstTrans = false;
				SecondTrans = true;
			}
		}else if (SecondTrans) {
			if (Camera.position != TransEnd) {
				Camera.position = Vector3.Lerp (TransStart, TransMiddle, (Time.time - TimerStart) / FirstTransTime);
				Camera.rotation = Quaternion.Lerp(TransRStart, TransREnd, (Time.time - TimerStart) / FirstTransTime);
			} else {
				SecondTrans = false;
			}
		}
	}


	// This is were all the logic for setting a loaded level with the data in this object
	void OnLevelWasLoaded(int Level){
		// Main Game
		if (Level == 1) {
			// Spawn Tanks
		}
	}
}
