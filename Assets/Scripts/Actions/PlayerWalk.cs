using UnityEngine;

public class PlayerWalk : WalkAbs {
	
	public float speedUpFactor = 1.9f;
	
	public override void reset () {
		//if (idle != null) idle.setIdle(true);
		walking = false;
		stop = false;
	}
	
	public override void walk (float velocity) {
		if (stop)
			return;
		
		float velX = base.shape.body.velocity.x;
		
		// when jumping and trying to move in opposite direction, just lessen current velocity
		if (jump.IsJumping() && (velX * velocity) < 0f) {
			velocity = velX * 0.75f;
			gain = 1f; // reset gain because of weird issues in velocity
		}
		else if (Gamepad.isB() || (Input.GetButton("Fire1") && Input.touchCount == 0)) {
			gain = speedUpFactor;
		}
		else {
			gain = 1f;
		}
		
		base._walk(velocity);
		walkAC.animComp.setFPS(walkAC.animFPS * gain);
	}
}
