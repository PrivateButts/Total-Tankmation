//Fires the projectile

using UnityEngine;
using System.Collections;

public class ProjectileShooter : MonoBehaviour {
	public AudioSource shoot;
	float SoundTime;
	float LastShot;
	bool playing;
	// Use this for initialization
	GameObject [] Weapons;
	GameObject trail;
	GameObject currentWeapon;
	public float Volume;
	void Start () {
		//Initialize LastShot
		playing = false;
		SoundTime = Time.time;
		LastShot = Time.time;
		//preload the projectile
		AvailWeapons weaponController = GameObject.FindGameObjectWithTag ("AvailWeapons").GetComponent<AvailWeapons>();
		Weapons = new GameObject[weaponController.weapon.Length];
		for (int i=0; i<weaponController.weapon.Length; i++) {
			//Debug.Log (weaponController.weapon[i].name);
			Weapons[i] = weaponController.weapon[i];
		}
		trail = Resources.Load ("projectileTrail") as GameObject;
	}
	
	// Update is called once per frame
	void Update () {
		//Spacebar to fire
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
	public void Shoot(){
		//Check to make sure we haven't fired too recently
		if(Time.time - LastShot > 1){
			shoot.Play ();
			shoot.volume = Volume;
			playing = true;
			SoundTime = Time.time;
			checkWeapon();
			GameObject Tank = this.transform.parent.parent.parent.parent.gameObject;
			TankController TankController = Tank.GetComponent<TankController>();
			//Start preparing the projectile for launch
			GameObject projectile = Instantiate(currentWeapon) as GameObject;
			//Starting location of projectile
			Projectile projCont = projectile.GetComponent<Projectile>();
			if (projCont == null){
				Mirv projCont2 = projectile.GetComponent<Mirv>();
				projCont2.owner = gameObject.transform.parent.parent.parent.parent.gameObject;
			} else {

			projCont.owner = gameObject.transform.parent.parent.parent.parent.gameObject;
			}
			projectile.transform.position = transform.position + new Vector3(0,0,0);
			projectile.transform.rotation = transform.rotation;
			projectile.transform.Rotate (0,180,0);
			Rigidbody rb = projectile.GetComponent<Rigidbody>();
			//Initial velocity relative to the empty that is firing it.
			rb.velocity = transform.rotation * new Vector3(0,0,-TankController.power);
			//Update timer for next shot delay
			LastShot = Time.time;
		}
	}
	void checkWeapon(){
		GameObject controllerOBJ = GameObject.FindGameObjectWithTag ("TurnController");
		TurnController controller = controllerOBJ.GetComponent<TurnController> ();
		if (controller.currentWeapon < Weapons.Length) {
			currentWeapon = Weapons[controller.currentWeapon];
			Debug.Log ("Firing");
		} else {
			Debug.Log ("Error, invalid weapon");
		}
	}
}
