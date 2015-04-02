using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AITank : MonoBehaviour {
	//public Transform damageTransform;
	//public GameObject damagePrefab = Resources.Load ("Text Damage Display");

	public float HP = 100;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	void AddDamage(float damage = 1){
		HP -= damage;
		Debug.Log("Damage: " + damage + ", HP Remaining: " + HP);
		Debug.Log (HP);

		GameObject damageGameObject = (GameObject)Instantiate(Resources.Load ("Text Damage Display"), transform.position + new Vector3 (0, 2, 0), transform.rotation);
		damageGameObject.GetComponentInChildren<TextMesh>().text = damage.ToString();
		damageGameObject.transform.position = damageGameObject.transform.position + new Vector3 (0.5F, 1F, 0F);
		damageGameObject.transform.Rotate (0, 180, 0);

		if (HP <= 0) {
			Debug.Log("Tank Destroyed");
			Destroy (gameObject);
		}
	}
}
