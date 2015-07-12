using UnityEngine;

/// <summary>
/// This script needs to be attached to the camera.
/// It makes the camera move on Y axis until the player is located at center of screen or a desired location.
/// </summary>
public class LockYWhenPlayerOnTarget : MonoBehaviour {

	private float origTimeFactor;
	private PlayerFollowerXYConfig origFollower;
	private Player player;
	
	void Awake () {
		origFollower = GetComponent<PlayerFollowerXYConfig>();
		this.enabled = false;
	}
	
	void OnDestroy () {
		player = null;
		origFollower = null;
	}
	
	/**
	 * LateUpdate is called after all Update functions have been called.
	 * Dependant objects might have moved during Update.
	 */
	void LateUpdate () {
		float screenPosY = Camera.main.WorldToScreenPoint(player.transform.position).y;
		float diff = Mathf.Abs(screenPosY - (Screen.height / 2));
		if (player.isDying() || (!player.isJumping() && diff < 2f)) {
			this.enabled = false;
			restoreConfig();
		}
	}
	
	private void restoreConfig () {
		// restore Main Camera's follower script configuration
		origFollower.lockY = true;
		origFollower.timeFactor = origTimeFactor;
	}
	
	public void correctPosY () {
		// copy current Main Camera configuration
		origTimeFactor = origFollower.timeFactor;
		// set new Main Camera's follower script configuration according to the effect we want to achieve
		origFollower.lockY = false; // follow the player
		origFollower.timeFactor *= 2.5f; // speed up the correction
		
		player = LevelManager.Instance.getPlayer();
		this.enabled = true;
	}
}
