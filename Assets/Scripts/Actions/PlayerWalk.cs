using UnityEngine;

/**
 * Moves a transform horizontally. So this script manages if transform is moving left or right.
 */
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
		
		if (Gamepad.isB() || Input.GetButton("Fire1")) {
			gain = speedUpFactor;
			base._walk(velocity);
			if (walkAC.animComp != null)
				walkAC.animComp.setFPS(animFPSBoost);
		}
		else {
			gain = 1f;
			base._walk(velocity);
			if (walkAC.animComp != null)
				walkAC.animComp.setFPS(walkAC.animFPS);
		}
	}
}
