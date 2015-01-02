using UnityEngine;

public class BrickInvisibleTrigger : MonoBehaviour {
	
	private Brick brickScript;
	
	// Use this for initialization
	void Start () {
		
		brickScript = transform.parent.GetComponent<Brick>();
		
		// when is invisible then needs to only activate bottom trigger
		if (brickScript.isInvisible) {
			brickScript.gameObject.collider.enabled = false;
			brickScript.gameObject.renderer.enabled = false;
#if UNITY_4_AND_LATER
			transform.parent.FindChild("BrickTop").gameObject.SetActive(false);
#else
			transform.parent.FindChild("BrickTop").gameObject.active = false;
#endif
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
