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
			rotateAmount = TankController.turretRate * TankController.modifier;
		}
		if(Input.GetKey(KeyCode.RightArrow)){
			rotateAmount = -TankController.turretRate * TankController.modifier;
		}
		
		transform.Rotate(0, 0, -rotateAmount);

	
		
	}
	
};