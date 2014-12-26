using UnityEngine;

/// <summary>
/// This script can be asigned to a game object to follow a desired target in XY plane.
/// </summary>
public class PlayerFollowerXY : PlayerFollowerXYConfig {
	
	private bool instantlyOneTime = false; // if true then the camera will not use Lerp to move to location. Valid to use one time
	private float constantTimer = 0.016f;
	
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
	}
	
	void Update () {
		// NOTE: use FixedUpdate() if camera jitters

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
			transform.position = Vector3.Lerp(transform.position, thePos, constantTimer);
			// Increase or decrease the constant lerp timer
		    if (thePos == transform.position)
		        // Go to position1 t = 0.0f
		        constantTimer = Mathf.Clamp(constantTimer - Time.deltaTime, 0.0f, 1.0f);
		    else
		        // Go to position2 t = 1.0f
		        constantTimer = Mathf.Clamp(constantTimer + Time.deltaTime, 0.0f, 1.0f);
		}
		
		if (lookAtTarget.position.y < LevelManager.STOP_CAM_FOLLOW_POS_Y)
			setEnabled(false);
	}
	
	public void doInstantMoveOneTime () {
		instantlyOneTime = true;
	}
	
	public void setEnabled (bool val) {
		enabled = val;
	}
}
