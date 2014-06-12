using UnityEngine;

public class CameraFollower : MonoBehaviour {
	
	public bool lookTarget = true;
	public float timeFactor = 4f;
	public float offsetY = 3f;
	public bool smoothLerp = true;
	
	private Transform lookAtTarget; // object which this game object will folow to and also look at to
	private bool instantlyOneTime = false; // if true then the camera will not use Lerp to move to location. Valid to use one time
	private bool stop = false;
	private float constantTimer = 0.15f;
	
	// Use this for initialization
	void Start () {

		lookAtTarget = LevelManager.Instance.getPlayer().transform;
		instantlyOneTime = false;
		
		// starting position is the Mario's XY position
		Vector3 thePos = transform.position;
		thePos.x = lookAtTarget.position.x;
		thePos.y = lookAtTarget.position.y;
		transform.position = thePos;
	}
	
	/// LateUpdate is called after all Update functions have been called.
	/// Dependant objects might have moved during Update.
	void LateUpdate () {
		// NOTE: use FixedUpdate() if camera jitters
		
		if (stop)
			return;

		// always look at target?
		if (lookTarget) {
			Quaternion origRot = transform.rotation;
			transform.LookAt(lookAtTarget);
			Quaternion gotoRot = transform.rotation;
			transform.rotation = Quaternion.Lerp(origRot, gotoRot, Time.deltaTime * timeFactor);
		}
		
		Vector3 thePos = transform.position;
		// get XY position of target and keep Z untouch
		thePos.x = lookAtTarget.position.x;
		thePos.y = lookAtTarget.position.y + offsetY;
		
		// always follow the pivot
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
		
		if (lookAtTarget.position.y < LevelManager.FREE_FALL_STOP_CAM_FOLLOW)
			stopAnimation();
	}
	
	public void doInstantMovOneTime () {
		instantlyOneTime = true;
	}
	
	public void stopAnimation () {
		stop = true;
	}
}
