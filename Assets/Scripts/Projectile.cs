//What happens when the tank shell hits something

using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {
	public float baseDamage = 45;
	public float damageAOE = 5;
	public float rateDropoff = 1;
	GameObject expSphere;
	GameObject expParticle;
	public float projTTL = 25;
	float startTime;
	public bool followcam = false;
	public GameObject owner;
	public float emissionRate = 20;
	public bool alive;
	void Start () {
		alive = true;
		//grab explosion object from resources
		startTime = Time.time;
		expSphere = Resources.Load ("Explosionsphere") as GameObject;
		expParticle = Resources.Load ("Explosion1") as GameObject;
		transform.GetChild (0).GetComponent<ParticleSystem> ().emissionRate = emissionRate;
	}

	void Update () {
		//Time to live on projectiles
		if (Time.time - startTime > projTTL) {
			Destroy(gameObject);
		}
	}

	void OnTriggerEnter(Collider other){
		if (other.tag != "KillBox" && other.tag != "Weapon" && other.tag != "Trail") {
			//Create explosion
			GameObject explosion = Instantiate (expSphere) as GameObject;
			//Create explosion at the location where the collision occured
			explosion.transform.position = transform.position;
			explosion.GetComponent<SphereExplosion> ().explosionsize = damageAOE;
			explosion = Instantiate (expParticle) as GameObject;
			//Create explosion at the location where the collision occured
			explosion.transform.position = transform.position;
			explosion.GetComponent<ParticleSystem>().startSpeed = damageAOE;
			Collider[] hits = Physics.OverlapSphere (gameObject.transform.position, damageAOE);
			int i = 0;
			while (i < hits.Length) {
				if (hits [i].tag == "AITank" || hits [i].tag == "Trail" || hits [i].tag == "PlayerTank") {
					float distance = Vector3.Distance (gameObject.transform.position, hits [i].gameObject.transform.position);
					if (distance < 1) {
						distance = 1;
					}
					float damage = baseDamage / (distance/rateDropoff);
					if (damage > baseDamage){
						damage = baseDamage;
					}

					hits [i].SendMessage ("AddDamage", damage);
				}
				i++;
			}
			//Tell owner how far it went
			owner.GetComponent<TankController>().shotDistance = Vector3.Distance(transform.position, owner.transform.position);

			//Destroy the projectile
			transform.GetChild (0).GetComponent<ParticleSystem> ().emissionRate = 0;
			transform.DetachChildren();
			Destroy (gameObject);
		}
	}

}
