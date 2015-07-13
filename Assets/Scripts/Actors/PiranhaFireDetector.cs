using UnityEngine;

public class PiranhaFireDetector : MonoBehaviour {
	
	private Transform target;
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnTriggerEnter (Collider collider) {

		if (collider.tag.Equals("Mario")) {
			target = collider.transform;
		}
	}
	
	void OnTriggerExit (Collider collider) {

		if (collider.tag.Equals("Mario")) {
			target = null;
		}
	}
	
	public Transform getTarget () {
		return target;
	}
}
