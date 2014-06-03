using UnityEngine;

public class GoombaDieAnim : MonoBehaviour {
	
	private bool dying;
	private static float CRUNCH_PROPORTION = 0.8f;
	private static Vector3 CRUNCH_VECTOR = Vector3.up;
	
	void Awake () {
		dying = false;
	}
	
	public void die () {
		// avoid re doing the animation
		if (dying)
			return;
		
		dying = true;
		
		// resize the game object, it also affects the chipmunk shape
		transform.localScale -= CRUNCH_VECTOR * CRUNCH_PROPORTION;
		// transform the body downwards
		ChipmunkBody body = GetComponent<ChipmunkBody>();
		float centerOffsetY = ((1f - CRUNCH_PROPORTION)*0.5f) * GetComponentInChildren<AnimateTiledTexture>().renderer.bounds.size.y;
		Vector3 thePos = body.position;
		thePos.y -= centerOffsetY*(1f+CRUNCH_PROPORTION);
		body.position = thePos;
		
		// disable the game object (not children) to avoid ugly upward movement due to
		// unknown behavior (maybe because its move script continues working?)
		gameObject.active = false;
	}
	
	public bool isDying () {
		return dying;
	}
}
