using UnityEngine;

public class PlayerFollowerXYConfig : MonoBehaviour {
	
	public bool lookTarget = false;
	public float timeFactor = 3f;
	public float offsetY = 5f;
	public bool smoothLerp = true;
	public bool lockY = false;
	
	public void setStateFrom (PlayerFollowerXYConfig other) {
		lookTarget = other.lookTarget;
		timeFactor = other.timeFactor;
		offsetY = other.offsetY;
		smoothLerp = other.smoothLerp;
		lockY = other.lockY;
	}
}
