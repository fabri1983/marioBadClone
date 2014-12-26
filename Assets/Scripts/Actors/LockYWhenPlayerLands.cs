using UnityEngine;

/// <summary>
/// This script needs to be attached to the camera.
/// It makes the camera follow Player's Y position until it lands.
/// </summary>
public class LockYWhenPlayerLands : MonoBehaviour {
	
	private bool origSmoothLerp;
	private PlayerFollowerXYConfig origFollower;
	private Player player;
	
	void Start () {
		enabled = false;
	}
	
	void OnDestroy () {
		player = null;
	}
	
	public void init () {
		origFollower = GetComponent<PlayerFollowerXYConfig>();
		
		// copy current Main Camera configuration
		origSmoothLerp = origFollower.smoothLerp;
		
		// set new Main Camera's follower script configuration according to the effect we want to achieve
		origFollower.lockY = false; // follow the player
		origFollower.smoothLerp = false; // instant movement along the player's movement
		
		// get player instance's transform
		player = LevelManager.Instance.getPlayer();
		
		enabled = true;
	}
	
	/**
	 * LateUpdate is called after all Update functions have been called.
	 * Dependant objects might have moved during Update.
	 */
	void LateUpdate () {
		// When the level starts the player initial status is jumping.
		// So when no jumping anymore it means it hit ground and is time to restore original follower config
		if (!player.isJumping()) {
			enabled = false;
			origFollower.lockY = true;
			restoreConfig();
		}
	}
	
	public void restoreConfig () {
		// restore Main Camera's follower script configuration
		origFollower.smoothLerp = origSmoothLerp;
	}
}
