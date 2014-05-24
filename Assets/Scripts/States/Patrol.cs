using UnityEngine;

public class Patrol : MonoBehaviour {
	
	public float movePower = 6f;
	
	private MoveAbs move;
	private Vector3 dir;
	private bool stop;
	
	// Use this for initialization
	void Start () {
		dir = Vector3.right; // initial normalized direction 
		move = GetComponent<MoveAbs>();
		stop = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (stop)
			return;
		// always in movement. The only opportunity to stop is when chase action takes place
		move.move(dir * movePower);
	}
	
	void OnCollisionEnter (Collision collision) {
		// change direction of movement whenever hit something
		if (!collision.transform.tag.Equals("Mario")) {
			// if normal.x is near to 1 it means it is a hit against something perpendicular to floor (a wall)
			float absX = Mathf.Abs(collision.contacts[0].normal.x);
			if (absX > 0.8f)
				dir.x *= -1f;
		}
	}
	
	/**
	 * Set direction only (expected to be normalized)
	 */
	public void setNewDir (Vector3 pDir) {
		dir = pDir;
	}
	
	public void setMovePower (float val) {
		movePower = val;
	}
	
	public void stopPatrol () {
		stop = true;
		move.stopMoving();
	}
	
	public void enablePatrol () {
		stop = false;
	}
}
