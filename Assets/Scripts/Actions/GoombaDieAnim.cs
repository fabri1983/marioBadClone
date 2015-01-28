#if !(UNITY_3_0 || UNITY_3_0_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5)
#define UNITY_4_AND_LATER
#endif
using UnityEngine;

public class GoombaDieAnim : MonoBehaviour {
	
	private bool dying;
	private const float CRUNCH_PROPORTION = 0.8f;
	private static Vector3 CRUNCH_VECTOR = Vector3.up;
	private float sizeY;
	
	void Awake () {
		dying = false;
	}
	
	void Start () {
		sizeY = GetComponentInChildren<AnimateTiledTexture>().renderer.bounds.size.y;
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
		float centerOffsetY = ((1f - CRUNCH_PROPORTION)*0.5f) * sizeY;
		Vector3 thePos = body.position;
		thePos.y -= centerOffsetY*(1f+CRUNCH_PROPORTION);
		body.position = thePos;
		
		// disable the game object (not children) to avoid ugly upward movement due to
		// unknown behavior (maybe because its move script continues working?)
#if UNITY_4_AND_LATER
		gameObject.SetActive(false);
#else
		gameObject.active = false;
#endif
	}
	
	public bool isDying () {
		return dying;
	}
}
