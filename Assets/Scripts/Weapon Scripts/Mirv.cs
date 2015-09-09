using UnityEngine;
using System.Collections;

public class Mirv : MonoBehaviour {
	//Projectile Settings
    public float BaseDamage = 45;
	public float DamageAOE = 5;
    public float TTS = 2;
    public float ProjTTL = 25;
    public float SpreadSpeed = 10F;
    public int MirvCount = 4;
	


    //Object management
	GameObject prefab;
	GameObject prefab2;
	float startTime;
	public bool Followcam = false;
	public GameObject MirvProjectiles;
    GameObject trail;
	float mirvTime;
	public GameObject Owner;
	public bool Alive;



	// Use this for initialization
	void Start () {
		Alive = true;
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
	

    //TTL Garbage collection
	void Update () {
		//Time to live on projectiles
		if (Time.time - startTime > ProjTTL) {
			Destroy(gameObject);
		} else if (Time.time - mirvTime > TTS) {
			SpawnMirv();
		}

	}


    //Collision event handler
	void OnTriggerEnter(Collider other){
		if (other.tag != "KillBox" && other.tag != "Weapon" && other.tag != "Trail") {
			//Create explosion
			GameObject explosion = Instantiate (prefab) as GameObject;
			//Create explosion at the location where the collision occured
			explosion.transform.position = transform.position;
            explosion.GetComponent<SphereExplosion>().ExplosionSize = DamageAOE;
			explosion = Instantiate (prefab2) as GameObject;
			//Create explosion at the location where the collision occured
			explosion.transform.position = transform.position;
			Collider[] hits = Physics.OverlapSphere (gameObject.transform.position, DamageAOE);


            //Process everything within AOE effect
			int i = 0;
			while (i < hits.Length) {
				if (hits [i].tag == "AITank" || hits [i].tag == "Trail" || hits [i].tag == "PlayerTank") {
                    //Calculate applied damage
					float distance = Vector3.Distance (gameObject.transform.position, hits [i].gameObject.transform.position);
					if (distance < 1) {
						distance = 1; //Avoid increased damage beyond full damage
					}
					float damage = BaseDamage / distance; //Reduce damage by distance

                    //Actually send damage notifications
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


			//Destroy the projectile
			transform.GetChild (0).GetComponent<ParticleSystem> ().emissionRate = 0;
			transform.DetachChildren();
			Destroy (gameObject);
		}
	}

	void SpawnMirv(){
		float iSpread = 0.1F;
		for (int i = 0; i<MirvCount; i++) {
			GameObject mirvs = Instantiate (MirvProjectiles) as GameObject;
			mirvs.transform.position = transform.localPosition + transform.right * iSpread;
			mirvs.GetComponent<Rigidbody>().velocity = GetComponent<Rigidbody>().velocity + new Vector3(Random.Range (-1, 1), Random.Range (-1, 1), Random.Range (-1, 1)) * SpreadSpeed;
			if (mirvs.GetComponent<Mirv> ()) {
				mirvs.GetComponent<Mirv> ().Owner = Owner;
			} else {
				mirvs.GetComponent<Projectile> ().Owner = Owner;
			}
		}
		transform.GetChild (0).GetComponent<ParticleSystem> ().emissionRate = 0;
		transform.DetachChildren ();
		Destroy (gameObject);
	
	}
}
