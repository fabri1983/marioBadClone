using UnityEngine;

public class Idle : MonoBehaviour {
	
	private WalkAbs move;
	private Jump jump;
	private Crouch crouch;
	private AnimateTiledConfig idleAC;
	
	void Awake () {
		move = GetComponent<WalkAbs>();
		jump = GetComponent<Jump>();
		crouch = GetComponent<Crouch>();
	}
	
	void Start () {
		// NOTE: it seems that Awake() from children are executed after parent's Awake()
		idleAC = GetComponentInChildren<IdleAnimConfig>();
	}
	
	public void setIdle (bool force) {
		
		// not idle if jumping and force = false
		if (!force && jump != null && jump.IsJumping())
			return;
		
		if (move != null)
			move.stopWalking();
		
		if (crouch != null)
			crouch.noCrouch();
		
		// set the correct sprite animation
		if (idleAC == null)
			idleAC = GetComponentInChildren<IdleAnimConfig>();
		if (idleAC != null) {
			idleAC.animComp.setFPS(idleAC.animFPS);
			idleAC.animComp.setRowLimits(idleAC.rowStartAnim, idleAC.rowLengthAnim);
			idleAC.animComp.setColLimits(idleAC.maxColsAnimInRow, idleAC.colStartAnim, idleAC.colLengthAnim);
			idleAC.animComp.setPingPongAnim(idleAC.pingPongAnim);
			idleAC.animComp.Play();
			idleAC.working = true;
		}
	}
}
