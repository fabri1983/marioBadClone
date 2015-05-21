using UnityEngine;

public class Goal : MonoBehaviour {
	
	public EnumGoalActivation activation = EnumGoalActivation.ACTIVATION_UP;
	
	private bool isInside;
	
	void Awake() {
		targetOutside();
	}
	
	void Update () {
		if (isActivationValid()) {
			targetOutside();
			LevelManager.Instance.loadNextLevel();
		}
	}

	private void targetOutside () {
		isInside = false;
		this.enabled = false;
	}
	
	private void targetInside() {
		isInside = true;
		this.enabled = true;
	}
	
	public bool isActivationValid () {
		if (!isInside)
			return false;
		switch (activation) {
			case EnumGoalActivation.ACTIVATION_UP:
				return Gamepad.isUp();
			case EnumGoalActivation.ACTIVATION_DOWN:
				return Gamepad.isDown();
			case EnumGoalActivation.ACTIVATION_RIGHT:
				return Gamepad.isRight();
			case EnumGoalActivation.ACTIVATION_LEFT:
				return Gamepad.isLeft();
			default: break;
		}
		return false;
	}
	
	public static bool beginCollisionWithPlayer (ChipmunkArbiter arbiter) {
		ChipmunkShape shape1, shape2;
		// The order of the arguments matches the order in the function name.
		arbiter.GetShapes(out shape1, out shape2);

		Goal g = shape1.GetComponent<Goal>();
		g.targetInside();
		
		return true;
	}
	
	public static void endCollisionWithPlayer (ChipmunkArbiter arbiter) {
		ChipmunkShape shape1, shape2;
		// The order of the arguments matches the order in the function name.
		arbiter.GetShapes(out shape1, out shape2);

		Goal g = shape1.GetComponent<Goal>();
		g.targetOutside();
	}
}
