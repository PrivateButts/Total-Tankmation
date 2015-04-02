using UnityEngine;
using System.Collections;

public class FollowCam : MonoBehaviour {
	Camera cam;
	float start;
	bool followcam = false;
	// Use this for initialization
	void Start () {
		cam = GetComponent<Camera> ();
		GameObject parent = this.transform.parent.gameObject;
		//followcam = parent.projTTL;
		cam.transform.rotation = gameObject.transform.parent.transform.rotation;
		cam.enabled = false;
		start = Time.time;

	}
	
	// Update is called once per frame
	void Update () {
		if (Time.time - start > 0.2 && followcam == true) {
			cam.enabled = true;
		}
	}
}
