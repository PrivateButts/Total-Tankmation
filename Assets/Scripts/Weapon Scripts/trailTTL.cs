using UnityEngine;
using System.Collections;

public class trailTTL : MonoBehaviour {
	float created;
	float ttl;


	//Setup TTL system
	void Start () {
		ttl = gameObject.GetComponent<ParticleSystem> ().startLifetime;
		created = Time.time;
	}

	
	//Cleanup
	void Update () {
		if (Time.time - created > ttl + gameObject.GetComponent<ParticleSystem> ().duration) {
			Destroy(gameObject); //Destroy the trail object after standard timeout + duration of particle effect
		}
	}


    //Stop emmitting particles when hit by explosion
	void AddDamage(float damage){
		gameObject.GetComponent<ParticleSystem> ().emissionRate = 0;
	}


    //Stop emmiting particles when collision detected
	void OnCollistionEnter(){
		gameObject.GetComponent<ParticleSystem> ().emissionRate = 0;
	}
}
