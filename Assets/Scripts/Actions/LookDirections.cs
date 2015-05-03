using UnityEngine;

public class LookDirections : MonoBehaviour {

	private const float _PERIOD = 5f; // factor used in timing for waiting before look direction action takes effect

	public float camDisplacement = 10f;
	public float speedFactor = 3f;
	public float restoringSpeed = 20f;
	
	private float dirSign;
	private AnimateTiledConfig lookUpAC;
	private PlayerFollowerXYConfig tempConfig; // for temporal storage
	private RestoreAfterLookDirections restoreLookDir;
	private PlayerFollowerXY playerFollower;
	private PlayerWalk walk;
	private float timing;

	// Use this for initialization
	void Awake () {
		lookUpAC = AnimateTiledConfig.getByName(gameObject, EnumAnimateTiledName.LookUpwards, true);
		dirSign = 0f;
		walk = GetComponent<PlayerWalk>();
		// since we cannot create an instance from a MonoBehaviour then do next
		tempConfig = gameObject.AddComponent<PlayerFollowerXYConfig>();
		// disable the dummy component from being updated by Unity3D game loop
		tempConfig.enabled = false;
	}
	
	/// <summary>
	/// Setup needed per scene, when it is loaded.
	/// </summary>
	public void setup () {
		restoreLookDir = Camera.main.GetComponent<RestoreAfterLookDirections>();
		playerFollower = Camera.main.GetComponent<PlayerFollowerXY>();
		dirSign = 0f;
		timing = 0f;
	}

	public void lookUpwards () {
		// since is allowed to look upwards and walk at the same time then make sure the correct 
		// sprite anim is executed when is not walking
		if (dirSign > 0f && !walk.isWalking())
			lookUpAC.setupAndPlay();

		// if it's not correct timing then exit method
		if (timing < 1f) {
			timing += Time.deltaTime * _PERIOD;
			return;
		}
		// avoid re calculation if is already looking upwards
		if (dirSign > 0f || !restoreLookDir.isRestored())
			return;

		dirSign = 1f;
		lookToDir();
	}

	public void lookDownwards () {
		// avoid re calculation if is already looking downwards or if it's not correct timing
		if (dirSign < 0f || !restoreLookDir.isRestored())
			return;

		dirSign = -1f;
		lookToDir();
	}

	private void lookToDir () {
		// copy state
		tempConfig.setStateFrom(playerFollower);
		// apply changes
		playerFollower.lockY = false;
		playerFollower.smoothLerp = true; // moves the camera with nice lerping
		playerFollower.offsetY += dirSign * camDisplacement;
		playerFollower.timeFactor = speedFactor;
		// set dirty state to the restoring logic
		restoreLookDir.setRestored(false);
	}
	
	public void restore () {
		timing = 0f;

		if (dirSign == 0f || restoreLookDir.isRestored())
			return;
		
		// set back state as it was previous to look upwards
		playerFollower.setStateFrom(tempConfig);
		// enables the script that will restore the Player Follower's property
		restoreLookDir.restore(restoringSpeed);
		
		dirSign = 0f;
	}
	
	public void lockYWhenJumping () {
		// when jumping and still looking up, then lock Y axis to avoid a continuing displacement of camera
		if (dirSign > 0f)
			playerFollower.lockY = true;
	}

	public bool isLookingAnyDirection () {
		return dirSign != 0f;
	}

	public bool isLookingUpwards () {
		return dirSign > 0f;
	}

	public bool isLookingDownwards () {
		return dirSign < 0f;
	}
}
