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
		// NOTE: children's Awake() doesn't exist on parent's Awake(), so you must get children components on Start()
		idleAC = AnimateTiledConfig.getByName(gameObject, EnumAnimateTiledName.Idle, true);
	}
	
	public void setIdle (bool force) {
		
		// not idle if jumping and force = false
		if (!force && jump != null && jump.isJumping())
			return;
		
		if (move != null)
			move.stop();
		
		if (crouch != null)
			crouch.noCrouch();
		
		// due to problems on Unity's initialization order there is a use case where the object isn't instantiated
		/*if (idleAC == null)
			idleAC = AnimateTiledConfig.getByName(gameObject, EnumAnimateTiledName.Idle, true);
		if (idleAC != null)*/
			idleAC.setupAndPlay();
	}
}
