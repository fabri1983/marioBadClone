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
		if (instance != null && instance != this) {
			Destroy(this.gameObject);
		}
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
	
	void OnDestroy() {
		instance = null;
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
	
	/// <summary>
	/// This invoked from StartLevel object which has the number of the level.
	/// It enables the player's game object if playerEnabled is true, load the spawn positions, set the player's position.
	/// </summary>
	/// <param name='level'>
	/// Number of scene according Unity's indexing
	/// </param>
	/// <param name='playerEnabled'>
	/// True if player starts being enabled. False if not
	/// </param>
	/// <param name='levelExtent'>
	/// Level dimension in world coordinates
	/// </param>
	public void startLevel (int level, bool playerEnabled, Rect levelExtent) {
		activeLevel = level;
		CameraManager.Instance.getInFrontCam().gameObject.SetActiveRecursively(false); // disable in front camera
		Camera.main.GetComponent<PlayerFollowerXY>().doInstantMoveOneTime(); // move camera instantaneously to where player spawns
		player.toogleActivate(playerEnabled); // activate the player's game object
		setPlayerPosition(level); // set Mario spawn position for this level
		setParallaxProperties(levelExtent); // configure the parallax properties for a correct scrolling
		
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
			Camera camInFront = CameraManager.Instance.getInFrontCam();
			// stop main camera animation
			Camera.main.GetComponent<PlayerFollowerXY>().stopAnimation();
			// set camera's position the same position than main camera
			camInFront.transform.position = Camera.main.transform.position;
			camInFront.transform.forward = Camera.main.transform.forward;
			// enable the camera so it renders mario game object in front of all
			camInFront.gameObject.SetActiveRecursively(true);
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
	
	/// <summary>
	/// Gets the game object that contains all the GUI elements in the scene.
	/// Important: this only works if there is all Parallax scripts are parented by one game object.
	/// </summary>
	/// <returns>
	/// The Transform component.
	/// </returns>
	public Transform getGUILayers () {
		GameObject guiLayers = GameObject.Find("GUI_Layers");
		
		if (guiLayers == null) {
			Debug.LogWarning("Couldn't find game object named GUI_Layers");
			return null;
		}
		
		return guiLayers.transform;
	}
	
	/// <summary>
	/// Gets all the Parallax components from the main camera and configure them according to 
	/// the extension of the level: min world position and max world position.
	/// </summary>
	/// <param name='levelExtent'>
	/// Level dimension in world coordinates. Used to configure the parallax scripts
	/// </param>
	private void setParallaxProperties (Rect levelExtent) {
		Transform t = getGUILayers();
		if (t == null)
			return;
		
		Parallax[] parallax = t.GetComponentsInChildren<Parallax>();
		
		float length = Mathf.Abs(levelExtent.xMin - levelExtent.xMax);
		float height = Mathf.Abs(levelExtent.yMin - levelExtent.yMax);
		Vector2 playerSpawnPos = spawnPosArray[activeLevel].position;
		
		for (int i=0,c=parallax.Length; i<c;++i) {
			parallax[i].setLevelExtentWorldUnits(length, height);
			parallax[i].setOffsetWorldCoords(playerSpawnPos.x, playerSpawnPos.y);
		}
	}
}
