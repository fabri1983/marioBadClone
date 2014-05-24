using UnityEngine;

public class GhostAnim : MonoBehaviour {
	
	private Chase chase;
	private Ghost ghost;
	private GameObject ghostAngry, ghostQuite;
	
	// Use this for initialization
	void Start () {
		
		chase = GetComponent<Chase>();
		ghost = GetComponent<Ghost>();
		// get child components
		ghostAngry = transform.FindChild("GhostAngry").gameObject;
		ghostQuite = transform.FindChild("GhostQuite").gameObject;
		// disable the ghost angry game object
		ghostAngry.active = false;
	}
	
	// Update is called once per frame
	void Update () {
		
		// cool state
		if (chase.getTarget() == null || ghost.isTargetFacingMe()) {
			ghostAngry.active = false;
			ghostQuite.active = true;
		}
		// angry state
		else {
			ghostAngry.active = true;
			ghostQuite.active = false;
		}
	}
}
