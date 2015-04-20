using UnityEngine;
using System.Collections;

public class MenuTrans : MonoBehaviour {
	// Transition Varbs
	private float TimerStart;
	private bool FirstTrans, SecondTrans;
	
	public float FirstTransTime, SecondTransTime;
	public Vector3 TransStart, TransMiddle, TransEnd;
	public Quaternion TransRStart, TransREnd;
	public Transform Camera;	

	// Menu Transition
	public void OnPlayButtonClicked(){
		TimerStart = Time.time;
		FirstTrans = true;
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
			if (Camera.rotation != TransREnd) {
				Camera.position = Vector3.Lerp (TransMiddle, TransEnd, (Time.time - TimerStart) / SecondTransTime);
				Camera.rotation = Quaternion.Lerp(TransRStart, TransREnd, (Time.time - TimerStart) / SecondTransTime);
			} else {
				SecondTrans = false;
			}
		}
	}

}
