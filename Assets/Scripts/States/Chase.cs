using UnityEngine;

public class Chase : MonoBehaviour {
	
	public bool allowChasing = false;
	public float movePower = 6f;
	
	private MoveAbs move;
	private Patrol patrol;
	Transform target;
	private Vector3 lastDir;
	private bool wasChasing, stop;
	
	// Use this for initialization
	void Start () {
		move = GetComponent<MoveAbs>();
		patrol = GetComponent<Patrol>();
		wasChasing = false;
		stop = false;
	}
	
	// Update is called once per frame
	void Update () {
		
		if (stop)
			return;
		
		// there is a target which patrol to?
		if (target) {
			if (patrol != null)
				patrol.stopPatrol();
			// calculate vector direction
			lastDir = target.position - transform.position;
			lastDir.Normalize();
			move.move(lastDir * movePower);
		}
		// restore last direction of movement for patrolling
		else if (wasChasing) {
			wasChasing = false;
			if (patrol != null) {
				patrol.setNewDir(lastDir);
				patrol.enablePatrol();
			}
		}
	}
	
	void OnTriggerEnter (Collider collider) {
		if (allowChasing) {
			if (collider.tag.Equals("Mario")) {
				target = collider.transform;
				wasChasing = true;
			}
		}
	}
	
	void OnTriggerExit (Collider collider) {
		if (allowChasing) {
			if (collider.tag.Equals("Mario"))
				target = null;
		}
	}
	
	public void stopChasing () {
		stop = true;
	}
	
	public void enableChasing () {
		stop = false;
	}
	
	public Transform getTarget () {
		return target;
	}
}
