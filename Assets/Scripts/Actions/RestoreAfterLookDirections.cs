using UnityEngine;

/// <summary>
/// This script needs to be attached to the camera game object.
/// After an up/downwards look action the camera position needs to be restored to original Player Follower's properties.
/// The event that triggers this behavior is when the Main Camera's Y position changes from movement to still.
/// </summary>
public class RestoreAfterLookDirections : MonoBehaviour {
	
	private PlayerFollowerXYConfig origFollower;
	private PlayerFollowerXYConfig tempConfig; // for temporal storage
	private bool restored;
	
	void Awake () {
		// since we cannot create an instance from a MonoBehaviour then do next
		tempConfig = gameObject.AddComponent<PlayerFollowerXYConfig>();
		// disable the dummy component from being updated by Unity3D game loop
		tempConfig.enabled = false;
		
		origFollower = GetComponent<PlayerFollowerXYConfig>();
	
		enabled = false;
		restored = true;
	}
	
	void OnDestroy () {
		origFollower = null;
	}
	
	public void restore (float restoreSpeed) {
		tempConfig.setStateFrom(origFollower);
		
		// apply changes accordingly to expected effect
		origFollower.lockY = false;
		origFollower.smoothLerp = true; // nice lerping
		origFollower.timeFactor = restoreSpeed; // speedup the lerping

		enabled = true;
		restored = false;
	}
	
	/**
	 * LateUpdate is called after all Update functions have been called.
	 * Dependant objects might have moved during Update.
	 */
	void LateUpdate () {
		
		// difference between current cam's Y position and the final cam's Y position with offset
		float diff = Mathf.Abs(origFollower.getFinalDisplacement() - transform.position.y);
		
		// since the lerping function used with the camera is continued then it will take a little bit more to 
		// reach the correct displacement from player. So let's use an epsilon value
		if (diff < 0.01f) {
			origFollower.setStateFrom(tempConfig);
			enabled = false;
			restored = true;
		}
	}
	
	public bool isRestored () {
		return restored;
	}
	
	public void setRestored (bool val) {
		restored = val;
	}
}
