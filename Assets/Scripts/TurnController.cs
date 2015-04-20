using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class TurnController : MonoBehaviour {
	//GUI Configuration
	public Text txtPlayers;
	public Text txtPlayer;
	public Text txtPower;
	public Text txtSpeed;
	public Text txtElevator;
	public Text txtResult;
	public Text txtWeapon;
	public Text txtScore;


	//Weapon stuff
	public string[] weapons;


	//Turn processing
	public bool gameover = false;
	public bool controlsactive = true;
	public bool AIActive = false;
	public bool AITurnOver = true;
	public bool PlayerTurnOver = false;


	//Tank and Controller related
	public int player = 0;
	public int currentAI = -1;
	public GameObject [] players; //TODO: Convert this to a list for consistance
	int numPlayers;
	List<GameObject> AItankList = new List<GameObject>();
	public List<TankController> tankController = new List<TankController>();
	public List<TankController> AItankController = new List<TankController>();
	public List<TankController> AllTankConroller = new List<TankController>(); //TODO: get rid of the need to have multiple lists of controllers
	public float timeFired;


	//Spawing Controls
	public GameObject playertanktospawn;
	public GameObject AItanktospawn;
	//How many Players/AI to spawn
	public int numberofplayers = 1;
	public int numberofAIplayers = 1;
	//Set the spawn area
	public int spawnrangex = 200;
	public int spawnrangez = 200;


	//Terrain object
	public Terrain terrainobj;


	//Camera settings
	int lastcamera;
	int rotationDir = 0;
	bool skyCamActive = false;










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
		lastcamera = numberofAIplayers + player;


		/************************************
		 * 	Generate List of Available Weapons
		 * *********************************/
		AvailWeapons weaponController = GameObject.FindGameObjectWithTag ("AvailWeapons").GetComponent<AvailWeapons>();
		weapons = new string[weaponController.weapon.Length];
		for (int i=0; i < weaponController.weapon.Length; i++) {
			weapons[i] = weaponController.weapon[i].name;
		}
		txtWeapon.text = weapons[0];


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
				if (temptime % 20 == 0) {
					Debug.Log ("Active Weapon:" + activeWeapon [0].name);
					Debug.Log (activeWeapon[0]);
					Debug.Log (Time.time - timeFired);
				}
				/*if(Time.time - activeWeapon[0].GetComponent<projTTL>().startTime > activeWeapon[0].GetComponent<projTTL>().failSafeTTL){
					Debug.Log ("Soft Error Correcting");
					DestroyObject(activeWeapon[0]);
				}*/
				else if(Time.time - timeFired > 30){
					Debug.Log ("Hard Error Correcting");
					DestroyObject(activeWeapon[0]);
				}
			}



			//If keyboard not locked out, its still a player's turn, allow controls to apply to current player's tank
		} else if (controlsactive == true && player < players.Length) {
			/******************************************
			 * 
			 * Controls enabled, let the player do stuff
			 * 
			 * ***************************************/

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
				timeFired = Time.time;
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


			/***************************************
			 * 
			 * 			Camera Controls
			 * 
			 * ***************************************/
			if (Input.GetButtonDown ("ChangeCamera")) {
				if(tankController[player].cameraPref == 1){
					swapCameraToSky(numberofAIplayers + player);
					tankController[player].cameraPref = 2;
					tankController[player].mySkyCam.transform.LookAt(tankController[player].turret.transform);
					tankController[player].mySkyCam.transform.Rotate (tankController[player].cameraAngle, 0, 0);
				} else {
					swapCamera(numberofAIplayers + player);
					tankController[player].cameraPref = 1;
				}
			}
			if (Input.GetButton ("RotateLeft") && tankController[player].cameraPref == 2){
				Debug.Log ("Move Camera");
				tankController[player].mySkyCam.transform.LookAt(tankController[player].turret.transform);
				tankController[player].mySkyCam.transform.Translate (Vector3.right * Time.deltaTime * -40);
				tankController[player].mySkyCam.transform.LookAt(tankController[player].turret.transform);
				tankController[player].mySkyCam.transform.Rotate (tankController[player].cameraAngle, 0, 0);
			}
			if (Input.GetButton ("RotateRight") && tankController[player].cameraPref == 2){
				Debug.Log ("Move Camera");
				tankController[player].mySkyCam.transform.LookAt(tankController[player].turret.transform);
				tankController[player].mySkyCam.transform.Translate (Vector3.right * Time.deltaTime * 40);
				tankController[player].mySkyCam.transform.LookAt(tankController[player].turret.transform);
				tankController[player].mySkyCam.transform.Rotate (tankController[player].cameraAngle, 0, 0);
			}
			if (Input.GetButton ("RotateUp")  && tankController[player].cameraPref == 2){
				Debug.Log (tankController[player].mySkyCam);
				Debug.Log (tankController[player].cameraAngle);
				tankController[player].cameraAngle += Time.deltaTime * 40;
				tankController[player].mySkyCam.transform.LookAt(tankController[player].turret.transform);
				tankController[player].mySkyCam.transform.Rotate (tankController[player].cameraAngle, 0, 0);
			}
			if (Input.GetButton ("RotateDown")  && tankController[player].cameraPref == 2){
				Debug.Log (tankController[player].mySkyCam);
				Debug.Log (tankController[player].cameraAngle);
				tankController[player].cameraAngle += Time.deltaTime * -40;
				tankController[player].mySkyCam.transform.LookAt(tankController[player].turret.transform);
				tankController[player].mySkyCam.transform.Rotate (tankController[player].cameraAngle, 0, 0);
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

				//Decide which way to rotate to aim at target if not already decided
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

				//Save the current distance to target
				float dist2 = Vector3.Distance (AllTankConroller [AItankController [currentAI].target].gameObject.transform.position, AItankController [currentAI].gun.gameObject.transform.position);

				//Rotate towards target
				AItankController [currentAI].turret.transform.Rotate (0, 0, rotationDir);
				//Debug.Log (dist2);

				//If the distance got longer, you were already at the closest rotation.
				if (dist2 < Vector3.Distance (AllTankConroller [AItankController [currentAI].target].gameObject.transform.position, AItankController [currentAI].gun.gameObject.transform.position)) {

					//If the AI has already fired, it should adjust its power
					if (AItankController [currentAI].shotDistance > 0) {
						//If the previous shot fell short
						if (AItankController [currentAI].shotDistance > dist2) {
							//Increase power
							AItankController [currentAI].power -= Random.Range (0F, 20F);

						//If the previous shot went long
						} else {
							//Decrease power
							AItankController [currentAI].power += Random.Range (0F, 20F);
						}
					}

					//Fire
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

	//Turn swap function, doesn't matter what the game state is, this should work.
	void nextTurn(){
		Debug.Log ("AI turn: " + AITurnOver.ToString() + " Player Turn: " + PlayerTurnOver.ToString());

		//Check to make sure the game isn't over before letting anyone take a turn
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
		//Check to see if we are past the last AI tank
		if (currentAI >= AItankList.Count) {
			Debug.Log ("End AI Turn");

			//Prepare AI index for next time
			currentAI = -1;

			//Update turn statuses

			AITurnOver = true;
			PlayerTurnOver = false;

		} else if (AItankController [currentAI].destroyed == true) {
			//Call next AI if the current one is already dead
			nextAI ();

		} else {
			//activate current AI
			swapCameraToSky(currentAI);
			AIActive = true;
		}
	}

	void nextPlayer(){
		player++;
		Debug.Log ("Player: " + player.ToString());
		//Check to see if we are past the last player
		if (player >= players.Length) {
			Debug.Log ("Last Player reached, no turn");

			//Move player so its ready for the start of the next turn
			player = -1;

			//Update turn states
			PlayerTurnOver = true;
			AITurnOver = false;

			//Back to game loop, which will call nextTurn
			return;
		} else {
			Debug.Log ("Player: " + player.ToString());
		}


		//Check to see if game is over before proceeding to the player turn
		if (gameOver ()) {

		} else if (tankController [player].destroyed == false && PlayerTurnOver == false) {
			//Checked to make sure the player about to be activated isn't dead


			//Swap camera to the prefered view on the next player
			Debug.Log ("Activate Camera on next Player");
			if(tankController[player].cameraPref == 1){
			swapCamera (numberofAIplayers + player);
			} else {
				swapCameraToSky(numberofAIplayers + player);
			}
			
			//Finally, enable controls if the player is alive
			controlsactive = true;
		} else {
			//If the player was dead, go to next player
			nextPlayer();
		}
	}

	//Swap to the sky cam of the designated tank
	void swapCamera(int nextCamera){
		//Once the next player is set, enable camera and audio for that player's camera
		if(skyCamActive == true) {
			AllTankConroller [lastcamera].mySkyCam.GetComponent<Camera> ().enabled = false;
			AllTankConroller [lastcamera].mySkyCam.GetComponent<AudioListener> ().enabled = false;
			skyCamActive = false;
		} else {
			AllTankConroller[lastcamera].mycamera.GetComponent<Camera>().enabled = false;
			AllTankConroller[lastcamera].mycamera.GetComponent<AudioListener>().enabled = false;
		}
		AllTankConroller[nextCamera].mycamera.GetComponent<Camera>().enabled = true;
		AllTankConroller[nextCamera].mycamera.GetComponent<AudioListener>().enabled = true;

		lastcamera = nextCamera;
	}

	//Swap to the close in camera of the designated tank
	void swapCameraToSky(int nextCamera){
		if (skyCamActive == true) {
			AllTankConroller [lastcamera].mySkyCam.GetComponent<Camera> ().enabled = false;
			AllTankConroller [lastcamera].mySkyCam.GetComponent<AudioListener> ().enabled = false;
			skyCamActive = false;
		} else {
			AllTankConroller[lastcamera].mycamera.GetComponent<Camera>().enabled = false;
			AllTankConroller[lastcamera].mycamera.GetComponent<AudioListener>().enabled = false;
		}
		skyCamActive = true;
		AllTankConroller [nextCamera].mySkyCam.GetComponent<Camera> ().enabled = true;
		AllTankConroller [nextCamera].mySkyCam.GetComponent<AudioListener> ().enabled = true;
		lastcamera = nextCamera;

	
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
			temptank.tag = ("AITank");
		}
	}

	//Function that checks to see if the game should be over
	bool gameOver(){
		//Count dead AI and dead players
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


		//If they add up to within 1 of the total, game is over
		if (destroyedplayers + destroyedAI >= (players.Length + AItankList.Count) - 1) {
			tankController [0].mycamera.GetComponent<Camera> ().enabled = true;
			gameover = true;
			string dispResult = "Press Enter to Start a new game\n";
			if (destroyedplayers + destroyedAI == players.Length + AItankList.Count) {
				dispResult += "The game ended in a draw";
			} else if (destroyedplayers == players.Length - 1) {
				for (int j = 0; j < players.Length; j++) {
					if (tankController [j].destroyed == false){
						tankController[j].score += 500;
						dispResult += "Player " + (j).ToString () + " has survived the round! +500 points";
					}
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
			//Return true that the game is over
			return true;
		} else {
			//Return false if 2 or more entities are still alive
			return false;
		}
	}

}
