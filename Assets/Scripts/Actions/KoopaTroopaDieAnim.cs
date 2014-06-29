using UnityEngine;

public class KoopaTroopaDieAnim : MonoBehaviour {
	
	public float kickedSpeed = 12f;
	public float hideColliderProportion = 0.80f;
	
	private Chase chase;
	private Patrol patrol;
	private bool hidden, bouncing, dying;
	private DieAnimConfig dieAC;
	private float colliderCenterY, centerOffsetY;
	private ChipmunkBoxShape box;
	
	// Use this for initialization
	void Awake () {
		patrol = GetComponent<Patrol>();
		chase = GetComponent<Chase>();
		dieAC = GetComponentInChildren<DieAnimConfig>();
		hidden = false;
		bouncing = false;
		dying = false;
		
		// take the collider and some useful values
		ChipmunkBoxShape[] boxes = GetComponents<ChipmunkBoxShape>();
		for (int i=0,c=boxes.Length; i<c; ++i) {
			box = boxes[i];
			// the koopa has two chipmunk boxes, take the correct one
			if ("KoopaTroopa".Equals(box.collisionType))
				break;
		}
		colliderCenterY = box.center.y;
		centerOffsetY = ((1f - hideColliderProportion)*0.5f) * box.size.y;
	}
	
	void Update () {
		// set the correct sprite animation
		if (hidden)
			setAnim();
	}
	
	private void setAnim () {
		dieAC.animComp.setFPS(dieAC.animFPS);
		dieAC.animComp.setRowLimits(dieAC.rowStartAnim, dieAC.rowLengthAnim);
		dieAC.animComp.setColLimits(dieAC.maxColsAnimInRow, dieAC.colStartAnim, dieAC.colLengthAnim);
		dieAC.animComp.setPingPongAnim(dieAC.pingPongAnim);
		dieAC.animComp.Play();
	}
	
	/*public void die () {
		dying = true;
		bouncing = false;
		patrol.stopPatrol();
		setAnim(); // set correct sprite anim
	}*/
	
	public void hide () {
		if (!hidden) {
			// resize the collider
			Vector3 theSize = box.size;
			theSize.y *= hideColliderProportion;
			box.size = theSize;
			// transform the collider
			Vector3 theCenter = box.center;
			theCenter.y -= centerOffsetY;
			box.center = theCenter;
		}		
		chase.setOperable(false); // chase is not allowed to work anymore
		hidden = true;
		bouncing = false;
		patrol.stopPatrol();
		setAnim(); // set correct sprite animation
	}
	
	public void bounce (float dir) {
		hidden = true;
		bouncing = true;
		// tell patrol to start moving like crazy
		patrol.enablePatrol();
		patrol.setMoveSpeed(kickedSpeed);
		patrol.setNewDir(dir);
		setAnim(); // set correct sprite animation
	}
	
	public bool isDying () {
		return dying;
	}
	
	public bool isHidden () {
		return hidden;
	}
	
	public bool isBouncing () {
		return bouncing;
	}
}
