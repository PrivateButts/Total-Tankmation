//Controls raising and lowering gun

using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class ElevatorController : MonoBehaviour{

	public float currentEl = 0;
	public Text elevator;
	void Start(){
	}

	void FixedUpdate(){
		GameObject Tank = GameObject.Find ("PlayerTank");
		TankController TankController = Tank.GetComponent<TankController>();

		float elevateAmount = 0;
		if(Input.GetKey(KeyCode.UpArrow)){
			if (currentEl < TankController.maxElevation){
				elevateAmount = TankController.elevateRate * TankController.modifier;
			}
		}
		if(Input.GetKey(KeyCode.DownArrow)){
			if (currentEl > TankController.minElevation) {
				elevateAmount = -TankController.elevateRate * TankController.modifier;
			}

		}

		transform.Rotate (elevateAmount, 0,  0);
		currentEl += elevateAmount;
		elevator.text = "Elevation: " + currentEl.ToString ("F1");
		
	}
	
};