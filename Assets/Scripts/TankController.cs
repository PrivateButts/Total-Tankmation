//Primary controller, contains the settings for all the subcontrollers

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TankController : MonoBehaviour{

	//Settings that define the tank's characteristics
	public float maxSpeed = 5;
	public float forwardSpeed = 20;
	public float turnSpeed  = 2;
	public float elevateRate = 1;
	public float turretRate = 1;
	public float maxElevation = 70;
	public float minElevation = -5;
	public float maxPower = 120;
	public float deltaPower = 2;
	public float power = 20;
	public float modifier = 1;
	public bool followcam = true;
	public float HP = 100;
	public string Type = "Tank";
	//public bool playerControlled = true;

	public Rigidbody rb;
	//References to related game Objects
	public GameObject turret;
	public GameObject elevator;
	public GameObject mycamera;
	public GameObject mySkyCam;
	GameObject prefSmoke;
	//Gun Controller
	public ProjectileShooter gun;


	public float forwardMoveAmount = 0;
	public float turnAmount = 0;

	public float rotateAmount = 0;
	public float currentEl = 40;
	public float elevateAmount = 40;



	//Variables to store information about the tank's state needed by turn controller
	public float score = 0;
	public bool destroyed = false;
	public float shotDistance = -1;
	public int target = -1;
	public int currentWeapon = 0;
	public int cameraPref = 1;
	public float cameraAngle = 0;
	public float fuel = 100;

	void Start(){
		//Prepare prefab
		prefSmoke = Resources.Load ("Smoke") as GameObject;

		//Get own rigid body
		rb = GetComponent<Rigidbody> ();

		//Randomize starting turret elevation
		float initialel = Random.Range (15F, 45F);
		elevator.transform.Rotate (initialel, 0, 0);
		elevateAmount = initialel;
		currentEl = initialel;

		//Randomize initial direction of the tank
		transform.Rotate(0, Random.Range (0, 360), 0);

	}


	//Tank Movement, can't use axis due to conflict with turret controls
	void FixedUpdate(){
		rb.maxAngularVelocity = maxSpeed;

		//Move as instructed by turn controller
		transform.Rotate(0, -turnAmount, 0);
		rb.AddRelativeForce(0,0,-forwardMoveAmount);



		//Speed Limiter
		if (Mathf.Sqrt(Mathf.Pow (rb.velocity.z, 2) + Mathf.Pow (rb.velocity.x, 2)) > maxSpeed) {
			rb.drag = .95F;
		} else {
			rb.drag = .1F;
		}

	}


	//Process incoming damage notificaiton
	void AddDamage(float damage){
		//Make sure you still have HP left to damage
		if (HP > 0) {
			GameObject turnControllerObj = GameObject.FindGameObjectWithTag ("TurnController");
			TurnController turnController = turnControllerObj.GetComponent<TurnController>();
			//Apply damage to the correct player/AI's score, limiting score to health remaining
			if(damage < 0){
				Debug.Log ("Shot self");
				damage = damage * -1;
			} else {
				if(turnController.AITurnOver){
					if (damage < HP) {
						Debug.Log ("Player " + turnController.player.ToString() + " Score +" + HP.ToString());
						turnController.tankController[turnController.player].score += damage;
					} else {
						Debug.Log ("Player " + turnController.player.ToString() + " Score +" + damage.ToString());
						turnController.tankController[turnController.player].score += HP;
					}
				} else {
					if (damage < HP) {
						Debug.Log ("Player " + turnController.currentAI.ToString() + " Score +" + HP.ToString());
						turnController.AItankController[turnController.currentAI].score += damage;
					} else {
						Debug.Log ("Player " + turnController.currentAI.ToString() + " Score +" + damage.ToString());
						turnController.AItankController[turnController.currentAI].score += HP;
					}
				}

			}
			//Actually inflict damage
			HP -= damage;

			//Display damage notification popup
			Quaternion notifRot;
			if(turnController.player >= 0){
				if(gameObject == turnController.players[turnController.player]){
					Debug.Log ("Shot self");
					notifRot = Quaternion.LookRotation(turret.transform.position -(turret.transform.position - turret.transform.up));
				} else {
					notifRot = Quaternion.LookRotation(gameObject.transform.position -turnController.players[turnController.player].transform.position);
					Debug.Log ("Shot other");
				}
			} else {
				notifRot = Quaternion.LookRotation(turret.transform.position -(turret.transform.position - turret.transform.up));
				Debug.Log ("Shot By AI");
			}
			DamageNotif (damage.ToString ("F1"), 1F, 0F, 2F, notifRot);


			//If HP has been reduced to below 0, tank destroyed, notify and set status
			if (HP <= 0) {
				DamageNotif ("Destroyed", -1F, 0F, 2F, notifRot);
				Debug.Log ("Tank Destroyed");
				destroyed = true;
				GameObject smoke = Instantiate(prefSmoke) as GameObject;
				smoke.transform.position = transform.position;
				//Destroy (gameObject);
			}
		}
	
	}

	//Actual spawner of notification
	void DamageNotif(string damage, float height, float center, float size, Quaternion notifRot){
		GameObject damageGameObject = (GameObject)Instantiate(Resources.Load ("Text Damage Display"), transform.position + new Vector3 (0, 2, 0), transform.rotation);
		damageGameObject.GetComponentInChildren<TextMesh>().text = damage;
		damageGameObject.GetComponentInChildren<TextMesh>().characterSize = size;
		damageGameObject.transform.position = damageGameObject.transform.position + new Vector3 (center, height, 0F);
		damageGameObject.transform.rotation = notifRot;
	}

};