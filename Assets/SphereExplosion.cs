using UnityEngine;
using System.Collections;

public class SphereExplosion : MonoBehaviour {
	float created;
	public float explosionsize = 5;
	float expansion = .2F;
	float maxsize = 4.8F;
	// Use this for initialization
	void Start () {
		expansion = explosionsize / 25F;
		maxsize = explosionsize - 0.2F;
		created = Time.time;
	}
		
	// Update is called once per frame
	void Update () {
		if (gameObject.transform.localScale.y < maxsize) {
			gameObject.transform.localScale += new Vector3 (expansion, expansion, expansion);
		}
		if (Time.time - created > 2) {
			Destroy(gameObject);
		}
	}
}