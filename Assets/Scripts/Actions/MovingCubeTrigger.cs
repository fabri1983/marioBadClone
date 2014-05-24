using UnityEngine;

public class MovingCubeTrigger : MonoBehaviour {
	
	private Transform movingCubeLogical;
	
	// Use this for initialization
	void Start () {
	
		movingCubeLogical = transform.parent.parent;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnTriggerStay (Collider collider) {
	
		if (collider.name.Equals("Mario")) {
			collider.transform.parent = movingCubeLogical;
		}
	}
	
	void OnTriggerExit (Collider collider) {
	
		if (collider.name.Equals("Mario")) {
			collider.transform.parent = null;
		}
	}
}
