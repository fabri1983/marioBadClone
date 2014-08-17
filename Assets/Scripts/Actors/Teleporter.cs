using UnityEngine;

public class Teleporter : MonoBehaviour, ITeleporter {
	
	// Indicate direction for in and out transition and amount of movement for the entering tube effect
	public Vector3 transitionDirIn, transitionDirOut;
	// where the teleporting will move the target to
	public Transform endingPos;
	
	private Transform target;
	private ITeleportable teleportable;
	private IFadeable fader;
	private PlayerFollowerXY camFollower;
	
	// Use this for initialization
	void Start () {
		fader = (IFadeable)Camera.main.GetComponentInChildren(typeof(IFadeable));
		camFollower = GameObject.FindObjectOfType(typeof(PlayerFollowerXY)) as PlayerFollowerXY;
	}
	
	void Update () {
	
		if (!target) 
			return;
		
		// if fade in is occurring then wait till finishes and then do the target's transalation
		if (EnumFadeDirection.FADE_IN.Equals(fader.getFadingDirection())) {

			if (fader.isTransitionFinished()) {
				Transform targetTemp = target;
				target = null;
				// translate
				targetTemp.position = endingPos.position;
				// instantly move the camera avoiding lerp
				camFollower.doInstantMoveOneTime();
			}	
		}
		else {
			// if fade out is occurring then only need to reset its state when finishes
			if (EnumFadeDirection.FADE_OUT.Equals(fader.getFadingDirection())) {
				if (fader.isTransitionFinished()) {
					fader.stopFading();
				}
			}
			// execute animation only if:
			//  - player is ready for teleporting and has valid spawn ending position
			// or
			//  - player is teleporting
			if ( (teleportable.isReadyForTeleport() && endingPos) || teleportable.isTeleporting() ) {
				// animate
				doAnimation();
			}
		}
	}
	
	void OnTriggerEnter (Collider collider) {
		
		// allowed target enters the trigger
		if (collider.tag.Equals("Mario")) {
			
			// get the target and its ITeleportable script
			target = collider.transform;
			teleportable = (ITeleportable)target.GetComponent(typeof(ITeleportable));

			// checks if teleportable isn't valid
			if (teleportable == null) {
				target = null;
				return;
			}
			
			// set intial status
			teleportable.setOnTeleportPlace(true);
			teleportable.setDirEntrance(getDirEntrance());
			
			// if the player was teleporting then start the fade out transition
			if (teleportable.isTeleporting())
				fader.startFading(EnumFadeDirection.FADE_OUT);
		}
	}
	
	void OnTriggerExit (Collider collider) {

		// when target exits the trigger
		if (collider.tag.Equals("Mario") && target != null) {
			
			teleportable.setOnTeleportPlace(false);
			teleportable.setReadyForTeleport(false);
			teleportable.setDirEntrance(Vector3.zero);
			
			// start the fade in only if target is "going"
			if (!teleportable.isOnJump() && teleportable.isTeleporting()) {
				teleportable.setOnJump(true);
				fader.startFading(EnumFadeDirection.FADE_IN);
			}
			// exiting the teleport: deattach all references to the target
			else if (teleportable.isOnJump() && teleportable.isTeleporting()) {
				target.gameObject.layer = teleportable.getOriginalLayer();
				target = null;
				teleportable.setTeleporting(false);
				teleportable.setOnJump(false);
				// enable target's state
				teleportable.toogleTargetState(true);
			}
		}
	}
	
	private void doAnimation() {
		
		// change the layer of the player to avoid collision with the tube
		target.gameObject.layer = LevelManager.LAYER_TELEPORT;
		// we are teleporting
		teleportable.setTeleporting(true);
		teleportable.toogleTargetState(false);
			
		// translate the player certain units per second
		if (!teleportable.isOnJump())
			target.Translate(transitionDirIn * Time.deltaTime, Space.World);
		else
			target.Translate(transitionDirOut * Time.deltaTime, Space.World);
	}
	
	public Vector3 getDirEntrance () {
		return transitionDirIn;
	}
}
