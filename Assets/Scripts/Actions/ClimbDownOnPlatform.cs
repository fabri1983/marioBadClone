using UnityEngine;
using System.Collections;

/// <summary>
/// This class allow to pull down from a platform when on crouching.
/// </summary>
public class ClimbDownOnPlatform : MonoBehaviour {

	private bool isOnPlatform = false;
	private bool traversingUpwards = false;
	private Crouch crouch;

	void Awake () {
		crouch = GetComponent<Crouch>();
	}

	public bool isTraversingUpwards () {
		return traversingUpwards;
	}

	public void setTraversingUpwards (bool val) {
		traversingUpwards = val;
	}

	public void handleLanding () {
		traversingUpwards = false;
		isOnPlatform = true;
	}

	/// <summary>
	/// The separation may be from above, below, even from sides.
	/// So the corretc state is handled according other states set on other collision phases.
	/// </summary>
	public void handleSeparation () {
		if (traversingUpwards) {
			isOnPlatform = true;
			traversingUpwards = false;
		}
		else if (isOnPlatform)
			traversingUpwards = true;
		else
			isOnPlatform = false;
	}

	public bool isPullingDown () {
		if (isOnPlatform && crouch != null && crouch.isCrouching())
			return true;
		return false;
	}
}
