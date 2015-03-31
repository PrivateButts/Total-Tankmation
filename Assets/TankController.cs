//Primary controller, contains the settings for all the subcontrollers

using UnityEngine;
using System.Collections;

public class TankController : MonoBehaviour{
	public float maxSpeed = 5;
	public float forwardSpeed = 20;
	public float turnSpeed  = 2;
	public Rigidbody rb;
	public float elevateRate = 1;
	public float turretRate = 1;
	public float maxElevation = 70;
	public float minElevation = -5;	
	public float power = 15;

	

	void Start(){

		rb = GetComponent<Rigidbody> ();

	}


	//Tank Movement, can't use axis due to conflict with turret controls
	void FixedUpdate(){
		rb.maxAngularVelocity = maxSpeed;
		float forwardMoveAmount = 0;
		float turnAmount = 0;
		if(Input.GetKey(KeyCode.W)){
			forwardMoveAmount = forwardSpeed;
		}
		if(Input.GetKey(KeyCode.S)){
			forwardMoveAmount = -forwardSpeed;
		}
		if(Input.GetKey(KeyCode.A)){
			turnAmount = turnSpeed;
		}
		if(Input.GetKey(KeyCode.D)){
			turnAmount = -turnSpeed;
		}



		transform.Rotate(0, -turnAmount, 0);
		rb.AddRelativeForce(0,0,-forwardMoveAmount);


		//Speed Limiter
		if (Mathf.Sqrt(Mathf.Pow (rb.velocity.z, 2) + Mathf.Pow (rb.velocity.x, 2)) > maxSpeed) {
			rb.drag = .95F;
		} else {
			rb.drag = .1F;
		}
		Debug.Log (Mathf.Sqrt(Mathf.Pow (rb.velocity.z, 2) + Mathf.Pow (rb.velocity.x, 2)));

	}

};