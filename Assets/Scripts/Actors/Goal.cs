using UnityEngine;

public class Goal : MonoBehaviour {
	
	public EnumGoalActivation activation = EnumGoalActivation.ACTIVATION_UP;
	
	void Awake() {
		setTargetInside(false);
	}
	
	void Update () {
		if (isActivationValid()) {
			setTargetInside(false);
			LevelManager.Instance.loadNextLevel();
		}
	}
	
	private void setTargetInside (bool value){
		this.enabled = value;
	}
	
	public bool isActivationValid () {		
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
		
		shape1.getOwnComponent<Goal>().setTargetInside(true);
		
		// Returning false from a begin callback means to ignore the collision response for these two colliding shapes 
		// until they separate. Also for current frame. Ignore() does the same but next fixed step.
		return false;
	}
	
	public static void endCollisionWithPlayer (ChipmunkArbiter arbiter) {
		ChipmunkShape shape1, shape2;
		// The order of the arguments matches the order in the function name.
		arbiter.GetShapes(out shape1, out shape2);
		
		shape1.getOwnComponent<Goal>().setTargetInside(false);
	}
}
