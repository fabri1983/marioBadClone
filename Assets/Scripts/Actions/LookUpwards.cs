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
	private PlayerWalk walk;
	
	// Use this for initialization
	void Awake () {
		lookUpAC = AnimateTiledConfig.getByName(gameObject, EnumAnimateTiledName.LookUpwards, true);
		lookingUp = false;
		// since we cannot create an instance from a MonoBehaviour then do next
		tempConfig = gameObject.AddComponent<PlayerFollowerXYConfig>();
		// disable the dummy component from being updated by Unity3D game loop
		tempConfig.enabled = false;
		
		walk = GetComponent<PlayerWalk>();
	}
	
	/// <summary>
	/// Setup needed per scene, when it is loaded.
	/// </summary>
	public void setup () {
		restoreLookUp = Camera.main.GetComponent<RestoreAfterLookUpwards>();
		playerFollower = Camera.main.GetComponent<PlayerFollowerXY>();
		lookingUp = false;
	}
	
	public void lookUpwards () {
		// since is allowed to look upwards and walk at the same time then make sure the correct 
		// sprite anim is executed when is not walking
		if (lookingUp && !walk.isWalking())
			lookUpAC.setupAndPlay();
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
	
	public void lockYWhenJumping () {
		// when jumping and still looking up, then lock Y axis to avoid a continuing displacement of camera
		if (lookingUp)
			playerFollower.lockY = true;
	}
	
	public bool isLookingUpwards () {
		return lookingUp;
	}
}
