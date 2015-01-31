using UnityEngine;

/// <summary>
/// Checks if Player position in Screen Y axis is above or below a threshold to then apply 
/// a camera offset forcing the camera shifts and tries to center the Player in Y screen axis.
/// </summary>
public class PlayerFollowerYCorrection : MonoBehaviour, IGUICameraSyncable {

	PlayerFollowerXYConfig xyConfig;

	void Awake () {
		xyConfig = GetComponent<PlayerFollowerXYConfig>();
		// register against the GUICameraSync manager
		Camera.main.GetComponent<GUICameraSync>().register(this);
	}

	public int getPriority() {
		return 0; // priority when updating due to GUICameraSync
	}

	public void updateCamera () {
		// TODO: check here if the player screen position is above or below 10%.
		// If so then apply an offet to xyConfig.offsetY (and maybe increase the xyConfig.timeFactor)
	}
}
