using UnityEngine;

public class LookUpwards : MonoBehaviour {
	
	public float camDisplacement = 10f;
	public float speedFactor = 3f;

	private bool lookingUp;
	private AnimateTiledConfig lookUpAC;
	private PlayerFollowerXYConfig tempConfig; // for temporal storage
	private float origPosY;
	
	// Use this for initialization
	void Awake () {
		lookUpAC = AnimateTiledConfig.getByName(gameObject, EnumAnimateTiledName.LookUpwards, true);
		lookingUp = false;
		// since we cannot create an instance from a MonoBehaviour then do next
		tempConfig = gameObject.AddComponent<PlayerFollowerXYConfig>();
		// disable the dummy component from being updated by Unity3D game loop
		tempConfig.enabled = false;
	}
	
	public void lookUpwards () {
		// avoid re calculation if is already looking upwards
		if (lookingUp)
			return;
		
		// let us know if the restoring script could completelly set the camera in his original position and properties
		bool fullyCompleted = Camera.main.GetComponent<RestoreAfterLookUpwards>().wasRestored();
		if (fullyCompleted)
			origPosY = transform.localPosition.y;
		
		// get follower script
		PlayerFollowerXY playerFollower = Camera.main.GetComponent<PlayerFollowerXY>();
		// copy state
		tempConfig.setStateFrom(playerFollower);
		// apply changes
		playerFollower.lockY = false;
		playerFollower.smoothLerp = true; // the camera moves with nice lerping
		playerFollower.offsetY += camDisplacement;
		playerFollower.timeFactor = speedFactor;
		
		// set the correct sprite animation
		lookUpAC.setupAndPlay();
		lookingUp = true;
	}
	
	public void stop () {
		if (!lookingUp)
			return;
		
		// get follower script
		PlayerFollowerXY playerFollower = Camera.main.GetComponent<PlayerFollowerXY>();
		// set back state as it was previous to look upwards
		playerFollower.setStateFrom(tempConfig);
		// start the script that will let the camera moves to correct position
		if (playerFollower.lockY == true)
			Camera.main.GetComponent<RestoreAfterLookUpwards>().init(origPosY);
		
		lookingUp = false;
	}
}
