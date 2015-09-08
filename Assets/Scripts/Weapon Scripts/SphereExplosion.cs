using UnityEngine;
using System.Collections;

public class SphereExplosion : MonoBehaviour {
	float created;
	public float explosionsize = 5;
	float expansion = .2F;
	float maxsize = 4.8F;

	public AudioSource expSFX;

	// Use this for initialization
	void Start () {
		expansion = explosionsize / 50F;
		maxsize = explosionsize - 0.2F;
		created = Time.time;
		expSFX.time = 0;
		expSFX.Play ();

	}
	// Update is called once per frame
	void Update () {
		Material mat = gameObject.GetComponent<Renderer>().material;
		transform.localScale = Vector3.Lerp (new Vector3 (1f, 1f, 1f), new Vector3 (maxsize, maxsize, maxsize), Time.time - created);
		if (Time.time > created + .25f) {
			if(mat.GetFloat("_ClipRange") <=0){
				Destroy(gameObject);
				return;
			}
			mat.SetFloat("_ClipRange", mat.GetFloat("_ClipRange") -.01f);
		}
	}
}