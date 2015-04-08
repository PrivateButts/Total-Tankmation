using UnityEngine;
using System.Collections;

public class Mirv : MonoBehaviour {
	public float baseDamage = 45;
	public float damageAOE = 5;
	GameObject prefab;
	GameObject prefab2;
	public float projTTL = 2;
	float startTime;
	public bool followcam = false;
	public GameObject mirvProjectiles;
	public float spreadSpeed = 10F;



	// Use this for initialization
	void Start () {
		startTime = Time.time;
		prefab = Resources.Load ("Explosionsphere") as GameObject;
		prefab2 = Resources.Load ("Explosion1") as GameObject;
	}
	
	// Update is called once per frame
	void Update () {
		//Time to live on projectiles
		if (Time.time - startTime > projTTL) {
			SpawnMirv();
		}
	}


	void OnTriggerEnter(Collider other){
		if (other.tag != "KillBox") {
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
			Destroy (gameObject);
		}
	}

	void SpawnMirv(){
		float iSpread = 1F;
		GameObject mirvs = Instantiate(mirvProjectiles) as GameObject;
		mirvs.transform.position = transform.localPosition + transform.right * iSpread;
		mirvs.rigidbody.velocity = rigidbody.velocity + transform.right * spreadSpeed;
		mirvs = Instantiate(mirvProjectiles) as GameObject;
		mirvs.transform.position = transform.localPosition + transform.right * -iSpread;
		mirvs.rigidbody.velocity = rigidbody.velocity + transform.right * -spreadSpeed;		
		mirvs = Instantiate(mirvProjectiles) as GameObject;
		mirvs.transform.position = transform.localPosition + transform.up * iSpread;
		mirvs.rigidbody.velocity = rigidbody.velocity + transform.up * spreadSpeed;		
		mirvs = Instantiate(mirvProjectiles) as GameObject;
		mirvs.transform.position = transform.localPosition + transform.up * -iSpread;
		mirvs.rigidbody.velocity = rigidbody.velocity + transform.up * -spreadSpeed;
		Destroy (gameObject);
	
	}
}
