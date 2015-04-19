using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class TurnController : MonoBehaviour {
	public GameObject [] players;
	public List<TankController> tankController = new List<TankController>();
	int numPlayers;
	//GUI Configuration
	public Text txtPlayers;
	public Text txtPlayer;
	public Text txtPower;
	public Text txtSpeed;
	public Text txtElevator;
	public Text txtResult;
	public Text txtWeapon;
	public Text txtScore;
	public int player = 0;
	public string[] weapons;
	public int currentWeapon = 0;
	public float starttime;
	public bool gameover = false;
	public bool controlsactive = true;
	public bool AIActive = false;
	public int currentAI = -1;
	List<GameObject> AItankList = new List<GameObject>();
	public List<TankController> AItankController = new List<TankController>();
	public List<TankController> AllTankConroller = new List<TankController>();
	public bool AITurnOver = true;
	public bool PlayerTurnOver = false;
	//Get objects to spawn for the Player and AI
	public GameObject playertanktospawn;
	public GameObject AItanktospawn;
	//Terrain object
	public Terrain terrainobj;
	//Set the spawn area
	public int spawnrangex = 200;
	public int spawnrangez = 200;
	int lastcamerea;
	int rotationDir = 0;
	public int numberofplayers = 1;
	public int numberofAIplayers = 1;








	// Use this for initialization
	void Start () {


		//Specify how many Players and AI players to spawn
		spawnTanks (numberofplayers, numberofAIplayers);


		/****************************
		 * 		Generate Array of player tank controllers	
		 * **************************/
		players = GameObject.FindGameObjectsWithTag ("PlayerTank");
		numPlayers = players.Length;
		txtPlayers.text = "Number of Players:" + numPlayers.ToString ();
		for (int i=0; i < numPlayers; i++) {
			tankController.Add(players[i].GetComponent<TankController>());
		}
		//Select a random player to go first
		player = Random.Range (0, players.Length);
		//Debug.Log (player);

		//Turn on camera and audio for the randomly selected first player
		tankController[player].mycamera.GetComponent<Camera>().enabled = true;
		tankController[player].mycamera.GetComponent<AudioListener>().enabled = true;
		lastcamerea = player;


		/************************************
		 * 	Generate List of Available Weapons
		 * *********************************/
		AvailWeapons weaponController = GameObject.FindGameObjectWithTag ("AvailWeapons").GetComponent<AvailWeapons>();
		weapons = new string[weaponController.weapon.Length];
		for (int i=0; i < weaponController.weapon.Length; i++) {
			weapons[i] = weaponController.weapon[i].name;
		}
		txtWeapon.text = weapons[currentWeapon];


		/*************************************
		*	Generate List of AI Tanks
		**************************************/
		GameObject [] temp;
		temp = GameObject.FindGameObjectsWithTag ("AITank");
		for (int i = 0; i < temp.Length; i++) {
			if(gameObject != temp[i]) //Not sure why I'm doing this
			{
				AItankList.Add(temp[i]);
				AItankController.Add (temp[i].GetComponent<TankController>());
			}
		}
		Debug.Log ("AI Tank list created with " + AItankList.Count + " members");
		currentAI = -1;

		/****************************************8
		 * 
		 * Create list of every tank
		 * 
		 * ***************************************/

		AllTankConroller = AItankController;
		Debug.Log (tankController.Count);
		for (int i = 0; i < tankController.Count; i++) {
			AllTankConroller.Add (tankController[i]);
			//Debug.Log (i);
		}
	}
	
	// Update is called once per frame
	void Update () {
		txtPlayer.text = "Current Player: " + (player + 1).ToString ();
		//Reset all movement control inputs to zero

		float modifier;
		//Control lockout is triggered when the current player fires
		if (controlsactive == false && AIActive == false && gameover == false) {
			//Debug.Log ("Checking to free controls");
			//Wait until all Weapon taged objects are done, this is currently all projectiles in flight, and all weapon animations
			GameObject [] activeWeapon = GameObject.FindGameObjectsWithTag ("Weapon");
			if (activeWeapon.Length == 0) {
				Debug.Log ("No Weapon Found");
				nextTurn ();


				//Check whether we have reached the higheset numbered player, select next player accordingly

			} else {
				int temptime = (int)(Time.time * 10F);
				if (temptime % 10 == 0) {
					//Debug.Log ("Active Weapon:" + activeWeapon [0].name);
				}
			}



			//If keyboard not locked out, its still a player's turn, allow controls to apply to current player's tank
		} else if (controlsactive == true && player < players.Length) {
			//Debug.Log (player);
			//Debug.Log (tankController.Count);
			tankController [player].forwardMoveAmount = 0;
			tankController [player].turnAmount = 0;
			tankController [player].rotateAmount = 0;
			tankController [player].elevateAmount = 0;
			/******************************************
			 * 
			 * 			KEYBOARD CONTROLS
			 * 
			 * ***************************************/
			//Moved all keyboard controls to one place
			if (Input.GetAxis ("Modifier") > 0) {
				modifier = .1F;
			} else {
				modifier = 1F;
			}
			//Tank Hull Controls
			//This could be simplified, I originally started with raw inputs, but have switched to allow players to configure from the unity game launcher
			if (Input.GetAxis ("HullMovement") > 0) {
				tankController [player].forwardMoveAmount = tankController [player].forwardSpeed;
			}
			if (Input.GetAxis ("HullMovement") < 0) {
				tankController [player].forwardMoveAmount = -tankController [player].forwardSpeed;
			}
			if (Input.GetAxis ("HullRotation") > 0) {
				tankController [player].turnAmount = tankController [player].turnSpeed * modifier;
			}
			if (Input.GetAxis ("HullRotation") < 0) {
				tankController [player].turnAmount = -tankController [player].turnSpeed * modifier;
			}
			//Gun Power Controls
			if (Input.GetAxis ("Power") > 0) {
				tankController [player].power += tankController [player].deltaPower * modifier;
				if (tankController [player].power > tankController [player].maxPower) {
					tankController [player].power = tankController [player].maxPower;
				}
			}
			if (Input.GetAxis ("Power") < 0) {
				tankController [player].power -= tankController [player].deltaPower * modifier;
				if (tankController [player].power < 0) {
					tankController [player].power = 0;
				}
			}
			//Turret Rotation
			if (Input.GetAxis ("TurretRotation") > 0) {
				tankController [player].rotateAmount = tankController [player].turretRate * modifier;
			}
			if (Input.GetAxis ("TurretRotation") < 0) {
				tankController [player].rotateAmount = -tankController [player].turretRate * modifier;
			}
			//Elevation
			if (Input.GetAxis ("Elevation") > 0) {
				if (tankController [player].currentEl < tankController [player].maxElevation) {
					tankController [player].elevateAmount = tankController [player].elevateRate * modifier;
				}
			}
			if (Input.GetAxis ("Elevation") < 0) {
				if (tankController [player].currentEl > tankController [player].minElevation) {
					tankController [player].elevateAmount = -tankController [player].elevateRate * modifier;
				}
				//FIRE!
			}
			if (Input.GetAxis ("Fire") > 0) {
				starttime = Time.time;
				tankController [player].gun.Shoot ();
				controlsactive = false;
				tankController [player].forwardMoveAmount = 0;
				tankController [player].turnAmount = 0;
				tankController [player].rotateAmount = 0;
				tankController [player].elevateAmount = 0;
			}

			if (Input.GetButtonDown ("ChangeWeapon")) {
				if (tankController[player].currentWeapon == weapons.Length - 1) {
					tankController[player].currentWeapon = 0;
				} else {
					tankController[player].currentWeapon++;
				}
			}

			//Update active tank based on keys being pressed
			if (controlsactive == true) {
				tankController [player].elevator.transform.Rotate (tankController [player].elevateAmount, 0, 0);
				tankController [player].currentEl += tankController [player].elevateAmount;
				tankController [player].turret.transform.Rotate (0, 0, -tankController [player].rotateAmount);
			}
			//Update GUI Elements
			txtPower.text = "Power: " + tankController [player].power.ToString ("F1");
			txtSpeed.text = "Speed: " + (Mathf.Sqrt (Mathf.Pow (tankController [player].rb.velocity.z, 2) + Mathf.Pow (tankController [player].rb.velocity.x, 2))).ToString ("F1");
			txtElevator.text = "Elevation: " + tankController [player].currentEl.ToString ("F1");
			txtScore.text = "Score: " + tankController [player].score.ToString ("F1");
			txtWeapon.text = weapons [tankController[player].currentWeapon];
		} else if (AIActive == true) {
			/*********************************************
			 * 
			 * What to do while an AI is active
			 * 
			 * ********************************************/
			if (AItankController [currentAI].destroyed == false) {
				//Choose a target 
				//If no target, or target destroyed, pick new target
				//Debug.Log ("AI Turn Loop");
				if ((AItankController [currentAI].target == -1) || (AllTankConroller [AItankController [currentAI].target].destroyed == true)) {
					Debug.Log ("Need New Target");
					AItankController [currentAI].target = -1;
					while (AItankController[currentAI].target == -1) {
						Debug.Log ("Old Target: " + AItankController [currentAI].target.ToString ());
						AItankController [currentAI].target = Random.Range (0, (AllTankConroller.Count));
						Debug.Log ("Choosing a target");
						Debug.Log ("New Target: " + AItankController [currentAI].target.ToString ());

						if (AllTankConroller [AItankController [currentAI].target].gameObject.name == AItankList [currentAI].name) {
							Debug.Log ("AI Tank Targeted Self");
							AItankController [currentAI].target = -1; 
						} else if (AllTankConroller [AItankController [currentAI].target].destroyed == true) {
							Debug.Log ("New Target Already Destroyed");
							AItankController [currentAI].target = -1; 
						}
					}
				}


				if (rotationDir == 0) {
					float dist = Vector3.Distance (AllTankConroller [AItankController [currentAI].target].gameObject.transform.position, AItankController [currentAI].gun.gameObject.transform.position);
					AItankController [currentAI].turret.transform.Rotate (0, 0, 1);
					if (dist > Vector3.Distance (AllTankConroller [AItankController [currentAI].target].gameObject.transform.position, AItankController [currentAI].gun.gameObject.transform.position)) {
						Debug.Log ("Positive Rotate");
						rotationDir = 1;
					} else {
						Debug.Log ("Negative Rotate");
						rotationDir = -1;
					}

				}
				float dist2 = Vector3.Distance (AllTankConroller [AItankController [currentAI].target].gameObject.transform.position, AItankController [currentAI].gun.gameObject.transform.position);
				AItankController [currentAI].turret.transform.Rotate (0, 0, rotationDir);
				//Debug.Log (dist2);
				if (dist2 < Vector3.Distance (AllTankConroller [AItankController [currentAI].target].gameObject.transform.position, AItankController [currentAI].gun.gameObject.transform.position)) {
					if (AItankController [currentAI].shotDistance > 0) {
						if (AItankController [currentAI].shotDistance > dist2) {
							AItankController [currentAI].power -= Random.Range (0F, 20F);
						} else {
							AItankController [currentAI].power += Random.Range (0F, 20F);
						}
					}
					AItankController [currentAI].gun.Shoot ();
					AIActive = false;
					rotationDir = 0;
				}
			}
		} else if (gameover == true) {
			if (Input.GetKeyDown (KeyCode.Return)) {  
				Application.LoadLevel (0);  
			}  

		}
	}

	void nextTurn(){
		//Once all the objects with the weapon tag have terminated disable camera and audio for the current player's camera
		Debug.Log ("AI turn: " + AITurnOver.ToString() + " Player Turn: " + PlayerTurnOver.ToString());
		if (gameOver ()) {
		} else {
			if (AITurnOver == true) {
				Debug.Log ("Trigger Player Turn");
				nextPlayer ();
			}
			if (PlayerTurnOver == true) {
				Debug.Log ("Trigger AI Turn");
				nextAI ();
			}
		}
	
	}

	void nextAI(){
		currentAI++;
		Debug.Log("AI Player: " + currentAI);
		if (currentAI >= AItankList.Count) {
			Debug.Log ("End AI Turn");
			currentAI = -1;
			AITurnOver = true;
			PlayerTurnOver = false;
		} else if (AItankController [currentAI].destroyed == true) {
			nextAI ();
		} else {
			//activate current AI
			AIActive = true;
		}
	}

	void nextPlayer(){
		player++;
		Debug.Log ("Player: " + player.ToString());
		if (player >= players.Length) {
			Debug.Log ("Last Player reached, no turn");
			player = -1;
			PlayerTurnOver = true;
			AITurnOver = false;
			return;
		} else {
			Debug.Log ("Player: " + player.ToString());
		}
		//Don't switch to tank if its been destroyed
		if (gameOver ()) {

		} else if (tankController [player].destroyed == false && PlayerTurnOver == false) {
			Debug.Log ("Activate Camera on next Player");
			swapCamera ();
			
			//Finally, enable controls if the player is alive
			controlsactive = true;
		} else {
			nextPlayer();
		}
	}
	void swapCamera(){
		//Once the next player is set, enable camera and audio for that player's camera
		tankController[lastcamerea].mycamera.GetComponent<Camera>().enabled = false;
		tankController[lastcamerea].mycamera.GetComponent<AudioListener>().enabled = false;
		tankController[player].mycamera.GetComponent<Camera>().enabled = true;
		tankController[player].mycamera.GetComponent<AudioListener>().enabled = true;
		lastcamerea = player;
	}

	//Player and AI spawner function
	void spawnTanks(int numplayers, int numAIs){
		//Loop creating Human Players
		for (int i=0; i<numplayers; i++) {
			//Random X,Z spawn coordinates inside specified range
			int x = Random.Range (-spawnrangex, spawnrangex);
			int	z = Random.Range (-spawnrangez, spawnrangez);
			//Determine Y coordinate, adjust by -400 based on terrain position, then add 2 to avoid spawning inside terrain
			int y = Mathf.RoundToInt(terrainobj.SampleHeight(new Vector3(x, 0, z)) - 398);
			//Spawn the player, set name, set position
			GameObject temptank = Instantiate(playertanktospawn) as GameObject;
			temptank.name = "Player " + (i).ToString();
			temptank.transform.position = new Vector3 (x, y, z);
		}
		//Loop creating AI players
		for (int i=0; i<numAIs; i++) {
			//Random X,Z spawn coordinates inside specified range
			int x = Random.Range (-spawnrangex, spawnrangex);
			int	z = Random.Range (-spawnrangez, spawnrangez);
			//Determine Y coordinate, adjust by -400 based on terrain position, then add 2 to avoid spawning inside terrain
			int y = Mathf.RoundToInt(terrainobj.SampleHeight(new Vector3(x, 0, z)) - 398);
			//Spawn the player, set name, set position
			GameObject temptank = Instantiate(AItanktospawn) as GameObject;
			temptank.name = "AI Tank " + (i).ToString();
			temptank.transform.position = new Vector3 (x, y, z);
		}
	}

	bool gameOver(){
			//Debug.Log("Attempted to load a dead tank");
		int destroyedplayers = 0;
		int destroyedAI = 0;
		for (int j = 0; j < players.Length; j++){
			if (tankController[j].destroyed == true)
				destroyedplayers++;
		}
		for (int j = 0; j < AItankList.Count; j++){
			if (AItankController[j].destroyed == true)
				destroyedAI++;
		}
		if (destroyedplayers + destroyedAI >= (players.Length + AItankList.Count) - 1) {
			tankController [0].mycamera.GetComponent<Camera> ().enabled = true;
			gameover = true;
			string dispResult = "Press Enter to Start a new game\n";
			if (destroyedplayers + destroyedAI == players.Length + AItankList.Count) {
				dispResult += "The game ended in a draw";
			} else if (destroyedplayers == players.Length - 1) {
				for (int j = 0; j < players.Length; j++) {
					if (tankController [j].destroyed == false)
						tankController[j].score += 500;
						dispResult += "Player " + (j + 1).ToString () + " has survived the round! +500 points";
				}
			} else {
				dispResult += "Humanity has been defeated";
			}
			dispResult = dispResult + "\n\nName\t\tScore";
			for( int i = 0; i < (players.Length); i++){
				Debug.Log (i);
				dispResult = dispResult + "\n" + players[i].name + "\t\t" + tankController[i].score.ToString();
			}
			for( int i = 0; i < (AItankList.Count); i++){
				Debug.Log (i);
				dispResult = dispResult + "\n" + AItankList[i].name + "\t\t" + AItankController[i].score.ToString();
			}
			txtResult.text = dispResult;
			return true;
		} else {
			return false;
		}
	}

}
