﻿//Primary controller, contains the settings for all the subcontrollers

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
	public float modifier = 1;
	public bool followcam = true;
	public float HP = 100;
	public string Type = "Tank";
	public bool playerControlled = true;
	public GameObject turret;
	public GameObject elevator;
	public ProjectileShooter gun;
	public float forwardMoveAmount = 0;
	public float turnAmount = 0;
	public GameObject mycamera;
	public float rotateAmount = 0;
	public float currentEl = 0;
	public float elevateAmount = 0;
	public bool destroyed = false;
	public float score = 0;
	GameObject prefab;

	void Start(){
		prefab = Resources.Load ("Smoke") as GameObject;
		rb = GetComponent<Rigidbody> ();

	}


	//Tank Movement, can't use axis due to conflict with turret controls
	void FixedUpdate(){
		rb.maxAngularVelocity = maxSpeed;

		transform.Rotate(0, -turnAmount, 0);
		rb.AddRelativeForce(0,0,-forwardMoveAmount);



		//Speed Limiter
		if (Mathf.Sqrt(Mathf.Pow (rb.velocity.z, 2) + Mathf.Pow (rb.velocity.x, 2)) > maxSpeed) {
			rb.drag = .95F;
		} else {
			rb.drag = .1F;
		}

	}

	void AddDamage(float damage){
		if (HP > 0) {

			GameObject turnControllerObj = GameObject.FindGameObjectWithTag ("TurnController");
			TurnController turnController = turnControllerObj.GetComponent<TurnController>();
			if (damage < HP) {
				Debug.Log ("Player " + turnController.player.ToString() + " Score +" + HP.ToString());
				turnController.tankController[turnController.player].score += damage;
			} else {
				Debug.Log ("Player " + turnController.player.ToString() + " Score +" + damage.ToString());
				turnController.tankController[turnController.player].score += HP;
			}
			HP -= damage;
			Quaternion notifRot;
			if(gameObject == turnController.players[turnController.player]){
				Debug.Log ("Shot self");
				notifRot = Quaternion.LookRotation(turret.transform.position -(turret.transform.position - turret.transform.up));
			} else {
				notifRot = Quaternion.LookRotation(gameObject.transform.position -turnController.players[turnController.player].transform.position);
				Debug.Log ("Shot other");
			}
			DamageNotif (damage.ToString ("F1"), 1F, 0F, 1F, notifRot);



			if (HP <= 0) {
				DamageNotif ("Destroyed", -1F, 0F, 1F, notifRot);
				Debug.Log ("Tank Destroyed");
				destroyed = true;
				GameObject smoke = Instantiate(prefab) as GameObject;
				smoke.transform.position = transform.position;
				//Destroy (gameObject);
			}
		}
	
	}

	void DamageNotif(string damage, float height, float center, float size, Quaternion notifRot){
		GameObject damageGameObject = (GameObject)Instantiate(Resources.Load ("Text Damage Display"), transform.position + new Vector3 (0, 2, 0), transform.rotation);
		damageGameObject.GetComponentInChildren<TextMesh>().text = damage;
		damageGameObject.GetComponentInChildren<TextMesh>().characterSize = size;
		damageGameObject.transform.position = damageGameObject.transform.position + new Vector3 (center, height, 0F);
		damageGameObject.transform.rotation = notifRot;
	}

};