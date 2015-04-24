using UnityEngine;
using System.Collections;

public class Mirv : MonoBehaviour {
	public float baseDamage = 45;
	public float damageAOE = 5;
	GameObject prefab;
	GameObject prefab2;
	public float TTS = 2;
	public float projTTL = 25;
	float startTime;
	public bool followcam = false;
	public GameObject mirvProjectiles;
	public float spreadSpeed = 10F;
	GameObject trail;
	float mirvTime;
	public GameObject owner;
	public int mirvCount = 4;
	public bool alive;



	// Use this for initialization
	void Start () {
		alive = true;
		mirvTime = Time.time;
		startTime = GameObject.FindGameObjectWithTag("TurnController").GetComponent<TurnController>().timeFired;
		prefab = Resources.Load ("Explosionsphere") as GameObject;
		prefab2 = Resources.Load ("Explosion1") as GameObject;
		//trail = Resources.Load ("projectileTrail") as GameObject;
		/*
		GameObject acttrail = Instantiate (trail) as GameObject;
		acttrail.transform.position = transform.position;
		acttrail.transform.rotation = transform.rotation;
		acttrail.rigidbody.velocity = rigidbody.velocity;*/
	}
	
	// Update is called once per frame
	void Update () {
		//Time to live on projectiles
		if (Time.time - startTime > projTTL) {
			Destroy(gameObject);
		} else if (Time.time - mirvTime > TTS) {
			SpawnMirv();
		}

	}


	void OnTriggerEnter(Collider other){
		if (other.tag != "KillBox" && other.tag != "Weapon" && other.tag != "Trail") {
			//Create explosion
			GameObject explosion = Instantiate (prefab) as GameObject;
			//Create explosion at the location where the collision occured
			explosion.transform.position = transform.position;
			explosion.GetComponent<SphereExplosion> ().explosionsize = damageAOE;
			explosion = Instantiate (prefab2) as GameObject;
			//Create explosion at the location where the collision occured
			explosion.transform.position = transform.position;
			Collider[] hits = Physics.OverlapSphere (gameObject.transform.position, damageAOE);
			int i = 0;
			while (i < hits.Length) {
				if (hits [i].tag == "AITank" || hits [i].tag == "Trail" || hits [i].tag == "PlayerTank") {
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

	void SpawnMirv(){
		float iSpread = 0.1F;
		for (int i = 0; i<mirvCount; i++) {
			GameObject mirvs = Instantiate (mirvProjectiles) as GameObject;
			mirvs.transform.position = transform.localPosition + transform.right * iSpread;
			mirvs.rigidbody.velocity = rigidbody.velocity + new Vector3(Random.Range (-1, 1), Random.Range (-1, 1), Random.Range (-1, 1)) * spreadSpeed;
			if (mirvs.GetComponent<Mirv> ()) {
				mirvs.GetComponent<Mirv> ().owner = owner;
			} else {
				mirvs.GetComponent<Projectile> ().owner = owner;
			}
		}
		transform.GetChild (0).GetComponent<ParticleSystem> ().emissionRate = 0;
		transform.DetachChildren ();
		Destroy (gameObject);
	
	}
}
