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
	private LockYWhenPlayerOnTarget camPosYCorrection;
	
	void Awake () {
		camPosYCorrection = GetComponent<LockYWhenPlayerOnTarget>();
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
		Vector3 screenPos = Camera.main.WorldToScreenPoint(player.transform.position);
		// check if the player's center screen position is above or below a threshold off the screen
		bool outsideArea = (screenPos.y < (0.2f * Screen.height) || screenPos.y > (0.9f * Screen.height));
		if (outsideArea) {
			if (!applied && !player.isDying() && player.gameObject.active) {
				xyConfig.lockY = false;
				camPosYCorrection.correctPosY();
				applied = true;
			}
		}
		else
			applied = false;
	}
}
