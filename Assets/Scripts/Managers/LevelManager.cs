using UnityEngine;
using System.Collections.Generic;
using System;

public class LevelManager : MonoBehaviour {
	
	public static int LAYER_TELEPORT;
	public static int LAYER_POWERUP;
	public static int LAYER_PLAYER;
	public static int LAYER_CAMERA_IN_FRONT;
	public const float ENDING_DIE_ANIM_Y_POS = -20f; // used in addition to current y pos
	public const float STOP_CAM_FOLLOW_POS_Y = -2f; // y world position for stopping camera follower
	public const int INVALID_PRIORITY = -1;
	
	// aspect ratio variables
	public static bool keepAspectRatio = false;
	public const float ASPECT_W = 16f;
	public const float ASPECT_H = 10.5f;
	
	/// spawn positions for player. They are set automatically when a level is loaded and 
	/// also when player fires a spawn position trigger
	private static SpawnPositionSpot[] spawnPosArray = new SpawnPositionSpot[Application.levelCount];
	
    private static LevelManager instance;
    private int activeLevel;
    private Player player;
	
	// used for Mario's die animation
	private GameObject mainCam, camInFront;
	
	private static PriorityComparator priorityComp = new PriorityComparator();
	
	/**
	 * Singleton access
	 */
	public static LevelManager Instance {
        get {
            if (instance == null) {
				// creates a game object with this script component
				instance = new GameObject("LevelManager").AddComponent<LevelManager>();
			}
            return instance;
        }
    }
	
	void Awake () {
		if (instance != null && instance != this)
			Destroy(this.gameObject);
		else {
			instance = this;
			DontDestroyOnLoad(gameObject);
		}
		initialize();
	}
	
	private void initialize() {
		
		LAYER_TELEPORT = LayerMask.NameToLayer("TeleportTrig");
		LAYER_CAMERA_IN_FRONT = LayerMask.NameToLayer("CameraInFront");
		LAYER_POWERUP = LayerMask.NameToLayer("PowerUp");
		LAYER_PLAYER = LayerMask.NameToLayer("Player");

		// reset spawn positions array
		for (int i=0; i < Application.levelCount; ++i)
			spawnPosArray[i].priority = INVALID_PRIORITY;
		
		// get Mario's game object reference.
		player = Player.Instance;
		
		// NOTE: here is the place where able to load a stored saved game: get latest level and spawn position, stats, powerups, etc
	}
	
	void OnApplicationQuit() {
#if UNITY_EDITOR
#else
		// NOTE: to avoid !IsPlayingOrAllowExecuteInEditMode error in console:
		instance = null;
#endif
    }
	
    public int getLevel() {
        return activeLevel;
    }
	
	public int getNextLevel () {
		// if exceeds max level then return first valid level
		return activeLevel + 1 >= Application.levelCount ? 0 : activeLevel + 1;
	}
	
	public void loadNextLevel() {
		resetSpawnPos(activeLevel);
		loadLevel(getNextLevel());
	}
	
	public void loadLevelSelection () {
		loadLevel(1);
	}
	
	public void loadLevel (int level) {
		// fix level index if invalid
		if (level < 0 || level >= Application.levelCount)
			activeLevel = 0;
		// update current level index
		else
			activeLevel = level;
		
		player.toogleActivate(false); // deactivate to avoid falling in empty scene
		player.restoreWalkVel(); // in case the player was colliding a wall
		OptionQuit.Instance.reset(); // remove option buttons if on screen
		GC.Collect();
		Application.LoadLevel(activeLevel); // load scene
	}
	
	/**
	 * This invoked from StartLevel object which has the number of the level.
	 * It enables the player's game object if playerEnabled is true, load the spawn positions, set the player's position.
	 */
	public void startLevel (int level, bool playerEnabled) {
		activeLevel = level;
		camInFront.SetActiveRecursively(false); // disable in front camera
		mainCam.GetComponent<CameraFollower>().doInstantMovOneTime(); // move camera instantaneously to where player spawns
		player.toogleActivate(playerEnabled); // activate the player's game object
		setPlayerPosition(level); // set Mario spawn position for this level
		
		// warm other needed elements in case they don't exist yet
		Gamepad.warm();
		TouchEventManager.warm();
		OptionQuit.warm();
	}
	
	public Player getPlayer () {
		return player;
	}
	
	private void setPlayerPosition (int level) {
		// if spawn position for current level wasn't already set then set it
		if (spawnPosArray[level].priority == INVALID_PRIORITY) {
			// get all SpawnPositionTrigger game objects
			SpawnPositionTrigger[] arr = (SpawnPositionTrigger[])FindObjectsOfType(typeof(SpawnPositionTrigger));
			// no triiger game objects? then use default spawn position
			if (arr == null || arr.Length == 0) {
				spawnPosArray[level].priority = 0; // to avoid seeking again for all triggers
				spawnPosArray[level].position = Vector2.zero;
			}
			else {
				// sort the array by priority value from highest to lowest
				Array.Sort(arr, priorityComp);
				spawnPosArray[level] = arr[0].getSpawnPos();
			}
		}
		// set player's spawn position
		player.GetComponent<ChipmunkBody>().position = spawnPosArray[level].position;
	}
	
	/**
	 * For the current level updates the last visited spawn position. 
	 * This is invoked from a trigger.
	 */
	public void updateLastSpawnPosition (SpawnPositionSpot sp) {
		spawnPosArray[activeLevel] = sp;
	}
	
	/**
	 * Set the spawn position for the pointed level as invalid, so next time the level is loaded it start from beginning.
	 */
	private void resetSpawnPos (int level) {
		spawnPosArray[level].priority = INVALID_PRIORITY;
	}
	
	/**
	 * Defines behavior when Mario loses game
	 */
	public void loseGame (bool dieAnim) {
		
		if (dieAnim) {
			// stop main camera animation
			mainCam.GetComponent<CameraFollower>().stopAnimation();
			// set camera's position the same position than main camera
			camInFront.transform.position = mainCam.transform.position;
			camInFront.transform.forward = mainCam.transform.forward;
			// enable the camera so it renders mario game object in front of all
			camInFront.SetActiveRecursively(true);
			// execute Mario's die animation
			player.die();
			
			// not need to enable main camera animation neither disabled in front camera because the level is reloaded
		}
		else {
			// reset mario properties
			player.resetPlayer();
			// re load current level
			loadLevel(activeLevel);
		}
	}
	
	public GameObject getMainCamera () {
		return mainCam;
	}
	
	public GameObject getLCameraInFront () {
		return camInFront;
	}
	
	public void setMainCamera (GameObject cam) {
		mainCam = cam;
	}
	
	public void setCamerainFront (GameObject cam) {
		camInFront = cam;
	}
}
