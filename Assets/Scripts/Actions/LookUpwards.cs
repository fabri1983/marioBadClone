using UnityEngine;
using System.Collections;

public class LookUpwards : MonoBehaviour {
	
	public float camDisplacement = 10f;
	public float speedFactor = 3f;

	private bool lookingUp;
	private AnimateTiledConfig lookUpAC;
	private PlayerFollowerXYConfig playerFollowerConfig;
	
	// Use this for initialization
	void Awake () {
		lookUpAC = AnimateTiledConfig.getByName(gameObject, EnumAnimateTiledName.LookUpwards, true);
		lookingUp = false;
		// since we cannot create an instance from a MonoBehaviour then do next
		playerFollowerConfig = gameObject.AddComponent<PlayerFollowerXYConfig>();
		playerFollowerConfig.enabled = false;
	}
	
	// Update is called once per frame
	public void lookUpwards () {
		if (lookingUp)
			return;
		
		// get follower script
		PlayerFollowerXY playerFollower = LevelManager.Instance.getMainCamera().GetComponent<PlayerFollowerXY>();
		// copy state
		playerFollowerConfig.setStateFrom(playerFollower);
		// apply changes
		playerFollower.lockY = false;
		playerFollower.smoothLerp = true;
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
		PlayerFollowerXY playerFollower = LevelManager.Instance.getMainCamera().GetComponent<PlayerFollowerXY>();
		// set back state to player follower component
		playerFollower.setStateFrom(playerFollowerConfig);
		
		lookingUp = false;
	}
}
