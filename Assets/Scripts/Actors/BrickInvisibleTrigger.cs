#if !(UNITY_3_0 || UNITY_3_0_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5)
#define UNITY_4_AND_LATER
#endif
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
