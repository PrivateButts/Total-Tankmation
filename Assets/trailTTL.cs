using UnityEngine;
using System.Collections;

public class trailTTL : MonoBehaviour {
	float created;
	float ttl;
	// Use this for initialization
	void Start () {
		ttl = gameObject.GetComponent<ParticleSystem> ().startLifetime;
		created = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
		if (Time.time - created > ttl + gameObject.GetComponent<ParticleSystem> ().duration) {
			Destroy(gameObject);
		}
	}

	void AddDamage(float damage){
		gameObject.GetComponent<ParticleSystem> ().emissionRate = 0;
	}
}
