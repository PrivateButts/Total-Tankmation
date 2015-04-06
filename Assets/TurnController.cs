using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TurnController : MonoBehaviour {
	GameObject [] players;
	TankController [] tankController = new TankController[10];
	int numPlayers;
	//GUI Configuration
	public Text txtPlayers;
	public Text txtPlayer;
	public Text txtPower;
	public Text txtSpeed;
	public Text txtElevator;
	public Text txtResult;
	int player = 0;
	bool gameover = false;
	bool controlsactive = true;
	// Use this for initialization
	void Start () {
		players = GameObject.FindGameObjectsWithTag ("PlayerTank");
		numPlayers = players.Length;
		txtPlayers.text = "Number of Players:" + numPlayers.ToString ();
		for (int i=0; i < numPlayers; i++) {
			tankController[i] = players[i].GetComponent<TankController>();
		}
		player = Random.Range (0, players.Length);
		Debug.Log (player);
		//Turn on camera and audio for the randomly selected first player
		tankController[player].mycamera.GetComponent<Camera>().enabled = true;
		tankController[player].mycamera.GetComponent<AudioListener>().enabled = true;
	}
	
	// Update is called once per frame
	void Update () {
		txtPlayer.text = "Current Player: " + (player + 1).ToString ();
		//Reset all movement control inputs to zero
		tankController[player].forwardMoveAmount = 0;
		tankController[player].turnAmount = 0;
		tankController[player].rotateAmount = 0;
		tankController[player].elevateAmount = 0;
		float modifier = 0;
		//Control lockout is triggered when the current player fires
		if (controlsactive == false && gameover == false) {
			//Wait until all Weapon taged objects are done, this is currently all projectiles in flight, and all weapon animations
			if (GameObject.FindWithTag ("Weapon") == null) {

				//Once all the objects with the weapon tag have terminated disable camera and audio for the current player's camera
				tankController[player].mycamera.GetComponent<Camera>().enabled = false;
				tankController[player].mycamera.GetComponent<AudioListener>().enabled = false;


				//Check whether we have reached the higheset numbered player, select next player accordingly
				if (player == players.Length - 1){
					player = 0;
				} else {
					player++;
				}
				//Don't switch to tank if its been destroyed
				if(tankController[player].destroyed == false){
					//Once the next player is set, enable camera and audio for that player's camera
					tankController[player].mycamera.GetComponent<Camera>().enabled = true;
					tankController[player].mycamera.GetComponent<AudioListener>().enabled = true;

					//Finally, enable controls if the player is alive
					controlsactive = true;
				} else {
					int destroyedplayers = 0;
					for (int j = 0; j < players.Length; j++){
						if (tankController[player].destroyed == true)
							destroyedplayers++;
					}
					if (destroyedplayers >= players.Length - 1){
						tankController[player].mycamera.GetComponent<Camera>().enabled = true;
						txtResult.text = "Game Has Ended";
						gameover = true;
					}
				}
			}



		//If keyboard not locked out, its still a player's turn, allow controls to apply to current player's tank
		} else if (controlsactive == true) {
			/******************************************
			 * 
			 * 			KEYBOARD CONTROLS
			 * 
			 * ***************************************/
			//Moved all keyboard controls to one place
			if (Input.GetKey (KeyCode.LeftShift)) {
				modifier = .1F;
			} else {
				modifier = 1F;
			}
			//Tank Hull Controls
			if(Input.GetKey(KeyCode.W)){
				tankController[player].forwardMoveAmount = tankController[player].forwardSpeed;
			}
			if(Input.GetKey(KeyCode.S)){
				tankController[player].forwardMoveAmount = -tankController[player].forwardSpeed;
			}
			if(Input.GetKey(KeyCode.A)){
				tankController[player].turnAmount = tankController[player].turnSpeed * modifier;
			}
			if(Input.GetKey(KeyCode.D)){
				tankController[player].turnAmount = -tankController[player].turnSpeed * modifier;
			}
			//Gun Power Controls
			if(Input.GetKey(KeyCode.PageUp)){
				tankController[player].power += tankController[player].deltaPower * modifier;
				if (tankController[player].power > tankController[player].maxPower){
					tankController[player].power = tankController[player].maxPower;
				}
			}
			if(Input.GetKey(KeyCode.PageDown)){
				tankController[player].power -= tankController[player].deltaPower * modifier;
				if (tankController[player].power < 0){
					tankController[player].power = 0;
				}
			}
			//Turret Rotation
			if(Input.GetKey(KeyCode.LeftArrow)){
				tankController[player].rotateAmount = tankController[player].turretRate * tankController[player].modifier;
			}
			if(Input.GetKey(KeyCode.RightArrow)){
				tankController[player].rotateAmount = -tankController[player].turretRate * tankController[player].modifier;
			}
			//Elevation
			if(Input.GetKey(KeyCode.UpArrow)){
				if (tankController[player].currentEl < tankController[player].maxElevation){
					tankController[player].elevateAmount = tankController[player].elevateRate * tankController[player].modifier;
				}
			}
			if(Input.GetKey(KeyCode.DownArrow)){
				if (tankController[player].currentEl > tankController[player].minElevation) {
					tankController[player].elevateAmount = -tankController[player].elevateRate * tankController[player].modifier;
				}
			//FIRE!
			}
			if (Input.GetKey (KeyCode.Space)){
				tankController[player].gun.Shoot();
				controlsactive = false;
			}

			//Update active tank based on keys being pressed
			tankController[player].elevator.transform.Rotate (tankController[player].elevateAmount, 0,  0);
			tankController[player].currentEl += tankController[player].elevateAmount;
			tankController[player].turret.transform.Rotate(0, 0, -tankController[player].rotateAmount);

			//Update GUI Elements
			txtPower.text = "Power: " + tankController[player].power.ToString ("F1");
			txtSpeed.text =  "Speed: " + (Mathf.Sqrt(Mathf.Pow (tankController[player].rb.velocity.z, 2) + Mathf.Pow (tankController[player].rb.velocity.x, 2))).ToString("F1");
			txtElevator.text = "Elevation: " + tankController[player].currentEl.ToString ("F1");
		}
	}
}
