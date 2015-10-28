using UnityEngine;

public class BrickInvisibleTrigger : MonoBehaviour {
	
	private Brick brickScript;
	
	// Use this for initialization
	void Start () {
		
		brickScript = transform.parent.GetComponent<Brick>();
		
		// when is invisible then needs to only activate bottom trigger
		if (brickScript.isInvisible) {
			brickScript.gameObject.GetComponent<Collider>().enabled = false;
			brickScript.gameObject.GetComponent<Renderer>().enabled = false;
			GameObjectTools.setActive(transform.parent.FindChild("BrickTop").gameObject, false);
		}
		// if not then destroy the bottom trigger
		else
			Destroy(this.gameObject);
	}
	
	void OnTriggerEnter (Collider collider) {

		if (collider.tag.Equals("Mario") && brickScript.hitFromBelow(collider.transform)) {
			brickScript.executeHit(collider);
			
			Destroy(this.gameObject);
		}
	}
}
