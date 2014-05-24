using UnityEngine;

public class Goal : MonoBehaviour {
	
	public Vector3 dirEntrance = Vector3.up;
	
	private bool isInside = false;
	
	// Update is called once per frame
	void Update () {
	
		if (isInside &&
			((dirEntrance.x < 0 && Input.GetKey(KeyCode.LeftArrow)) 
			|| (dirEntrance.x > 0 && Input.GetKey(KeyCode.RightArrow))
			|| (dirEntrance.y < 0 && Input.GetKey(KeyCode.DownArrow))
			|| (dirEntrance.y > 0 && Input.GetKey(KeyCode.UpArrow))) ) {
			
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
