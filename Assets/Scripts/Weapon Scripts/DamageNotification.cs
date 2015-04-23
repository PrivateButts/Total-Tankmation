//Cleanup finished explosion effects

using UnityEngine;
using System.Collections;

public class DamageNotification : MonoBehaviour {
	public float ttl = 2;
	float created;
	// Use this for initialization
	void Start () {
		created = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
		gameObject.transform.position += new Vector3(0F,0.1F,0F);
		if (Time.time - created > ttl) {
			Destroy(gameObject);
		}
	}
}
