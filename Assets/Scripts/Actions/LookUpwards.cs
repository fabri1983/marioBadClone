using UnityEngine;

public class LookUpwards : MonoBehaviour {
	
	public float camDisplacement = 10f;
	public float speedFactor = 3f;
	public float restoringSpeed = 20f;

	private bool lookingUp;
	private AnimateTiledConfig lookUpAC;
	private PlayerFollowerXYConfig tempConfig; // for temporal storage
	private RestoreAfterLookUpwards restoreLookUp;
	private PlayerFollowerXY playerFollower;
	
	// Use this for initialization
	void Awake () {
		lookUpAC = AnimateTiledConfig.getByName(gameObject, EnumAnimateTiledName.LookUpwards, true);
		lookingUp = false;
		// since we cannot create an instance from a MonoBehaviour then do next
		tempConfig = gameObject.AddComponent<PlayerFollowerXYConfig>();
		// disable the dummy component from being updated by Unity3D game loop
		tempConfig.enabled = false;
	}
	
	public void setup () {
		restoreLookUp = Camera.main.GetComponent<RestoreAfterLookUpwards>();
		playerFollower = Camera.main.GetComponent<PlayerFollowerXY>();
	}
	
	public void lookUpwards () {
		// avoid re calculation if is already looking upwards
		if (lookingUp || !restoreLookUp.isRestored())
			return;
		
		// copy state
		tempConfig.setStateFrom(playerFollower);
		// apply changes
		playerFollower.lockY = false;
		playerFollower.smoothLerp = true; // the camera moves with nice lerping
		playerFollower.offsetY += camDisplacement;
		playerFollower.timeFactor = speedFactor;
		
		restoreLookUp.setRestored(false);
		
		// set the correct sprite animation
		lookUpAC.setupAndPlay();
		lookingUp = true;
	}
	
	public void restore () {
		if (!lookingUp || restoreLookUp.isRestored())
			return;
		
		// set back state as it was previous to look upwards
		playerFollower.setStateFrom(tempConfig);
		// start the script that will let the camera moves to correct position
		restoreLookUp.init(restoringSpeed);
		
		lookingUp = false;
	}
	
	public bool isLookingUpwards () {
		return lookingUp;
	}
}
