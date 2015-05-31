using UnityEngine;

public class Goal : MonoBehaviour, IInCollisionCP {
	
	public EnumGoalActivation activation = EnumGoalActivation.ACTIVATION_UP;
	
	private bool inCollision;
	
	void Awake() {
		this.enabled = false;
	}
	
	public bool InCollision {
		get {return inCollision;}
		set {this.enabled = value; inCollision = value;}
	}
	
	void Update () {
		if (isActivationValid()) {
			this.enabled = false;
			LevelManager.Instance.loadNextLevel();
		}
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
}
