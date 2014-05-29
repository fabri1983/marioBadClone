using UnityEngine;

/**
 * Moves a transform horizontally. So this script manages if transform is moving left or right.
 */
public class Move : MoveAbs {
	
	public static Vector2 MOVEMENT_AXIS = Vector2.right;
	
	private bool stop;
	private bool animSet;
	private Vector2 dir; // only for accessing sign direction
	private Jump jump;
	private AnimateTiledConfig moveAC;
	private ChipmunkShape shape;
	
	void Awake () {
		stop = false;
		animSet = false;
		jump = GetComponent<Jump>();
		shape = GetComponent<ChipmunkShape>();
		moveAC = GetComponentInChildren<MoveAnimConfig>();
	}
	
	public override void move (Vector2 velocity) {
		if (stop)
			return;
		
		// set the correct sprite animation
		if (!animSet && (jump == null || !jump.IsJumping())) {
			moveAC.animComp.setFPS(moveAC.animFPS);
			moveAC.animComp.setRowLimits(moveAC.rowStartAnim, moveAC.rowLengthAnim);
			moveAC.animComp.setColLimits(moveAC.maxColsAnimInRow, moveAC.colStartAnim, moveAC.colLengthAnim);
			moveAC.animComp.setPingPongAnim(moveAC.pingPongAnim);
			moveAC.animComp.Play();
			animSet = true;
		}
		
		dir = velocity;
		
		Vector2 v = shape.body.velocity;
		// apply available axis mask
		v.x = velocity.x * MOVEMENT_AXIS.x;
		v.y = velocity.y * MOVEMENT_AXIS.y;
		shape.body.velocity = v;
	}
	
	public override void move(float velocity) {
		// apply available axis mask
		move(MOVEMENT_AXIS * velocity);
	}
	
	public override void stopMoving () {
		stop = true;
		animSet = false;
	}
	
	public void enableMoving () {
		stop = false;
	}
	
	public override bool isMoving () {
		return !stop;
	}
	
	/**
	 * Typically used ot track direction's changes
	 */
	public Vector2 getDir () {
		return dir;
	}
}
