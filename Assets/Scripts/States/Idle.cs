using UnityEngine;

public class Idle : AnimateTiledConfig {
	
	private MoveAbs move;
	private Jump jump;
	private Crouch crouch;
	
	void Awake () {
		move = GetComponent<MoveAbs>();
		jump = GetComponent<Jump>();
		crouch = GetComponent<Crouch>();
	}
	
	public void setIdle (bool force) {
		
		// not idle if jumping and force = false
		if (jump != null && jump.IsJumping() && !force)
			return;
			
		if (move != null)
			move.stopMoving();
		
		if (crouch != null)
			crouch.noCrouch();
		
		// set the correct sprite animation
		if (animComponent != null) {
			animComponent.setFPS(animFPS);
			animComponent.setRowLimits(rowStartAnim, rowLengthAnim);
			animComponent.setColLimits(maxColsAnimInRow, colStartAnim, colLengthAnim);
			animComponent.setPingPongAnim(pingPongAnim);
			animComponent.Play();
		}
	}
}
