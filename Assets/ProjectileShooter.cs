﻿//Fires the projectile

using UnityEngine;
using System.Collections;

public class ProjectileShooter : MonoBehaviour {
	public AudioSource shoot;
	float SoundTime;
	float LastShot;
	bool playing;
	// Use this for initialization
	GameObject prefab;
	void Start () {
		//Initialize LastShot
		playing = false;
		SoundTime = Time.time;
		LastShot = Time.time;
		//preload the projectile
		prefab = Resources.Load ("projectile") as GameObject;
	}
	
	// Update is called once per frame
	void Update () {
		//Spacebar to fire
		if (Input.GetKey (KeyCode.Space)) {
			//Check to make sure we haven't fired too recently
			if(Time.time - LastShot > 1){
				shoot.Play ();
				shoot.volume = 1;
				playing = true;
				SoundTime = Time.time;
				GameObject Tank = GameObject.Find ("TankHull");
				TankController TankController = Tank.GetComponent<TankController>();
				//Start preparing the projectile for launch
				GameObject projectile = Instantiate(prefab) as GameObject;
				//Starting location of projectile
				projectile.transform.position = transform.position + new Vector3(0,0,0);
				Rigidbody rb = projectile.GetComponent<Rigidbody>();
				//Initial velocity relative to the empty that is firing it.
				rb.velocity = transform.rotation * new Vector3(0,0,-TankController.power);
				//Update timer for next shot delay
				LastShot = Time.time;
			}
		}
		if (playing == true) {
			if (Time.time - SoundTime > 2) {
				shoot.Stop ();
				playing = false;  
			}
			if (Time.time - SoundTime > 1){
				shoot.volume -= 1 * Time.deltaTime;
			}
		}
	}
}