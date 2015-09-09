//Cleanup finished explosion effects

using UnityEngine;
using System.Collections;

public class ExplosionCleanup : MonoBehaviour {
	float created;


	// Use this for initialization
	void Start () {
		created = Time.time;
	}
	

	//Remove explosion object after 2 seconds
	void Update () {
		if (Time.time - created > 2) {
			Destroy(gameObject);
		}
	}
}
