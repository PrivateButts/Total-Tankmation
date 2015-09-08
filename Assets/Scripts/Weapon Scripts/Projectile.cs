//What happens when the tank shell hits something

using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {
	//Characteristics of the weapon
	public float BaseDamage = 45;
	public float DamageAOE = 5;
	public float RateDropoff = 1;
	public float ProjTTL = 25;


	//Explosion effects
	GameObject expSphere;
	GameObject expParticle;


    //Object management
	float startTime;
	public bool Followcam = false;
	public GameObject Owner;
	public float EmissionRate = 20;
	public bool Alive;


    //Initialization
	void Start () {
		Alive = true;
		//grab explosion object from resources
		startTime = Time.time;
		expSphere = Resources.Load ("Explosionsphere") as GameObject;
		expParticle = Resources.Load ("Explosion1") as GameObject;
		transform.GetChild (0).GetComponent<ParticleSystem> ().emissionRate = EmissionRate;
	}


    //Cleanup lost projectiles
	void Update () {
		//Time to live on projectiles
		if (Time.time - startTime > ProjTTL) {
			Destroy(gameObject);
		}
	}


    //Collision handler
	void OnTriggerEnter(Collider other){
		if (other.tag != "KillBox" && other.tag != "Weapon" && other.tag != "Trail") {
			//Create explosion
			GameObject explosion = Instantiate (expSphere) as GameObject;
			//Create explosion at the location where the collision occured
			explosion.transform.position = transform.position;
            explosion.GetComponent<SphereExplosion>().ExplosionSize = DamageAOE;
			explosion = Instantiate (expParticle) as GameObject;
			//Create explosion at the location where the collision occured
			explosion.transform.position = transform.position;
			explosion.GetComponent<ParticleSystem>().startSpeed = DamageAOE;
			Collider[] hits = Physics.OverlapSphere (gameObject.transform.position, DamageAOE);
			
            //Notify objects hit in the AoE
            int i = 0;
			while (i < hits.Length) {
				if (hits [i].tag == "AITank" || hits [i].tag == "Trail" || hits [i].tag == "PlayerTank") {
					float distance = Vector3.Distance (gameObject.transform.position, hits [i].gameObject.transform.position);
					if (distance < 1) {
						distance = 1; //Full damage
					}
					float damage = BaseDamage / (distance/RateDropoff); //Damage reduced by range
					if (damage > BaseDamage){
						damage = BaseDamage;  //Make extra sure damage is never higher then full damage
					}

                    //Actually send the damage notifications
					if(hits[i].gameObject == Owner.gameObject){
						hits [i].SendMessage ("AddDamage", -damage);//Special case for self inflicted damage
					} else {
						hits [i].SendMessage ("AddDamage", damage);
					}
				}

                //Terrain deformation
				if (hits[i].tag == "Terrain"){
					Debug.Log("Attemping to deform terrain");
					hits[i].gameObject.GetComponent<DestructableTerrain>().Crater (Mathf.RoundToInt(explosion.transform.position.x), Mathf.RoundToInt(explosion.transform.position.z), Mathf.RoundToInt(DamageAOE), 1000f);
				}
				i++;
			}

			//Tell owner how far it went
			Owner.GetComponent<TankController>().ShotDistance = Vector3.Distance(transform.position, Owner.transform.position);

			//Destroy the projectile
			transform.GetChild (0).GetComponent<ParticleSystem> ().emissionRate = 0;
			transform.DetachChildren();
			Destroy (gameObject);
		}
	}

}
