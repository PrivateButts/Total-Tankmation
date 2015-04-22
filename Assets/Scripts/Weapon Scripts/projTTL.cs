using UnityEngine;
using System.Collections;

public class projTTL : MonoBehaviour {
	public float startTime;
	public float failSafeTTL = 15;
	// Use this for initialization
	void Start () {
		startTime = Time.time;
	}
	/*
	// Update is called once per frame
	void Update () {
		if (Time.time - startTime > failSafeTTL) {
			Destroy (gameObject);
		}
	}*/
}
