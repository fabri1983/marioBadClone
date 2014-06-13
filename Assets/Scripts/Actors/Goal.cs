using UnityEngine;

public class Goal : MonoBehaviour {
	
	public Vector3 dirEntrance = Vector3.up;
	
	private bool isInside = false;
	
	// Update is called once per frame
	void Update () {
	
		if (isInside &&
			((dirEntrance.x < 0 && Input.GetAxis("Horizontal") < -0.2f) 
			|| (dirEntrance.x > 0 && Input.GetAxis("Horizontal") > 0.2f)
			|| (dirEntrance.y < 0 && Input.GetAxis("Vertical") < -0.2f)
			|| (dirEntrance.y > 0 && Input.GetAxis("Vertical") > 0.2f)) ) {
			
			isInside = false;
			LevelManager.Instance.loadNextLevel();
		}
	}
	
	void OnTriggerEnter (Collider collider) {
		if (collider.tag.Equals("Mario"))
			isInside = true;
	}
	
	void OnTriggerExit (Collider collider) {
		if (collider.tag.Equals("Mario"))
			isInside = false;
	}
}
