using UnityEngine;

/// <summary>
/// This script needs to be attached to the camera.
/// After a look upwards action we need to restore the camera original position and Player Follower's properties.
/// The event that trieggers that is when the Main Camera's Y position changes from movement to still.
/// </summary>
public class RestoreAfterLookUpwards : MonoBehaviour {
	
	private bool restored;
	private float prevPosY;
	private float targetPosY;
	private PlayerFollowerXYConfig origFollower;
	private PlayerFollowerXYConfig tempConfig; // for temporal storage
	
	void Awake () {
		// since we cannot create an instance from a MonoBehaviour then do next
		tempConfig = gameObject.AddComponent<PlayerFollowerXYConfig>();
		// disable the dummy component from being updated by Unity3D game loop
		tempConfig.enabled = false;
		
		restored = true;
		enabled = false;
	}
	
	void OnDestroy () {
		origFollower = null;
	}
	
	public void init (float targetYpos) {
		targetPosY = targetYpos;
		origFollower = GetComponent<PlayerFollowerXYConfig>();
		tempConfig.setStateFrom(origFollower);
		
		// apply changes accordingly to expected effect
		origFollower.lockY = false;
		origFollower.smoothLerp = true; // nice lerping
		origFollower.timeFactor *= 8f;
		
		restored = false;
		enabled = true;
	}
	
	void LateUpdate () {
		// since the lerping function used with the camera is continues then it will take a little more to 
		// reach target Y position,so we must use an epsilon
		float diff = Mathf.Abs(targetPosY - transform.localPosition.y);
			
		if (diff < 0.01f) {
			restored = true;
			enabled = false;
			origFollower.setStateFrom(tempConfig);
		}
	}
	
	public bool wasRestored () {
		return restored;
	}
}
