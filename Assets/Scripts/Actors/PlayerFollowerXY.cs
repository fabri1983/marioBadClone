using UnityEngine;

/// <summary>
/// This script can be asigned to a game object to follow a desired target in XY plane.
/// </summary>
public class PlayerFollowerXY : PlayerFollowerXYConfig {
	
	private Transform lookAtTarget; // object which this game object will folow and look at to
	private bool instantlyOneTime = false; // if true then the camera will not use Lerp to move to location. Valid to use one time
	private bool stop = false;
	private float constantTimer = 0.15f;
	private Transform layersStruct; // this is the container of background and foreground objects
	
	void Awake () {
		// get the game object that contains every layer game object for background and foreground
		layersStruct = LevelManager.Instance.getLayersStruct();
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
		if (layersStruct != null) {
			layersStruct.position = transform.position;
			layersStruct.rotation = transform.rotation;
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

		// update the layers struct transform
		if (layersStruct != null) {
			layersStruct.position = transform.position;
			layersStruct.rotation = transform.rotation;
		}
	}
	
	public void doInstantMoveOneTime () {
		instantlyOneTime = true;
	}
	
	public void stopAnimation () {
		stop = true;
	}
}
