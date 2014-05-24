using UnityEngine;

public class Ghost : MonoBehaviour {
	
	private Fly fly;
	private Move move;
	private Chase chase;
	private bool goingRight;
	
	// Use this for initialization
	void Start () {
		
		fly = GetComponent<Fly>();
		fly.setAutomaticFly(true);
		move = GetComponent<Move>();
		chase = transform.GetComponent<Chase>();
		
		// assuming koopa starts going left
		goingRight = false;
	}
	
	// Update is called once per frame
	void Update () {
		
		if (chase.getTarget() == null) {
			// setting null as target the fly script will stop
			fly.fly(null, 0f, 0f);
			return;
		}
		
		// unique behavior: when change direction then scale by -1 in X to simulate rotation
		if (goingRight && move.getDir().x < 0f) {
			Vector3 theScale = transform.localScale;
			theScale.x = transform.localScale.x * -1f;
			transform.localScale = theScale;
			goingRight = false;
		}
		else if (!goingRight && move.getDir().x > 0f) {
			Vector3 theScale = transform.localScale;
			theScale.x = transform.localScale.x * -1f;
			transform.localScale = theScale;
			goingRight = true;
		}
		
		// disable chasing if Mario is facing the ghost
		if (isTargetFacingMe()) {
			// setting null as target the fly script will stop
			fly.fly(null, 0f, 0f);
			chase.stopChasing();
		}
		else {
			chase.enableChasing();
			// only matters the target since the fly is in automatic mode
			fly.fly(transform, 0f, 0f);
		}
	}
	
	void OnCollisionEnter (Collision collision) {
		
		// when collides with Mario, then kill it
		if (collision.transform.tag.Equals("Mario")) {
			move.stopMoving();
			LevelManager.Instance.loseGame(true);
		}
		// if a powerUp (ie fireball) hits the goomba then it dies
		else if (collision.transform.tag.Equals("PowerUp")) {
			Destroy(collision.gameObject);
			Destroy(gameObject);
		}
	}
	
	public bool isTargetFacingMe () {
		// if Mario's forward vector is facing towards ghost position then Dot product will be < 0.
		// Also consider when Dot returns 0.
		Vector3 ghostVector = chase.getTarget().position - transform.position;
		return Vector3.Dot(chase.getTarget().right, ghostVector) <= 0f;
	}
}
