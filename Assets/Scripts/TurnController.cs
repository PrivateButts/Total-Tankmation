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
	public Text txtFuel;
	public Text txtHealth;


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

	//
	public GameData _gameData;
	public float fuelUseRate = 0.1F;

   
    // Use this for initialization
	void Start() {
        _gameData = GameObject.FindGameObjectWithTag ("GameData").GetComponent<GameData> ();

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
		tankController[player].Mycamera.GetComponent<Camera>().enabled = true;
		tankController[player].Mycamera.GetComponent<AudioListener>().enabled = true;
		lastcamera = numberofAIplayers + player;
        		
		/************************************
		 * 	Generate List of Available Weapons
		 * *********************************/
		AvailWeapons weaponController = GameObject.FindGameObjectWithTag ("AvailWeapons").GetComponent<AvailWeapons>();
		weapons = new string[weaponController.Weapon.Length];
		for (int i=0; i < weaponController.Weapon.Length; i++) {
			weapons[i] = weaponController.Weapon[i].name;
		}
		txtWeapon.text = weapons[0];
		

		/*************************************
		*	Generate List of AI Tanks
		**************************************/
        Debug.Log("Start AI Tank List Generation");
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
			tankController [player].ForwardMoveAmount = 0;
			tankController [player].TurnAmount = 0;
			tankController [player].RotateAmount = 0;
			tankController [player].ElevateAmount = 0;
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
			if (Input.GetAxis ("HullMovement") > 0 && AllTankConroller[numberofAIplayers + player].Fuel > 0) {
				tankController [player].ForwardMoveAmount = tankController [player].ForwardSpeed;
				AllTankConroller[numberofAIplayers + player].Fuel -= fuelUseRate;
			}
			if (Input.GetAxis ("HullMovement") < 0 && AllTankConroller[numberofAIplayers + player].Fuel > 0) {
				tankController [player].ForwardMoveAmount = -tankController [player].ForwardSpeed;
				AllTankConroller[numberofAIplayers + player].Fuel -= fuelUseRate;
			}
			if (Input.GetAxis ("HullRotation") > 0) {
				tankController [player].TurnAmount = tankController [player].TurnSpeed * modifier;
				//AllTankConroller[numberofAIplayers + player].fuel -= fuelUseRate;
			}
			if (Input.GetAxis ("HullRotation") < 0) {
				tankController [player].TurnAmount = -tankController [player].TurnSpeed * modifier;
				//AllTankConroller[numberofAIplayers + player].fuel -= fuelUseRate;
			}
			//Gun Power Controls
			if (Input.GetAxis ("Power") > 0) {
				tankController [player].Power += tankController [player].DeltaPower * modifier;
				if (tankController [player].Power > tankController [player].MaxPower) {
					tankController [player].Power = tankController [player].MaxPower;
				}
			}
			if (Input.GetAxis ("Power") < 0) {
				tankController [player].Power -= tankController [player].DeltaPower * modifier;
				if (tankController [player].Power < 0) {
					tankController [player].Power = 0;
				}
			}
			//Turret Rotation
			if (Input.GetAxis ("TurretRotation") > 0) {
				tankController [player].RotateAmount = tankController [player].TurretRate * modifier;
			}
			if (Input.GetAxis ("TurretRotation") < 0) {
				tankController [player].RotateAmount = -tankController [player].TurretRate * modifier;
			}
			//Elevation
			if (Input.GetAxis ("Elevation") > 0) {
				if (tankController [player].CurrentEl < tankController [player].MaxElevation) {
					tankController [player].ElevateAmount = tankController [player].ElevateRate * modifier;
				}
			}
			if (Input.GetAxis ("Elevation") < 0) {
				if (tankController [player].CurrentEl > tankController [player].MinElevation) {
					tankController [player].ElevateAmount = -tankController [player].ElevateRate * modifier;
				}
				//FIRE!
			}
			if (Input.GetAxis ("Fire") > 0) {
				timeFired = Time.time;
				tankController [player].Gun.Shoot ();
				controlsactive = false;
				tankController [player].ForwardMoveAmount = 0;
				tankController [player].TurnAmount = 0;
				tankController [player].RotateAmount = 0;
				tankController [player].ElevateAmount = 0;
			}

			if (Input.GetButtonDown ("ChangeWeapon")) {
				if (tankController[player].CurrentWeapon == weapons.Length - 1) {
					tankController[player].CurrentWeapon = 0;
				} else {
					tankController[player].CurrentWeapon++;
				}
			}


			/***************************************
			 * 
			 * 			Camera Controls
			 * 
			 * ***************************************/
			if (Input.GetButtonDown ("ChangeCamera")) {
				if(tankController[player].CameraPref == 1){
					swapCameraToSky(numberofAIplayers + player);
					tankController[player].CameraPref = 2;
					tankController[player].MySkyCam.transform.LookAt(tankController[player].Turret.transform);
					tankController[player].MySkyCam.transform.Rotate (tankController[player].CameraAngle, 0, 0);
				} else {
					swapCamera(numberofAIplayers + player);
					tankController[player].CameraPref = 1;
				}
			}
			if (Input.GetButton ("RotateLeft") && tankController[player].CameraPref == 2){
				Debug.Log ("Move Camera");
				tankController[player].MySkyCam.transform.LookAt(tankController[player].Turret.transform);
				tankController[player].MySkyCam.transform.Translate (Vector3.right * Time.deltaTime * -40);
				tankController[player].MySkyCam.transform.LookAt(tankController[player].Turret.transform);
				tankController[player].MySkyCam.transform.Rotate (tankController[player].CameraAngle, 0, 0);
			}
			if (Input.GetButton ("RotateRight") && tankController[player].CameraPref == 2){
				Debug.Log ("Move Camera");
				tankController[player].MySkyCam.transform.LookAt(tankController[player].Turret.transform);
				tankController[player].MySkyCam.transform.Translate (Vector3.right * Time.deltaTime * 40);
				tankController[player].MySkyCam.transform.LookAt(tankController[player].Turret.transform);
				tankController[player].MySkyCam.transform.Rotate (tankController[player].CameraAngle, 0, 0);
			}
			if (Input.GetButton ("RotateUp")  && tankController[player].CameraPref == 2){
				Debug.Log (tankController[player].MySkyCam);
				Debug.Log (tankController[player].CameraAngle);
				tankController[player].CameraAngle += Time.deltaTime * 40;
				tankController[player].MySkyCam.transform.LookAt(tankController[player].Turret.transform);
				tankController[player].MySkyCam.transform.Rotate (tankController[player].CameraAngle, 0, 0);
			}
			if (Input.GetButton ("RotateDown")  && tankController[player].CameraPref == 2){
				Debug.Log (tankController[player].MySkyCam);
				Debug.Log (tankController[player].CameraAngle);
				tankController[player].CameraAngle += Time.deltaTime * -40;
				tankController[player].MySkyCam.transform.LookAt(tankController[player].Turret.transform);
				tankController[player].MySkyCam.transform.Rotate (tankController[player].CameraAngle, 0, 0);
			}

			//Update active tank based on keys being pressed
			if (controlsactive == true) {
				tankController [player].Elevator.transform.Rotate (tankController [player].ElevateAmount, 0, 0);
				tankController [player].CurrentEl += tankController [player].ElevateAmount;
				tankController [player].Turret.transform.Rotate (0, 0, -tankController [player].RotateAmount);
			}





		} else if (AIActive == true) {
			/*********************************************
			 * 
			 * What to do while an AI is active
			 * 
			 * ********************************************/
			if (AItankController [currentAI].Destroyed == false) {
				//Choose a target 
				//If no target, or target destroyed, pick new target
				//Debug.Log ("AI Turn Loop");
				if ((AItankController [currentAI].Target == -1) || (AllTankConroller [AItankController [currentAI].Target].Destroyed == true)) {
					Debug.Log ("Need New Target");
					AItankController [currentAI].Target = -1;
					while (AItankController[currentAI].Target == -1) {
						Debug.Log ("Old Target: " + AItankController [currentAI].Target.ToString ());
						AItankController [currentAI].Target = Random.Range (0, (AllTankConroller.Count));
						Debug.Log ("Choosing a target");
						Debug.Log ("New Target: " + AItankController [currentAI].Target.ToString ());

						if (AllTankConroller [AItankController [currentAI].Target].gameObject.name == AItankList [currentAI].name) {
							Debug.Log ("AI Tank Targeted Self");
							AItankController [currentAI].Target = -1; 
						} else if (AllTankConroller [AItankController [currentAI].Target].Destroyed == true) {
							Debug.Log ("New Target Already Destroyed");
							AItankController [currentAI].Target = -1; 
						}
					}
				}

				//Decide which way to rotate to aim at target if not already decided
				if (rotationDir == 0) {
					float dist = Vector3.Distance (AllTankConroller [AItankController [currentAI].Target].gameObject.transform.position, AItankController [currentAI].Gun.gameObject.transform.position);
					AItankController [currentAI].Turret.transform.Rotate (0, 0, 1);
					if (dist > Vector3.Distance (AllTankConroller [AItankController [currentAI].Target].gameObject.transform.position, AItankController [currentAI].Gun.gameObject.transform.position)) {
						Debug.Log ("Positive Rotate");
						rotationDir = 1;
					} else {
						Debug.Log ("Negative Rotate"); 
						rotationDir = -1;
					}
				}

				//Save the current distance to target
				float dist2 = Vector3.Distance (AllTankConroller [AItankController [currentAI].Target].gameObject.transform.position, AItankController [currentAI].Gun.gameObject.transform.position);

				//Rotate towards target
				AItankController [currentAI].Turret.transform.Rotate (0, 0, rotationDir);
				//Debug.Log (dist2);

				//If the distance got longer, you were already at the closest rotation.
				if (dist2 < Vector3.Distance (AllTankConroller [AItankController [currentAI].Target].gameObject.transform.position, AItankController [currentAI].Gun.gameObject.transform.position)) {

					//If the AI has already fired, it should adjust its power
					if (AItankController [currentAI].ShotDistance > 0) {
						//If the previous shot fell short
						if (AItankController [currentAI].ShotDistance > dist2) {
							//Increase power
							AItankController [currentAI].Power -= Random.Range (0F, 20F);

						//If the previous shot went long
						} else {
							//Decrease power
							AItankController [currentAI].Power += Random.Range (0F, 20F);
						}
					}

					//Fire
					AItankController [currentAI].Gun.Shoot ();
					AIActive = false;
					rotationDir = 0;
					timeFired = Time.time;
				}
			}
		} else if (gameover == true) {
			if (Input.GetKeyDown (KeyCode.Return)) {  
				Application.LoadLevel (1);  
			}  

		}
		//Update GUI
		if (AITurnOver) {
			//Debug.Log("Update GUI Player");
			if(player < 0){
				txtPlayer.text = "Current Player: " + _gameData.Tanks [0].Name;
			} else {
				txtPlayer.text = "Current Player: " + _gameData.Tanks [player].Name;
			}
			updateGUI(player + numberofAIplayers);
		} else if (PlayerTurnOver) {
			//Debug.Log("Update GUI AI");
			txtPlayer.text = "Current Player: AI - " + _gameData.Tanks [currentAI + numberofplayers].Name;
			updateGUI(currentAI);
		}

	}


	//Turn swap function, doesn't matter what the game state is, this should work. Figures out who the next player is and starts thier turn
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


    //Call the next AI's turn to begin
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

		} else if (AItankController [currentAI].Destroyed == true) {
			//Call next AI if the current one is already dead
			nextAI ();

		} else {
			//activate current AI
			swapCameraToSky(currentAI);
			AIActive = true;
		}
	}


    //Call the next Human Player's turn to begin
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

		} else if (tankController [player].Destroyed == false && PlayerTurnOver == false) {
			//Checked to make sure the player about to be activated isn't dead


			//Swap camera to the prefered view on the next player
			Debug.Log ("Activate Camera on next Player");
			if(tankController[player].CameraPref == 1){
			swapCamera (numberofAIplayers + player);
			} else {
				swapCameraToSky(numberofAIplayers + player);
			}
			
			//Finally, enable controls if the player is alive
			AllTankConroller[numberofAIplayers + player].Fuel = 30;
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
			AllTankConroller [lastcamera].MySkyCam.GetComponent<Camera> ().enabled = false;
			AllTankConroller [lastcamera].MySkyCam.GetComponent<AudioListener> ().enabled = false;
			skyCamActive = false;
		} else {
			AllTankConroller[lastcamera].Mycamera.GetComponent<Camera>().enabled = false;
			AllTankConroller[lastcamera].Mycamera.GetComponent<AudioListener>().enabled = false;
		}
		AllTankConroller[nextCamera].Mycamera.GetComponent<Camera>().enabled = true;
		AllTankConroller[nextCamera].Mycamera.GetComponent<AudioListener>().enabled = true;

		lastcamera = nextCamera;
	}


	//Swap to the close in camera of the designated tank
	void swapCameraToSky(int nextCamera){
		if (skyCamActive == true) {
			AllTankConroller [lastcamera].MySkyCam.GetComponent<Camera> ().enabled = false;
			AllTankConroller [lastcamera].MySkyCam.GetComponent<AudioListener> ().enabled = false;
			skyCamActive = false;
		} else {
			AllTankConroller[lastcamera].Mycamera.GetComponent<Camera>().enabled = false;
			AllTankConroller[lastcamera].Mycamera.GetComponent<AudioListener>().enabled = false;
		}

		skyCamActive = true;
		AllTankConroller [nextCamera].MySkyCam.GetComponent<Camera> ().enabled = true;
		AllTankConroller [nextCamera].MySkyCam.GetComponent<AudioListener> ().enabled = true;
		lastcamera = nextCamera;
	}


	//Player and AI spawner function
	void spawnTanks(int numplayers, int numAIs){
		//Loop creating Human Players
		for (int i=0; i<numplayers; i++){
			//Random X,Z spawn coordinates inside specified range
			int x = Random.Range (-spawnrangex, spawnrangex);
			int	z = Random.Range (-spawnrangez, spawnrangez);
			//Determine Y coordinate, adjust by -400 based on terrain position, then add 2 to avoid spawning inside terrain
			int y = Mathf.RoundToInt(terrainobj.SampleHeight(new Vector3(x, 0, z)) - 398);
			//Spawn the player, set name, set position
			GameObject temptank = Instantiate(playertanktospawn) as GameObject;
			temptank.name = _gameData.Tanks[i].Name;
			temptank.GetComponent<TankController>().Score = _gameData.Tanks[i].Score;
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
			temptank.name = "AI - " + _gameData.Tanks[numplayers + i].Name;
			temptank.GetComponent<TankController>().Score = _gameData.Tanks[numplayers + i].Score;
			temptank.transform.position = new Vector3 (x, y, z);
			temptank.tag = ("AITank");
            Debug.Log("Tank Spawned");
		}
	}


	//Function that checks to see if the game should be over
	bool gameOver(){
		//Count dead AI and dead players
		int destroyedplayers = 0;
		int destroyedAI = 0;

        //Count destroyed players
		for (int j = 0; j < players.Length; j++){
			if (tankController[j].Destroyed == true)
				destroyedplayers++;
		}

        //Count destroyed AIs
		for (int j = 0; j < AItankList.Count; j++){
			if (AItankController[j].Destroyed == true)
				destroyedAI++;
		}


		//If they add up to within 1 of the total, game is over
		if (destroyedplayers + destroyedAI >= (players.Length + AItankList.Count) - 1) {
			tankController [0].Mycamera.GetComponent<Camera> ().enabled = true;
			gameover = true;
			string dispResult = "Press Enter to Start a new game\n";

			if (destroyedplayers + destroyedAI == players.Length + AItankList.Count) { //Draw Logic
				dispResult += "The game ended in a draw";

			} else if (destroyedplayers == players.Length - 1) { //Player win logic
				for (int j = 0; j < players.Length; j++) {
					if (tankController [j].Destroyed == false){
						tankController[j].Score += 500;
						dispResult += _gameData.Tanks[j].Name + " has survived the round! +500 points";
					}
				}

			} else { //AI Win Logic
				dispResult += "Humanity has been defeated";
			}


            //Generate results screen
			dispResult = dispResult + "\n\nName\t\tScore";
			for( int i = 0; i < (players.Length); i++){
				Debug.Log (i);
				dispResult = dispResult + "\n" + players[i].name + "\t\t" + tankController[i].Score.ToString("F1");
				_gameData.Tanks[i].Score = tankController[i].Score;
			}
			for( int i = 0; i < (AItankList.Count); i++){
				Debug.Log (i);
				dispResult = dispResult + "\n" + AItankList[i].name + "\t\t" + AItankController[i].Score.ToString("F1");
				_gameData.Tanks[numberofplayers + i].Score = AItankController[i].Score;
			}
			txtResult.text = dispResult;

			//Return true that the game is over
			_gameData.Seed += 10;
			return true;
		
        } else {
			//Return false if 2 or more entities are still alive
			return false;
		}
	}


    //GUI Updater
	void updateGUI(int displayIndex){
		
        //Update GUI Elements
		if (displayIndex < 0) {
			displayIndex = 0;
		}

        //GUI Elements to update
		txtPower.text = "Power: " + AllTankConroller [displayIndex].Power.ToString ("F1");
		txtSpeed.text = "Speed: " + (Mathf.Sqrt (Mathf.Pow (AllTankConroller [displayIndex].Rb.velocity.z, 2) + Mathf.Pow (AllTankConroller [displayIndex].Rb.velocity.x, 2))).ToString ("F1");
		txtElevator.text = "Elevation: " + AllTankConroller [displayIndex].CurrentEl.ToString ("F1");
		txtScore.text = "Score: " + AllTankConroller [displayIndex].Score.ToString ("F1");
		txtWeapon.text = weapons [AllTankConroller[displayIndex].CurrentWeapon];
		txtFuel.text = "Fuel: " + AllTankConroller [displayIndex].Fuel.ToString ("F1");
		txtHealth.text = "Health: " + AllTankConroller [displayIndex].HP.ToString ("F0");
	}

}
