using UnityEngine;

public class PiranhaAnim : MonoBehaviour {
	
	private float moveAmount = 4f;
	private float movTrack, top, bottom;
	private float idleTime = 1.5f;
	private float idleCounter;
	private bool goingDown, idle, stop, keepHide;
	
	// Use this for initialization
	void Start () {
		
		// piranha fire doesn't have renderer, so search down in his children
		if (gameObject.GetComponent<Renderer>() == null)
			top = transform.FindChild("PiranhaFire00").GetComponent<Renderer>().bounds.size.y;
		else
			top = GetComponent<Renderer>().bounds.size.y;
		bottom = 0f;
		goingDown = true;
		idle = false;
		idleCounter = 0f;
	}
	
	// Update is called once per frame
	void Update () {
	
		// stop is true only when the piranha is inside the tube and keepHide is true (is inside safe zone)
		if (stop)
			return;
		
		if (idle) {
			idleCounter += Time.deltaTime;
			if (idleCounter >= idleTime) {
				idleCounter = 0f;
				idle = false;
				// when finished idle inside the tube and flag keepHide is true (means Mario is in dead zone) then don't come out
				if (goingDown && keepHide)
					stop = true;
			}
		}
		else if (goingDown) {
			movTrack -= moveAmount * Time.deltaTime;
			if (movTrack < bottom) {
				goingDown = false;
				idle = true;
			}
			else 
				transform.Translate(0f, moveAmount * Time.deltaTime, 0f);
		}
		else {
			movTrack += moveAmount * Time.deltaTime;
			if (movTrack >= top + 0.5f) {
				goingDown = true;
				idle = true;
			}
			else 
				transform.Translate(0f, -moveAmount * Time.deltaTime, 0f);
		}
	}
	
	public bool isOut () {
		return idle && !goingDown;
	}
	
	public void setKeepHide (bool val) {
		keepHide = val;
		if (!val)
			stop = false;
	}
}
