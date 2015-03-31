//What happens when the tank shell hits something

using UnityEngine;
using System.Collections;

public class Explode : MonoBehaviour {
	GameObject prefab;
	public float projTTL = 25;
	float startTime;
	void Start () {
		//grab explosion object from resources
		startTime = Time.time;
		prefab = Resources.Load ("Explosion1") as GameObject;
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
		//Destroy the projectile
		Destroy (gameObject);
		Debug.Log (transform.position);
	
	}
}
