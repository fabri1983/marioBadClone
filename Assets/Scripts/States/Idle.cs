using UnityEngine;

public class Idle : MonoBehaviour {
	
	private WalkAbs move;
	private Jump jump;
	private Crouch crouch;
	private AnimateTiledConfig idleAC;
	
	void Start () {
		// Use Awake because of some weird issue with idleAC isn't correctly referenced
		move = GetComponent<WalkAbs>();
		jump = GetComponent<Jump>();
		crouch = GetComponent<Crouch>();
		idleAC = GetComponentInChildren<IdleAnimConfig>();
	}
	
	public void setIdle (bool force) {
		
		// not idle if jumping and force = false
		if (jump != null && jump.IsJumping() && !force)
			return;
		
		if (move != null)
			move.stopWalking();
		
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
