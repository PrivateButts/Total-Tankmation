//Primary controller, contains the settings for all the subcontrollers

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TankController : MonoBehaviour{

	//Settings that define the tank's characteristics
	public float MaxSpeed = 5;
	public float ForwardSpeed = 20;
	public float TurnSpeed  = 2;
	public float ElevateRate = 1;
	public float TurretRate = 1;
	public float MaxElevation = 70;
	public float MinElevation = -5;
	public float MaxPower = 120;
	public float DeltaPower = 2;
	public float Power = 20;
	public float Modifier = 1;
	public bool Followcam = true;
	public float HP = 100;
	public string Type = "Tank";
	//public bool playerControlled = true;

	public Rigidbody Rb;
	//References to related game Objects
	public GameObject Turret;
	public GameObject Elevator;
	public GameObject Mycamera;
	public GameObject MySkyCam;
	GameObject PrefSmoke;
	//Gun Controller
	public ProjectileShooter Gun;


	public float ForwardMoveAmount = 0;
	public float TurnAmount = 0;

	public float RotateAmount = 0;
	public float CurrentEl = 40;
	public float ElevateAmount = 40;



	//Variables to store information about the tank's state needed by turn controller
	public float Score = 0;
	public bool Destroyed = false;
	public float ShotDistance = -1;
	public int Target = -1;
	public int CurrentWeapon = 0;
	public int CameraPref = 1;
	public float CameraAngle = 0;
	public float Fuel = 100;
    public bool Active;
    public int RecentHit;

	void Start(){
		//Prepare prefab
		PrefSmoke = Resources.Load ("Smoke") as GameObject;

		//Get own rigid body
		Rb = GetComponent<Rigidbody> ();

		//Randomize starting turret elevation
		float initialel = Random.Range (15F, 45F);
		Elevator.transform.Rotate (initialel, 0, 0);
		ElevateAmount = initialel;
		CurrentEl = initialel;

		//Randomize initial direction of the tank
		transform.Rotate(0, Random.Range (0, 360), 0);

        Active = false;
        RecentHit = 0;

	}


	//Tank Movement, can't use axis due to conflict with turret controls
	void FixedUpdate(){
        if (Active || RecentHit > 0) {
            Rb.maxAngularVelocity = MaxSpeed;
            if (RecentHit > 0) {
                RecentHit -= 1;
            }
        } else {
            Rb.maxAngularVelocity = 0;
        }
        
		//Move as instructed by turn controller
		transform.Rotate(0, -TurnAmount, 0);
		Rb.AddRelativeForce(0,0,-ForwardMoveAmount);



		//Speed Limiter
		if (Mathf.Sqrt(Mathf.Pow (Rb.velocity.z, 2) + Mathf.Pow (Rb.velocity.x, 2)) > MaxSpeed) {
			Rb.drag = .95F;
		} else {
			Rb.drag = .1F;
		}

	}


	//Process incoming damage notificaiton
	void AddDamage(float damage){

        RecentHit = 600;

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
						turnController.tankController[turnController.player].Score += damage;
					} else {
						Debug.Log ("Player " + turnController.player.ToString() + " Score +" + damage.ToString());
						turnController.tankController[turnController.player].Score += HP;
					}
				} else {
					if (damage < HP) {
						Debug.Log ("Player " + turnController.currentAI.ToString() + " Score +" + HP.ToString());
						turnController.AItankController[turnController.currentAI].Score += damage;
					} else {
						Debug.Log ("Player " + turnController.currentAI.ToString() + " Score +" + damage.ToString());
						turnController.AItankController[turnController.currentAI].Score += HP;
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
					notifRot = Quaternion.LookRotation(Turret.transform.position -(Turret.transform.position - Turret.transform.up));
				} else {
					notifRot = Quaternion.LookRotation(gameObject.transform.position -turnController.players[turnController.player].transform.position);
					Debug.Log ("Shot other");
				}
			} else {
				notifRot = Quaternion.LookRotation(Turret.transform.position -(Turret.transform.position - Turret.transform.up));
				Debug.Log ("Shot By AI");
			}
			DamageNotif (damage.ToString ("F1"), 1F, 0F, 2F, notifRot);


			//If HP has been reduced to below 0, tank destroyed, notify and set status
			if (HP <= 0) {
				DamageNotif ("Destroyed", -1F, 0F, 2F, notifRot);
				Debug.Log ("Tank Destroyed");
				Destroyed = true;
				GameObject smoke = Instantiate(PrefSmoke) as GameObject;
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