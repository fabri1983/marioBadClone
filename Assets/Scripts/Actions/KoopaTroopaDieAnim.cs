using UnityEngine;

public class KoopaTroopaDieAnim : MonoBehaviour {
	
	public float kickedSpeed = 12f;
	
	private Chase chase;
	private Patrol patrol;
	private bool hidden, bouncing, dying;
	private DieAnimConfig dieAC;
	
	// Use this for initialization
	void Awake () {
		patrol = GetComponent<Patrol>();
		chase = GetComponent<Chase>();
		dieAC = GetComponentInChildren<DieAnimConfig>();
		hidden = false;
		bouncing = false;
		dying = false;
	}
	
	void Update () {
		// set the correct sprite animation
		if (hidden) {
			dieAC.animComp.setFPS(dieAC.animFPS);
			dieAC.animComp.setRowLimits(dieAC.rowStartAnim, dieAC.rowLengthAnim);
			dieAC.animComp.setColLimits(dieAC.maxColsAnimInRow, dieAC.colStartAnim, dieAC.colLengthAnim);
			dieAC.animComp.setPingPongAnim(dieAC.pingPongAnim);
			dieAC.animComp.Play();
		}
	}
	
	public void die () {
		dying = true;
		bouncing = false;
		patrol.stopPatrol();
		Update(); // set correct sprite anim
	}
	
	public void hide () {
		chase.setOperable(false); // chase is not allowed to work anymore
		hidden = true;
		bouncing = false;
		patrol.stopPatrol();
		Update(); // set correct sprite anim
	}
	
	public void bounce (float dir) {
		hidden = true;
		bouncing = true;
		// tell patrol to start moving like crazy
		patrol.enablePatrol();
		patrol.setMoveSpeed(kickedSpeed);
		patrol.setNewDir(dir); 
		Update(); // set correct sprite anim
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
