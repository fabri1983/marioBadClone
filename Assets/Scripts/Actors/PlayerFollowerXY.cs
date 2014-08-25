using UnityEngine;

/// <summary>
/// This script can be asigned to a game object to follow a desired target in XY plane.
/// </summary>
public class PlayerFollowerXY : PlayerFollowerXYConfig, IUpdateGUILayers {
	
	private Transform lookAtTarget; // object which this game object will folow and look at to
	private bool instantlyOneTime = false; // if true then the camera will not use Lerp to move to location. Valid to use one time
	private bool stop = false;
	private float constantTimer = 0.15f;
	private Transform guiLayers; // this is the container of gui objects (background, foreground, etc)
	
	void Awake () {
		// get the game object that contains the gui elements
		// NOTE: this works fine here only if this script is created per scene.
		guiLayers = LevelManager.Instance.getGUILayers();
	}
	
	// Use this for initialization
	void Start () {
		lookAtTarget = LevelManager.Instance.getPlayer().transform;
		instantlyOneTime = false;
		
		// starting position is the Mario's XY position
		Vector3 thePos = transform.position;
		thePos.x = lookAtTarget.position.x;
		thePos.y = lookAtTarget.position.y;
		if (lockY)
			 thePos.y += offsetY;
		transform.position = thePos;

		// update the layers struct transform
		if (guiLayers != null) {
			guiLayers.position = transform.position;
			guiLayers.rotation = transform.rotation;
		}
	}
	
	/**
	 * LateUpdate is called after all Update functions have been called.
	 * Dependant objects might have moved during Update.
	 */
	void LateUpdate () {
		// NOTE: use FixedUpdate() if camera jitters

		if (stop)
			return;

		// always look at target?
		if (lookTarget) {
			Quaternion origRot = transform.rotation; // save original rotation since lookAtTarget affects it
			transform.LookAt(lookAtTarget);
			transform.rotation = Quaternion.Lerp(origRot, transform.rotation, Time.deltaTime * timeFactor);
		}
		
		Vector3 thePos = transform.position;
		// get XY position of target and keep Z untouch
		thePos.x = lookAtTarget.position.x;
		if (!lockY || instantlyOneTime)
			thePos.y = lookAtTarget.position.y + offsetY;
		
		if (instantlyOneTime) {
			transform.position = thePos;
			instantlyOneTime = false;
		}
		else if (smoothLerp){
			transform.position = Vector3.Lerp(transform.position, thePos, Time.deltaTime * timeFactor);
		}
		else {
			transform.position = Vector3.Lerp(transform.position, thePos, constantTimer * timeFactor);
			// Increase or decrease the constant lerp timer
		    if (thePos == transform.position)
		        // Go to position1 t = 0.0f
		        constantTimer = Mathf.Clamp(constantTimer - Time.deltaTime, 0.0f, 1.0f);
		    else
		        // Go to position2 t = 1.0f
		        constantTimer = Mathf.Clamp(constantTimer + Time.deltaTime, 0.0f, 1.0f);
		}
		
		if (lookAtTarget.position.y < LevelManager.STOP_CAM_FOLLOW_POS_Y)
			stopAnimation();

		// update the game object that contains all the Parallax scripts that depends on this game object's position
		updateGUILayers();
	}
	
	public void doInstantMoveOneTime () {
		instantlyOneTime = true;
	}
	
	public void stopAnimation () {
		stop = true;
	}
	
	public void updateGUILayers () {
		if (guiLayers != null) {
			guiLayers.position = transform.position;
			guiLayers.rotation = transform.rotation;
		}
	}
}
