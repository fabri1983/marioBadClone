using UnityEngine;

public class GoombaDieAnim : MonoBehaviour, IReloadable {
	
	private bool dying = false;
	private const float CRUNCH_PROPORTION = 0.8f;
	private static Vector3 CRUNCH_VECTOR = Vector3.up;
	private float sizeY;
	private ChipmunkBody body;
	
	void Awake () {
		body = GetComponent<ChipmunkBody>();
	}
	
	void Start () {
		ReloadableManager.Instance.register(this as IReloadable, transform.position);
		sizeY = GetComponentInChildren<AnimateTiledTexture>().renderer.bounds.size.y;
	}
	
	void OnDestroy () {
		ReloadableManager.Instance.remove(this as IReloadable);
	}
	
	public bool isDying () {
		return dying;
	}
	
	public void die () {
		if (dying)
			return;
		dying = true;
		crunchComponent();
	}
	
	private void crunchComponent () {
		// resize the game object, it also affects the chipmunk shape
		transform.localScale -= CRUNCH_VECTOR * CRUNCH_PROPORTION;
		// transform the body downwards
		float centerOffsetY = ((1f - CRUNCH_PROPORTION) * 0.5f) * sizeY;
		Vector3 thePos = transform.position;
		thePos.y -= centerOffsetY * (1f + CRUNCH_PROPORTION);
		transform.position = thePos;
		body._UpdatedTransform();
	}
	
	private void stretchComponent () {
		// resize the game object, it also affects the chipmunk shape
		transform.localScale += CRUNCH_VECTOR * CRUNCH_PROPORTION;
		// transform the body downwards
		float centerOffsetY = ((1f - CRUNCH_PROPORTION) * 0.5f) * sizeY;
		Vector3 thePos = transform.position;
		thePos.y += centerOffsetY * (1f + CRUNCH_PROPORTION);
		transform.position = thePos;
		body._UpdatedTransform();
	}
	
	public void onReloadLevel (Vector3 pos) {
		if (dying == true)
			stretchComponent();
		dying = false;
	}

}
