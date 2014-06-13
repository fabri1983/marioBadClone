using UnityEngine;

public class Teleportable : MonoBehaviour, ITeleportable {

	private bool onTeleportTrigger = false;
	private bool readyForTeleport = false;
	private bool teleporting = false; // managed from the ITeleporter object
	private bool onJump;
	private int originalLayer;
	
	/**
	 * This vector only helps to figure out what arrow key should be used to get into the teleport.
	 * It is obtained from teleporter object
	 * For instance: 
	 *   (0,-1,0) means user needs to hit down arrow key for entrance to teleport
	 *   (1,0,0) means user needs to hit right arrow key for entrance to teleport
	 */
	private Vector3 dirEntrance = Vector3.zero;
	
	void Start () {
		originalLayer = gameObject.layer;
	}
	
	void Update () {

		if (onTeleportTrigger && !teleporting) {
			
			if (   (dirEntrance.x < 0 && Input.GetAxis("Horizontal") < -0.2f) 
				|| (dirEntrance.x > 0 && Input.GetAxis("Horizontal") > 0.2f)
				|| (dirEntrance.y < 0 && Input.GetAxis("Vertical") < -0.2f)
				|| (dirEntrance.y > 0 && Input.GetAxis("Vertical") > 0.2f) ) {
				
				// keep this game object's current layer
				originalLayer = gameObject.layer;
				// setting to true will let the teleporter script to begins the teleport effect
				setReadyForTeleport(true);
			}
		}
	}
	
	public void teleportReset () {
		setReadyForTeleport(false);
		toogleTargetState(true);
		setTeleporting(false);
		setOnJump(false);
		dirEntrance = Vector3.zero;
	}
	
	public bool isReadyForTeleport () {
		return readyForTeleport;
	}
	
	public void setReadyForTeleport (bool val) {
		readyForTeleport = val;
	}
	
	public bool isTeleporting () {
		return teleporting;
	}
	
	public void setTeleporting (bool val) {
		teleporting = val;
	}
	
	public bool isOnTeleportPlace () {
		return onTeleportTrigger;
	}
	
	public void setOnTeleportPlace (bool val) {
		onTeleportTrigger = val;
	}
	
	public void setDirEntrance (Vector3 dir) {
		dirEntrance = dir;
	}
		
	public void setOnJump (bool val) {
		onJump = val;
	}
	
	public bool isOnJump () {
		return onJump;
	}
	
	public void toogleTargetState (bool val) {
		if (rigidbody != null)
			rigidbody.useGravity = val;
		// reset jump state
		if (val) {
			Jump jump = GetComponent<Jump>();
			if (jump != null)
				jump.reset();
		}
	}
	
	public int getOriginalLayer () {
		return originalLayer;
	}
}
