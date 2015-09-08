//Fires the projectile

using UnityEngine;
using System.Collections;

public class ProjectileShooter : MonoBehaviour {
	public AudioSource shoot; //Sound file to play when shot fired
	float SoundTime;
	float LastShot;
	bool Playing;
    public float Volume; //Desired volume of sfx


	//Globals to import into from other objects
	GameObject [] Weapons;
	GameObject Trail;
	GameObject CurrentWeapon;
	GameObject Tank;
	TankController TankController;
	
    
    void Start () {
		//Initialize LastShot
		Tank = this.transform.parent.parent.parent.parent.gameObject; //Get the game object of the tank who is running the script
		TankController = Tank.GetComponent<TankController>(); //Get that tank's Tank Controller

        //Initialize times
        Playing = false;  
		SoundTime = Time.time;
		LastShot = Time.time;

		//Load the available weapons into the scripts weapons variable for later use.
		AvailWeapons weaponController = GameObject.FindGameObjectWithTag ("AvailWeapons").GetComponent<AvailWeapons>();
		Weapons = new GameObject[weaponController.weapon.Length];
		for (int i=0; i<weaponController.weapon.Length; i++) {
			//Debug.Log (weaponController.weapon[i].name);
			Weapons[i] = weaponController.weapon[i];
		}

        //Preload asset
		Trail = Resources.Load ("projectileTrail") as GameObject;
	}
	

	//Updates sound volume
	void Update () {
        //Only do this if sound is playing
		if (Playing == true) {
            //Sound timeout
			if (Time.time - SoundTime > 2) {
				shoot.Stop ();
				Playing = false;  
			}

            //Sound fade
			if (Time.time - SoundTime > 1){
				shoot.volume -= 1 * Time.deltaTime;
			}
		}
	}


    //Shoot function is called to actually fire a projectile
	public void Shoot(){
		//Check to make sure we haven't fired too recently
		if(Time.time - LastShot > 1){
            //Sound
			shoot.Play ();
			shoot.volume = Volume;
            Playing = true;
			SoundTime = Time.time;
            
            //Start of projectile launching code
			CurrentWeapon = Weapons[TankController.CurrentWeapon];
            GameObject projectile = Instantiate(CurrentWeapon) as GameObject; //Start preparing the projectile for launch
            Projectile projCont = projectile.GetComponent<Projectile>(); //Starting location of projectile

			//If the proj doesn't have a projectile component, use Mirv instead
            if (projCont == null){
				Mirv projCont2 = projectile.GetComponent<Mirv>();
				projCont2.owner = Tank;
			} else {
			projCont.owner = Tank;
			}

            //Projectile has been created, start setting the projectile in motion
			projectile.transform.position = transform.position + new Vector3(0,0,0);
			projectile.transform.rotation = transform.rotation;
			projectile.transform.Rotate (0,180,0);
			Rigidbody rb = projectile.GetComponent<Rigidbody>();
            rb.velocity = transform.rotation * new Vector3(0, 0, -TankController.Power); //Initial velocity relative to the empty that is firing it.
            LastShot = Time.time; //Update timer for next shot delay
		}
	}

}
