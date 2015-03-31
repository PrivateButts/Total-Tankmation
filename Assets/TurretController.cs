//Controls rotation of the turret

using UnityEngine;
using System.Collections;

public class TurretController : MonoBehaviour{

	void Start(){
	}
	
	
	
	void FixedUpdate(){
		GameObject Tank = GameObject.Find ("PlayerTank");
		TankController TankController = Tank.GetComponent<TankController>();

		float rotateAmount = 0;

		if(Input.GetKey(KeyCode.LeftArrow)){
			rotateAmount = TankController.turretRate;
		}
		if(Input.GetKey(KeyCode.RightArrow)){
			rotateAmount = -TankController.turretRate;
		}
		
		transform.Rotate(0, 0, -rotateAmount);

	
		
	}
	
};