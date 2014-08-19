using UnityEngine;

public class PlayerWalk : WalkAbs {
	
	public float animFPSBoost = 8;
	public float speedUpFactor = 1.9f;
	
	public override void reset () {
		//if (idle != null) idle.setIdle(true);
		walking = false;
		stop = false;
	}
	
	public override void walk (float velocity) {
		if (stop)
			return;
		
		if (Gamepad.isB() || (Input.GetButton("Fire1") && Input.touchCount == 0)) {
			gain = speedUpFactor;
			base._walk(velocity);
			walkAC.animComp.setFPS(animFPSBoost);
		}
		else {
			gain = 1f;
			base._walk(velocity);
			walkAC.animComp.setFPS(walkAC.animFPS);
		}
	}
}
