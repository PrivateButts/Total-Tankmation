//What happens when the tank shell hits something

using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {
	public float baseDamage = 45;
	public float damageAOE = 5;
	GameObject expSphere;
	GameObject expParticle;
	public float projTTL = 25;
	float startTime;
	public bool followcam = false;
	void Start () {
		//grab explosion object from resources
		startTime = Time.time;
		expSphere = Resources.Load ("Explosionsphere") as GameObject;
		expParticle = Resources.Load ("Explosion1") as GameObject;
	}

	void Update () {
		//Time to live on projectiles
		if (Time.time - startTime > projTTL) {
			Destroy(gameObject);
		}
	}

	void OnTriggerEnter(Collider other){
		if (other.tag != "KillBox") {
			//Create explosion
			GameObject explosion = Instantiate (expSphere) as GameObject;
			//Create explosion at the location where the collision occured
			explosion.transform.position = transform.position;
			explosion.GetComponent<SphereExplosion> ().explosionsize = damageAOE;
			explosion = Instantiate (expParticle) as GameObject;
			//Create explosion at the location where the collision occured
			explosion.transform.position = transform.position;
			Collider[] hits = Physics.OverlapSphere (gameObject.transform.position, damageAOE);
			int i = 0;
			while (i < hits.Length) {
				if (hits [i].tag == "Tank" || hits [i].tag == "Trail" || hits [i].tag == "PlayerTank") {
					float distance = Vector3.Distance (gameObject.transform.position, hits [i].gameObject.transform.position);
					if (distance < 1) {
						distance = 1;
					}
					float damage = baseDamage / distance;
					hits [i].SendMessage ("AddDamage", damage);
				}
				i++;
			}
			//Destroy the projectile
			transform.GetChild (0).GetComponent<ParticleSystem> ().emissionRate = 0;
			transform.DetachChildren();
			Destroy (gameObject);
		}
	}
}
