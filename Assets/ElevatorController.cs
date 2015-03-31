//Controls raising and lowering gun

using UnityEngine;
using System.Collections;

public class ElevatorController : MonoBehaviour{

	public float currentEl = 0;
	void Start(){
	}

	void FixedUpdate(){
		GameObject Tank = GameObject.Find ("TankHull");
		TankController TankController = Tank.GetComponent<TankController>();

		float elevateAmount = 0;
		if(Input.GetKey(KeyCode.UpArrow)){
			if (currentEl < TankController.maxElevation){
				elevateAmount = TankController.elevateRate;
			}
		}
		if(Input.GetKey(KeyCode.DownArrow)){
			if (currentEl > TankController.minElevation) {
				elevateAmount = -TankController.elevateRate;
			}

		}

		transform.Rotate (elevateAmount, 0,  0);
		currentEl += elevateAmount;
		
		
	}
	
};