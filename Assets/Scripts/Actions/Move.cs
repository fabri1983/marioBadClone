using UnityEngine;

/**
 * Moves a transform horizontally. So this script manages if transform is moving left or right.
 */
public class Move : MoveAbs {
	
	public static Vector3 MOVEMENT_AXIS = Vector3.right;
	
	private bool moving;
	private Vector3 dir; // only for accessing sign direction
	private Jump jump;
	
	private static Vector3 VECTOR3_ONE = new Vector3(1f,1f,1f);
	
	// Use this for initialization
	void Start () {
		moving = true;
		jump = GetComponent<Jump>();
	}
	
	public override void move (Vector3 dirAndPow) {
		if (!moving)
			return;
		dir = dirAndPow;
		float deltaTime = Time.deltaTime;
		transform.Translate(dirAndPow.x * MOVEMENT_AXIS.x * deltaTime, 
		                    dirAndPow.y * MOVEMENT_AXIS.y * deltaTime,
		                    dirAndPow.z * MOVEMENT_AXIS.z * deltaTime);
	}
	
	public override void move(float val) {
		move(VECTOR3_ONE * val);
	}
	
	public override void stopMoving () {
		moving = false;
	}
	
	public void enableMoving () {
		moving = true;
		// set the correct sprite animation
		if (animComponent != null) {
			if (jump == null || !jump.IsJumping()) {
				animComponent.setFPS(animFPS);
				animComponent.setRowLimits(rowStartAnim, rowLengthAnim);
				animComponent.setColLimits(maxColsAnimInRow, colStartAnim, colLengthAnim);
				animComponent.setPingPongAnim(pingPongAnim);
				animComponent.Play();
			}
		}
	}
	
	public override bool isMoving () {
		return moving;
	}
	
	/**
	 * Typically used ot track direction's changes
	 */
	public Vector3 getDir () {
		return dir;
	}
}
