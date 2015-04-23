using UnityEngine;
using System.Collections;

public class GetQuat : MonoBehaviour {
	public Quaternion Quat;
	
	// Update is called once per frame
	void Update () {
		Quat = transform.rotation;
	}
}
