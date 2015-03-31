//Primary controller, contains the settings for all the subcontrollers

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TankController : MonoBehaviour{
	public float maxSpeed = 5;
	public float forwardSpeed = 20;
	public float turnSpeed  = 2;
	public Rigidbody rb;
	public float elevateRate = 1;
	public float turretRate = 1;
	public float maxElevation = 70;
	public float minElevation = -5;
	public float maxPower = 120;
	public float deltaPower = 2;
	public float power = 20;
	public Text txtPower;
	public Text txtSpeed;

	

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
		if(Input.GetKey(KeyCode.PageUp)){
			power += deltaPower;
			if (power > maxPower){
				power = maxPower;
			}
		}
		if(Input.GetKey(KeyCode.PageDown)){
			power -= deltaPower;
			if (power < 0){
				power = 0;
			}
		}




		transform.Rotate(0, -turnAmount, 0);
		rb.AddRelativeForce(0,0,-forwardMoveAmount);

		txtPower.text = "Power: " + power.ToString ();

		//Speed Limiter
		if (Mathf.Sqrt(Mathf.Pow (rb.velocity.z, 2) + Mathf.Pow (rb.velocity.x, 2)) > maxSpeed) {
			rb.drag = .95F;
		} else {
			rb.drag = .1F;
		}
		txtSpeed.text =  "Speed: " + (Mathf.Sqrt(Mathf.Pow (rb.velocity.z, 2) + Mathf.Pow (rb.velocity.x, 2))).ToString("F1");

	}

};