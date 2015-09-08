//Cleanup damage notifications

using UnityEngine;
using System.Collections;

public class DamageNotification : MonoBehaviour {
	public float TTL = 2;
	float created;


	// Use this for initialization
	void Start () {
		created = Time.time;
	}

	
	//TTL check for the damage notification
	void Update () {
		gameObject.transform.position += new Vector3(0F,0.1F,0F);
		if (Time.time - created > TTL) {
			Destroy(gameObject);
		}
	}
}
