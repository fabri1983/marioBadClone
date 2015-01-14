using UnityEngine;
using System.Collections.Generic;
using System;

public class LevelManager : MonoBehaviour {

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
	
    private int activeLevel;
    private Player player;

	// Containers for GUI custom elements (background, foreground, buttons, text, etc)
	// They are only used for organizational purpose of GUI custom elements
    private static GameObject guiContainer_so = null; // scene only GUI container
    private static GameObject guiContainer_nd = null; // non destroyable GUI container
	private const string GUI_container_nd = "GUI_Container_nd";
	private const string GUI_container_so = "GUI_Container_so";
	
	private static PriorityComparator priorityComp = new PriorityComparator();
	
	private static LevelManager instance;
	
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
			Destroy(gameObject);
		}
		else {
			instance = this;
			DontDestroyOnLoad(gameObject);
		}

		initialize();
	}
	
	private void initialize() {
		// reset spawn positions array
		for (int i=0; i < Application.levelCount; ++i)
			spawnPosArray[i].priority = INVALID_PRIORITY;
		
		// get Mario's game object reference.
		player = Player.Instance;
		
		// NOTE: here is the place where able to load a stored saved game: get latest level and spawn position, stats, powerups, etc
	}
	
	void OnDestroy() {
		instance = null;
		player = null;
    }
	
    public int getLevel() {
        return activeLevel;
    }
	
	public int getNextLevel () {
		// if exceeds max level then return splash screen level
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
			activeLevel = 0; // splash screen
		// update current level index
		else
			activeLevel = level;
		
		player.toogleEnabled(false); // deactivate to avoid falling in empty scene, it will be restored with StartLevel script
		player.restoreWalkVel(); // in case the player was colliding a wall
		OptionQuit.Instance.reset(); // remove option buttons if on screen
		guiContainer_so = null; // reset the references of scene only GUI elements container
		
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
		// move camera instantaneously to where player spawns
		Camera.main.GetComponent<PlayerFollowerXY>().doInstantMoveOneTime();
		// activate the player's game object
		player.toogleEnabled(playerEnabled);
		// setup some scene only scripts in LookUpwards
		player.GetComponent<LookUpwards>().setup();
		// set Mario spawn position for this level
		setPlayerPosition(level);
#if UNITY_EDITOR		
		// throw error if no GUI container was created
		if (LevelManager.getGUIContainerNonDestroyable() == null)
			Debug.LogWarning("Missing " + GUI_container_nd + " game object. Will be created and populated with minimum elements.");
		if (LevelManager.getGUIContainerSceneOnly() == null)
			Debug.LogError("Missing " + GUI_container_so + " game object. Please create it.");
#endif		
		// if GUI_container_nd doesn't exist then create it and add minimum required game objects
		setupGUIContainerNonDestroyable();
		// configure the parallax properties for a correct scrolling of background and foreground images
		setParallaxProperties(spawnPosArray[activeLevel].position, levelExtent);
		// makes the camera to follow the player's Y axis until it lands. Then lock the camera's Y axis
		LockYWhenPlayerLands lockYscript = Camera.main.GetComponent<LockYWhenPlayerLands>();
		if (lockYscript)
			lockYscript.init();
		// warm no GUI dependant elements in case they don't exist yet
		TouchEventManager.warm();
		// find IFadeable component since main camera instance changes during scenes
		OptionQuit.Instance.setFaderFromMainCamera();
	}
	
	public Player getPlayer () {
		return player;
	}
	
	/// <summary>
	/// If not exist, then setups the GUI container for non destroyable game objects.
	/// </summary>
	private void setupGUIContainerNonDestroyable () {
		// if GUI_container_nd game object exists then continue normally
		if (LevelManager.getGUIContainerNonDestroyable() != null)
			return;

		// create the GUI_container_nd game object
		guiContainer_nd = new GameObject(GUI_container_nd, typeof(NonDestroyable));
#if UNITY_EDITOR
		Debug.Log(GUI_container_nd + " created.");
#endif		
		// add Gamepad game object into GUI_container_nd
		Gamepad.Instance.transform.parent = guiContainer_nd.transform;
		// add OptionQuit game object into GUI_container_nd
		OptionQuit.Instance.transform.parent = guiContainer_nd.transform;
	}
	
	private void setPlayerPosition (int level) {
		// if spawn position for current level wasn't already set then set it
		if (spawnPosArray[level].priority == INVALID_PRIORITY) {
			// get all SpawnPositionTrigger game objects from the current scene
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
	 * Defines behavior when Player loses.
	 */
	public void loseGame (bool dieAnim) {
		
		if (dieAnim) {
			// stop main camera animation
			Camera.main.GetComponent<PlayerFollowerXY>().setEnabled(false);
			// execute Mario's die animation
			player.die();
		}
		else {
			// reset mario properties
			player.resetPlayer();
			// re load current level
			loadLevel(activeLevel);
		}
	}
	
	/// <summary>
	/// Gets the game object of the game object named GUI_Container_nd which contains all the 
	/// GUI elements in the scene that aren't destroyable.
	/// </summary>
	/// <returns>
	/// A GameObject element for applying any operation on it
	/// </returns>
	public static GameObject getGUIContainerNonDestroyable () {
		if (guiContainer_nd != null)
			return guiContainer_nd;
		// cache the reference. Is the same across all scenes
	    guiContainer_nd = GameObject.Find(GUI_container_nd);
	    return guiContainer_nd;
	}
	
	/// <summary>
	/// Gets the game object of the game object named GUI_Container_so which contains all the 
	/// GUI elements in the scene that only exist during the liveness of the scene (destroyables).
	/// </summary>
	/// <returns>
	/// A GameObject element for applying any operation on it
	/// </returns>
	public static GameObject getGUIContainerSceneOnly () {
		if (guiContainer_so != null)
			return guiContainer_so;
		// cache the reference. Only has sense per scene
	    guiContainer_so = GameObject.Find(GUI_container_so);
	    return guiContainer_so;
	}
	
	/// <summary>
	/// Gets all the GUIParallax registered components and configure them according to 
	/// the extension of the level: min world position and max world position.
	/// Not all registered componenets may have a GUIParallax component.
	/// </summary>
	/// <param name='playerSpawnPos'>
	/// Player's current spawn position.
	/// </param>
	/// <param name='levelExtent'>
	/// Level dimension in world coordinates. Used to configure the parallax scripts.
	/// </param>
	private void setParallaxProperties (Vector2 playerSpawnPos, Rect levelExtent) {
		
		float length = Mathf.Abs(levelExtent.xMin - levelExtent.xMax);
		float height = Mathf.Abs(levelExtent.yMin - levelExtent.yMax);
        GUIParallax[] parallax = null;
		
		// setup the GUIParallax components from scene only GUI container
		parallax = getGUIContainerSceneOnly().GetComponentsInChildren<GUIParallax>();
		for (int i=0,c=parallax.Length; i<c;++i) {
            parallax[i].setLevelExtentWorldUnits(length, height);
            parallax[i].setOffsetWorldCoords(playerSpawnPos.x, playerSpawnPos.y);
        }
		
		// setup the GUIParallax components from non destroyable GUI container
		parallax = getGUIContainerNonDestroyable().GetComponentsInChildren<GUIParallax>();
		for (int i=0,c=parallax.Length; i<c;++i) {
            parallax[i].setLevelExtentWorldUnits(length, height);
            parallax[i].setOffsetWorldCoords(playerSpawnPos.x, playerSpawnPos.y);
        }
	}
}
