using UnityEngine;

public class KoopaTroopaDieAnim : MonoBehaviour {
	
	public float moveBerserkPower = 12f;
	
	private Jump jump;
	private Chase chase;
	private Patrol patrol;
	private bool hidding, bouncing, dying;
	private GameObject koopaFull, koopaHide;
	
	// Use this for initialization
	void Start () {

		hidding = false;
		bouncing = false;
		dying = false;
		
		// get the reference of every script involved in the koopa troopa life cycle
		jump = GetComponent<Jump>();
		chase = GetComponent<Chase>();
		patrol = GetComponent<Patrol>();
		
		// get child components
		koopaFull = transform.FindChild("KoopaTroopaFull").gameObject;
		koopaHide = transform.FindChild("KoopaTroopaHide").gameObject;
		// disable the koopa hide game object
		koopaHide.active = false;
	}
	
	// Update is called once per frame
	void Update () {
	
		if (dying)
			dieAnim();
	}
	
	private void dieAnim () {
		// currently only destroys the object
		Destroy(gameObject);
	}
	
	public void die () {
		dying = true;
		jump.setForeverJump(false);
		chase.stopChasing();
	}
	
	public void changeToHide () {
		hidding = true;
		bouncing = false;
		// disable chasing
		chase.stopChasing();
		jump.setForeverJump(false);
		koopaHide.active = true;
		koopaFull.active = false;
	}
	
	public void changeToBouncing (float dir) {
		hidding = false;
		bouncing = true;
		// change koopa troopa layer and tag
		gameObject.layer = LevelManager.POWERUP_LAYER;
		gameObject.tag = "PowerUp";
		// tell patrol to start moving like crazy
		patrol.enablePatrol();
		patrol.setMovePower(moveBerserkPower);
		patrol.setNewDir(dir); 
	}
	
	public bool isFull () {
		return !hidding && !bouncing;
	}
	
	public bool isHide () {
		return hidding;
	}
	
	public bool isBouncing () {
		return bouncing;
	}
}
