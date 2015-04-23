using UnityEngine;
using System.Collections;

public class MenuTrans : MonoBehaviour {
	// Transition Varbs
	private float TimerStart;
	private bool FirstTrans, SecondTrans, ToHelpTrans, FromHelpTrans;
	
	public float FirstTransTime, SecondTransTime, HelpTransTime;
	public Vector3 TransStart, TransMiddle, TransEnd, HelpLoc;
	public Quaternion TransRStart, TransREnd, HelpRot;
	public Transform Camera;

	// Menu Transition
	public void OnPlayButtonClicked(){
		TimerStart = Time.time;
		FirstTrans = true;
	}

	public void OnHelpClicked(){
		TimerStart = Time.time;
		ToHelpTrans = true;
	}

	public void OnBackClicked(){
		TimerStart = Time.time;
		FromHelpTrans = true;
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
		} else if (SecondTrans) {
			if (Camera.rotation != TransREnd) {
				Camera.position = Vector3.Lerp (TransMiddle, TransEnd, (Time.time - TimerStart) / SecondTransTime);
				Camera.rotation = Quaternion.Lerp (TransRStart, TransREnd, (Time.time - TimerStart) / SecondTransTime);
			} else {
				SecondTrans = false;
			}
		} else if (ToHelpTrans) {
			if (Camera.position != HelpLoc) {
				Camera.position = Vector3.Lerp (TransStart, HelpLoc, (Time.time - TimerStart) / HelpTransTime);
				Camera.rotation = Quaternion.Lerp (TransRStart, HelpRot, (Time.time - TimerStart) / HelpTransTime);
			} else {
				ToHelpTrans = false;
			}
		} else if (FromHelpTrans) {
			if (Camera.position != TransStart) {
				Camera.position = Vector3.Lerp (HelpLoc, TransStart, (Time.time - TimerStart) / HelpTransTime);
				Camera.rotation = Quaternion.Lerp (HelpRot, TransRStart, (Time.time - TimerStart) / HelpTransTime);
			} else {
				FromHelpTrans = false;
			}

		}
	}

}
