using UnityEngine;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour {
	
	public static int TELEPORT_LAYER;
	public static int FOR_ENEMY_LAYER;
	public static int CAMERA_IN_FRONT;
	public static int POWERUP_LAYER;
	public static int ONLY_WITH_PLAYER_LAYER;
	public const float ENDING_DIE_ANIM_Y_POS = -20f; // used in addition to current y pos
	public const float FREE_FALL_STOP_CAM_FOLLOW = -2f; // y world position for stopping camera follower
	
	// aspect ratio variables
	public static bool keepAspectRatio = true;
	public const float ASPECT_W = 16f;
	public const float ASPECT_H = 10.5f;
	
	// spawn positions for mario. They are set automatically when a level is loaded
	private static List<SpawnPositionSpot>[] spawnPosList = new List<SpawnPositionSpot>[Application.levelCount];
	// track last spawn position
	private static SpawnPositionSpot[] lastSpawnPos = new SpawnPositionSpot[Application.levelCount];
	
    private static LevelManager instance;
    private int activeLevel;
    private Player player;
	
	// used for Mario's die animation
	private GameObject mainCam, camInFront;
	
	/**
	 * Used for sorting the spawn positions
	 */
	public class PriorityComparator : System.Collections.Generic.IComparer<SpawnPositionSpot> {
		int System.Collections.Generic.IComparer<SpawnPositionSpot>.Compare(SpawnPositionSpot a, SpawnPositionSpot b) {
			SpawnPositionSpot va = (SpawnPositionSpot)a;
			SpawnPositionSpot vb = (SpawnPositionSpot)b;
			if (va.getPriority() < vb.getPriority())
				return -1;
			else if (va.getPriority() > vb.getPriority())
				return 1;
			return 0;
		}
	}
	
	/**
	 * Singleton access
	 */
	public static LevelManager Instance {
        get {
            if (instance == null) {
				// creates a game object with this script component.
				instance = new GameObject("LevelManager").AddComponent<LevelManager>();
				DontDestroyOnLoad(instance);
				instance.initialize();
			}
            return instance;
        }
    }
	
    /**
     * Sets the instance to null when the application quits
     */
    void OnApplicationQuit() {
		spawnPosList = null;
		instance = null;
		player = null;
    }
	
	private void initialize() {
		
		TELEPORT_LAYER = LayerMask.NameToLayer("TeleportTrig");
		FOR_ENEMY_LAYER = LayerMask.NameToLayer("ForEnemy");
		CAMERA_IN_FRONT = LayerMask.NameToLayer("CameraInFront");
		POWERUP_LAYER = LayerMask.NameToLayer("PowerUp");
		ONLY_WITH_PLAYER_LAYER = LayerMask.NameToLayer("OnlyWithPlayer");
		
		// reset indexes spawn position matrix
		for (int i=0; i < Application.levelCount; ++i)
			lastSpawnPos[i] = null;
		
		// get Mario's game object reference.
		player = Player.Instance;
		
		// NOTE: here is the place where able to load a stored saved game: get latest level and spawn position, stats, powerups, etc
	}
	
    public int getLevel() {
        return activeLevel;
    }
	
	public int getNextLevel () {
		// if exceeds max level then return first valid level
		return activeLevel + 1 >= Application.levelCount ? 0 : activeLevel + 1;
	}
	
	public void loadNextLevel() {
		resetLastSpawnPos(activeLevel);
		loadLevel(getNextLevel());
	}
	
	public void loadLevel (int level) {
		// fix level index if invalid
		if (level < 0 || level >= Application.levelCount)
			activeLevel = 0;
		// update current level index
		else
			activeLevel = level;
		
		// deactivate to avoid falling in empty scene
		player.gameObject.SetActiveRecursively(false);
		
		// load level
		Application.LoadLevel(activeLevel);
	}
	
	/**
	 * This invoked from StartLevel object which has the number of the level.
	 * It enables the player's game object if playerEnabled is true, load the spawn positions, set the player's position.
	 */
	public void startLevel (int level, bool playerEnabled) {
		activeLevel = level;
		// disable in front camera
		camInFront.camera.enabled = false;
		// activate the player's game object
		player.gameObject.SetActiveRecursively(playerEnabled);
		// load spawn positions for current level. This is invoked everytime a level is loaded but 
		// the method considers if spawn positions were already loaded.
		loadSpawnPositions(level);
		// set Mario spawn position for this level
		setPlayerPosition(level);
		// warm other needed elements in case they don't exist yet
		Gamepad.warm();
		TouchEventManager.warm();
		OptionClickQuit.warm();
	}
	
	public Player getPlayer () {
		return player;
	}
	
	private void setPlayerPosition (int level) {
		// set mario's spawn position
		if (spawnPosList[level].Count > 0) {
			if (lastSpawnPos[level] == null)
				resetLastSpawnPos(level);
			ChipmunkBody body = player.GetComponent<ChipmunkBody>();
			body.position = lastSpawnPos[level].getPosition();
		}
	}
	
	private void resetLastSpawnPos (int level) {
		if (spawnPosList[level].Count > 0)
			lastSpawnPos[level] = spawnPosList[level][0];
	}
	
	/**
	 * Every time the scene is reloaded checks if spawn positions for this level were already set.
	 * If not, then grab every SpawnPosition component and add it to the spawn positions list.
	 */
	private void loadSpawnPositions (int level) {

		if (spawnPosList[level] == null)
			spawnPosList[level] = new List<SpawnPositionSpot>(5);
		if (spawnPosList[level].Count == 0)
			populateSpawnPositions(level);
	}
	
	/**
	 * Get all SpawnPosition components in the current scene and add them to spawn positions list
	 */ 
	private void populateSpawnPositions (int level) {
		
		SpawnPositionTrigger[] arr = (SpawnPositionTrigger[])FindObjectsOfType(typeof(SpawnPositionTrigger));
		if (arr == null)
			return;
		for (int i=0; i < arr.Length; ++i) {
			List<SpawnPositionSpot> list = spawnPosList[level];
			list.Add(arr[i].getSpawnPos());
		}
		// sort the list in X axis in ascendent order by priority value
		List<SpawnPositionSpot> sortedList = spawnPosList[level];
		sortedList.Sort(new PriorityComparator());
	}
	
	/**
	 * For the current level updates the last visited spawn position. 
	 * This is invoked from a trigger.
	 */
	public void updateLastSpawnPosition (SpawnPositionSpot sp) {
		lastSpawnPos[activeLevel] = sp;
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
			// enable the camera so it renders mario game object in front of all
			camInFront.camera.enabled = true;
			camInFront.transform.forward = mainCam.transform.forward;
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
