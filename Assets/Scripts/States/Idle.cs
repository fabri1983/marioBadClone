using UnityEngine;

public class Idle : MonoBehaviour {
	
	private MoveAbs move;
	private Jump jump;
	private Crouch crouch;
	private AnimateTiledConfig idleAC;
	
	void Awake () {
		move = GetComponent<MoveAbs>();
		jump = GetComponent<Jump>();
		crouch = GetComponent<Crouch>();
		idleAC = GetComponentInChildren<IdleAnimConfig>();
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
		if (idleAC != null) {
			idleAC.animComp.setFPS(idleAC.animFPS);
			idleAC.animComp.setRowLimits(idleAC.rowStartAnim, idleAC.rowLengthAnim);
			idleAC.animComp.setColLimits(idleAC.maxColsAnimInRow, idleAC.colStartAnim, idleAC.colLengthAnim);
			idleAC.animComp.setPingPongAnim(idleAC.pingPongAnim);
			idleAC.animComp.Play();
		}
	}
}
