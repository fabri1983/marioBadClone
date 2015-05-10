using UnityEngine;

/// <summary>
/// Attach this script to the camera game object.
/// Checks if Player position in Screen Y axis is above or below a threshold in order to 
/// force the camera to shift up/down and tries to center the Player in Y screen axis.
/// </summary>
public class PlayerFollowerYCorrection : MonoBehaviour, IGUICameraSyncable {

	public int camSyncPriority = 0; // priority when updated by GUICameraSync

	private PlayerFollowerXYConfig xyConfig;
	private Player player;
	private bool applied;
	private LockYWhenPlayerLands lookUpCorrection;

	void Awake () {
		lookUpCorrection = GetComponent<LockYWhenPlayerLands>();
		xyConfig = GetComponent<PlayerFollowerXYConfig>();
		// register against the GUICameraSync manager
		Camera.main.GetComponent<GUICameraSync>().register(this);
		// cache the player
		player = LevelManager.Instance.getPlayer();
		applied = false;
	}

	public int getPriority() {
		return camSyncPriority;
	}

	public void updateCamera () {
		// check if the player's center screen position is above or below a threshold off the screen
		Vector3 screenPos = Camera.main.WorldToScreenPoint(player.transform.position);
		bool outsideThreshold = (screenPos.y < (0.2f * Screen.height) || screenPos.y > (0.9f * Screen.height));
		if (outsideThreshold) {
			if (!applied && !player.isDying() && player.gameObject.active) {
				// player is in top half screen
				if (screenPos.y > (0.5f * Screen.height)) {
					xyConfig.lockY = false;
					lookUpCorrection.enabled = true; // enable the script that will displace the camera upwards
				}
				// player is in bottom half screen
				else {
					xyConfig.lockY = false;
					// TODO: hacer script LockYWhenPlayerOnCenter y hacerlo parecido al de LockYWhenPlayerLands
				}
				// set correction applied
				applied = true;
			}
		}
		else
			applied = false;
	}
}
