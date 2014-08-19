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
		
		// due to problems on Unity's initialization order there is a use case where the object isn't instantiated
		if (idleAC == null)
			idleAC = GetComponentInChildren<IdleAnimConfig>();
		if (idleAC != null)
			idleAC.setupAndPlay();
	}
}
