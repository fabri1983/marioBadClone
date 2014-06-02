using UnityEngine;

public class Chase : MonoBehaviour {
	
	public bool allowChasing = false;
	public float movePower = 6f;
	
	private WalkAbs walk;
	private Patrol patrol;
	Transform target;
	private float lastDir;
	private bool wasChasing, stop;
	
	// Use this for initialization
	void Start () {
		walk = GetComponent<WalkAbs>();
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
			lastDir = Mathf.Sign(target.position.x - transform.position.x);
			walk.walk(lastDir * movePower);
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
		walk.stopWalking();
	}
	
	public void enableChasing () {
		stop = false;
		walk.enableWalking();
	}
	
	public Transform getTarget () {
		return target;
	}
}
