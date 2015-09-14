using UnityEngine;

public class Goal : MonoBehaviour {
	
	public EnumGoalActivation activation = EnumGoalActivation.ACTIVATION_UP;
	
	private int triggerA, triggerB;
	private BeforeLoadNextScene beforeNextScene;
	
	void Awake() {
		flagTargetOutside(triggerA);
		flagTargetOutside(triggerB);
		
		// setup the effects chain triggered before load next scene
		beforeNextScene = GetComponent<BeforeLoadNextScene>();
	}
	
	void Update () {
		if (isActivationValid()) {
			if (beforeNextScene != null) {
				this.enabled = false; // avoid re execution of the before next scene effect
				beforeNextScene.setScene(LevelManager.Instance.getNextLevelName());
				beforeNextScene.execute();
			}
			else
				LevelManager.Instance.loadNextLevel();
		}
	}
	
	private void flagTargetOutside (int target){
		if (triggerA == target)
			triggerA = -1;
		else if (triggerB == target)
			triggerB = -1;
		
		if (triggerA == -1 && triggerB == -1)
			this.enabled = false;
	}
	
	private void flagTargetInside (int flag) {
		if (triggerA == -1)
			triggerA = flag;
		else if (triggerB == -1)
			triggerB = flag;
		
		this.enabled = true;
	}
	
	public bool isActivationValid () {
		if (triggerA != -1 && triggerB != -1) {
			switch (activation) {
				case EnumGoalActivation.ACTIVATION_UP:
					return Gamepad.Instance.isUp();
				case EnumGoalActivation.ACTIVATION_DOWN:
					return Gamepad.Instance.isDown();
				case EnumGoalActivation.ACTIVATION_RIGHT:
					return Gamepad.Instance.isRight();
				case EnumGoalActivation.ACTIVATION_LEFT:
					return Gamepad.Instance.isLeft();
				default: break;
			}
		}
		return false;
	}
	
	public static bool beginCollisionWithPlayer (ChipmunkArbiter arbiter) {
		ChipmunkShape shape1, shape2;
		// The order of the arguments matches the order in the function name.
		arbiter.GetShapes(out shape1, out shape2);
		
		shape1.getOwnComponent<Goal>().flagTargetInside(shape1._handle.ToInt32());
		
		// Returning false from a begin callback means to ignore the collision response for these two colliding shapes 
		// until they separate. Also for current frame. Ignore() does the same but next fixed step.
		return false;
	}
	
	public static void endCollisionWithPlayer (ChipmunkArbiter arbiter) {
		ChipmunkShape shape1, shape2;
		// The order of the arguments matches the order in the function name.
		arbiter.GetShapes(out shape1, out shape2);
		
		shape1.getOwnComponent<Goal>().flagTargetOutside(shape1._handle.ToInt32());
	}
}
