using UnityEngine;
using System.Collections;

public class SphereExplosion : MonoBehaviour {
	float created;
	public float explosionsize = 5;
	public float expansion = .2F;
	float maxsize = 4.8F;

	public AudioSource expSFX;

	// Use this for initialization
	void Start () {
		expansion = explosionsize / 25F;
		maxsize = explosionsize - 0.2F;
		created = Time.time;
		expSFX.time = 0;
		expSFX.Play ();

	}
		
	// Update is called once per frame
	void Update () {
		transform.localScale = Vector3.Lerp (new Vector3 (1f, 1f, 1f), new Vector3 (maxsize, maxsize, maxsize), Time.time - created);
		if(Time.time > created + 2){
			Destroy(gameObject);
	}
}