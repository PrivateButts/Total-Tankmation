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

	void DamageNotif(string damage, float height, float center, float size){
		GameObject damageGameObject = (GameObject)Instantiate(Resources.Load ("Text Damage Display"), transform.position + new Vector3 (0, 2, 0), transform.rotation);
		damageGameObject.GetComponentInChildren<TextMesh>().text = damage;
		damageGameObject.GetComponentInChildren<TextMesh>().characterSize = size;
		damageGameObject.transform.position = damageGameObject.transform.position + new Vector3 (center, height, 0F);
		damageGameObject.transform.Rotate (0, 180, 0);
	}

	void AddDamage(float damage = 1){
		HP -= damage;
		Debug.Log("Damage: " + damage + ", HP Remaining: " + HP);
		Debug.Log (HP);

		DamageNotif (damage.ToString (), 1F , 0.5F, 1F);

		if (HP <= 0) {
			DamageNotif("Destroyed", -1F, 5.5F, 1.8F);
			Debug.Log("Tank Destroyed");
			Destroy (gameObject);
		}
	}
}
