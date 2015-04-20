using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class AITank : MonoBehaviour {
	//public Transform damageTransform;
	//public GameObject damagePrefab = Resources.Load ("Text Damage Display");
	List<GameObject> tankList = new List<GameObject>();
	public float HP = 100;
	public bool AIactive = false;
	float startTurnTime;
	public float turnLength = 10;


	// Use this for initialization
	void Start () {
		GameObject [] temp;
		temp = GameObject.FindGameObjectsWithTag ("Tank");
		for (int i = 0; i < temp.Length; i++) {
			if(gameObject != temp[i])
			{
			tankList.Add(temp[i]);
			}
		}
		temp = GameObject.FindGameObjectsWithTag ("PlayerTank");
		for (int i = 0; i < temp.Length; i++) {
			tankList.Add(temp[i]);
		}
		Debug.Log (tankList.Count);
	}
	
	// Update is called once per frame
	void Update () {
		if (AIactive == true){
			Debug.Log ("AI Active");
			if (turnLength < Time.time - startTurnTime) {
				Debug.Log ("AI Timeout");
				AIactive = false;
			}
			if ((Time.time * 100) % 10 == 0) {

				Debug.Log ("Time Left: " + (turnLength -Time.time - startTurnTime).ToString());
			}
		}
	}

	public void activate(){
		AIactive = true;
		Debug.Log ("AI Player Activating");
		startTurnTime = Time.time;
	}

	void DamageNotif(string damage, float height, float center, float size, Quaternion notifRot){
		GameObject damageGameObject = (GameObject)Instantiate(Resources.Load ("Text Damage Display"), transform.position + new Vector3 (0, 2, 0), transform.rotation);
		damageGameObject.GetComponentInChildren<TextMesh>().text = damage;
		damageGameObject.GetComponentInChildren<TextMesh>().characterSize = size;
		damageGameObject.transform.position = damageGameObject.transform.position + new Vector3 (center, height, 0F);
		damageGameObject.transform.rotation = notifRot;
	}

	void AddDamage(float damage){
		if (HP > 0) {
			GameObject turnControllerObj = GameObject.FindGameObjectWithTag ("TurnController");
			TurnController turnController = turnControllerObj.GetComponent<TurnController>();
			if (damage < HP) {
				Debug.Log ("Player " + turnController.player.ToString() + " Score +" + HP.ToString());
				turnController.tankController[turnController.player].score += damage;
			} else {
				Debug.Log ("Player " + turnController.player.ToString() + " Score +" + damage.ToString());
				turnController.tankController[turnController.player].score += HP;
			}
			HP -= damage;
			Quaternion notifRot;

			notifRot = Quaternion.LookRotation(gameObject.transform.position -turnController.players[turnController.player].transform.position);
			Debug.Log ("Shot other");

			Debug.Log ("Damage: " + damage + ", HP Remaining: " + HP);
			Debug.Log (HP);

			DamageNotif (damage.ToString ("F1"), 1F, 0.5F, 1F, notifRot);

			if (HP <= 0) {
				DamageNotif ("Destroyed", -1F, 5.5F, 1.8F, notifRot);
				Debug.Log ("Tank Destroyed");
				Destroy (gameObject);
			}
		}
	}
}
