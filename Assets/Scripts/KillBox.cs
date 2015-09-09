using UnityEngine;
using System.Collections;

public class KillBox : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}


    //Destroy objects exiting the killbox
	void OnTriggerExit(Collider other) {
		Debug.Log ("KB Exit" + other.name);
		Destroy (other.gameObject);
	}
}
