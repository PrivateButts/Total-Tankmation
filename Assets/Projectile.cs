//What happens when the tank shell hits something

using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {
	public float baseDamage = 45;
	GameObject prefab;
	GameObject prefab2;
	public float projTTL = 25;
	float startTime;
	public bool followcam = false;
	void Start () {
		//grab explosion object from resources
		startTime = Time.time;
		prefab = Resources.Load ("Explosionsphere") as GameObject;
		prefab2 = Resources.Load ("Explosion1") as GameObject;
	}

	void Update () {
		//Time to live on projectiles
		if (Time.time - startTime > projTTL) {
			Destroy(gameObject);
		}
	}

	void OnTriggerEnter(Collider other){
		//Create explosion
		GameObject explosion = Instantiate(prefab) as GameObject;
		//Create explosion at the location where the collision occured
		explosion.transform.position = transform.position;
		explosion = Instantiate(prefab2) as GameObject;
		//Create explosion at the location where the collision occured
		explosion.transform.position = transform.position;
		Collider[] hits = Physics.OverlapSphere (gameObject.transform.position, 5);
		int i = 0;
		while (i < hits.Length) {
			if(hits[i].tag == "Tank" || hits[i].tag == "Trail" || hits[i].tag == "PlayerTank"){
				float distance = Vector3.Distance(gameObject.transform.position, hits[i].gameObject.transform.position);
				if(distance < 1){
					distance = 1;
				}
				Debug.Log ("Range = " + distance);
				float damage = baseDamage/distance;
				hits[i].SendMessage ("AddDamage", damage);
			}
			i++;
		}
		//Destroy the projectile
		Destroy (gameObject);
		Debug.Log (transform.position);
	
	}
}
