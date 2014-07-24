using UnityEngine;

/// <summary>
/// Renders a fullscreen Quad with a texture on it.
/// This version has scroll texture option according to camera position or manual offset.
/// On screen redimension the quad's mesh (vers and UVs) is adjusted to cover the screen.
/// </summary>
[ExecuteInEditMode]
public class BGQuadParallax : MonoBehaviour, IScreenLayout {

	public Texture2D bgTexture;
	public Vector2 manualStep = Vector2.zero; // use 0 if you want the scroll happens according main cam movement
	public Vector2 tiling = Vector2.one; // this scales the texture if you want to show just a portion of it
	
	private const float epsilon = 0.09f;
	private Vector2 oldCamPos = Vector2.zero;
	private Vector2 accumOffset = Vector2.zero;
	private Vector2 offset = Vector2.zero; // offset depending on player's spawn position
	private Vector2 levelExtent = Vector2.zero; // level length according the game objects that define the level extent
	
	void Awake () {
		if (!bgTexture) {
			gameObject.SetActiveRecursively(false);
			return;
		}
		
		// register this class with ScreenLayoutManager for screen resize event
		ScreenLayoutManager.Instance.register(this);
		// initializes as inifnite to avoid NaN in division operation
		levelExtent.x = float.PositiveInfinity;
		levelExtent.y = float.PositiveInfinity;
	}
	
	void Start () {
		if (!bgTexture)
			return;
		
		// set current camera position
		Vector2 camPos = Camera.main.transform.position;
		oldCamPos.Set(camPos.x, camPos.y);
		
		fillScreen(); // set the quad to fill the viewport
		renderer.sharedMaterial.mainTexture = bgTexture;
		renderer.sharedMaterial.SetTextureScale("_MainTex", tiling);
	}
	
	void OnDestroy () {
		if (bgTexture) {
			renderer.sharedMaterial.SetTextureOffset("_MainTex", Vector2.zero);
#if UNITY_EDITOR
			// this is in case this script is used in editor mode
			renderer.sharedMaterial.mainTexture = bgTexture;
#endif
		}
	}

	void LateUpdate () {
		updateBGOffset();
	}
	
	private void updateBGOffset () {
		Vector2 camPos = Camera.main.transform.position; // get the scene camera position
		Vector2 diff = camPos - oldCamPos;

		// if manualOffset is not (0,0) then apply a fixed offset according to camera's movement
		if (!Vector2.zero.Equals(manualStep)) {
			// cam is going right, then offset to left
			if (diff.x > epsilon)
				accumOffset.x += manualStep.x;
			// cam is going left, then offset to right
			else if (diff.x < -epsilon)
				accumOffset.x -= manualStep.x;
			// cam is going up, then offset to down
			if (diff.y > epsilon)
				accumOffset.y += manualStep.y;
			// cam is going down, then offset to up
			else if (diff.y < -epsilon)
				accumOffset.y -= manualStep.y;
		}
		else {
			accumOffset.x = (camPos.x / levelExtent.x);
			accumOffset.y = (camPos.y / levelExtent.y);
		}
		
		renderer.sharedMaterial.SetTextureOffset("_MainTex", accumOffset + offset);
		oldCamPos.Set(camPos.x, camPos.y);
	}
	
	private void fillScreen () {
		Camera cam = Camera.main; // get the scene camera named MainCamera
		
		float pos = (cam.nearClipPlane + 0.01f);
		transform.position = cam.transform.position + cam.transform.forward * pos;
		
		float h, w;
		if(!cam.orthographic) {
			h = Mathf.Tan(cam.fov * Mathf.Deg2Rad * 0.5f) * pos * 2f;
			w = h * cam.aspect;
		}
		else {
			h = cam.orthographicSize * 2f;
			w = h / Screen.height * Screen.width;
		}
		
		// scale the game object to adjust it according screen bounds
		Vector3 theScale = transform.localScale;
		theScale.x = w;
		theScale.y = h;
		theScale.z = 0f;
		transform.localScale = theScale;
	}
	
	public void updateSizeAndPosition () {
		fillScreen();
	}
	
	/**
	 * Takes a world position (x,y) and converts it to texture offset.
	 * Use this to set the initial offset according player's spawn position.
	 */ 
	public void setOffsetWorldCoords (float x, float y) {
		offset.Set(x / levelExtent.x, y / levelExtent.y);
	}
	
	/**
	 * Set the length and height in world units of the level where the parallax background will scroll.
	 */ 
	public void setLevelExtentWorldUnits (float x, float y) {
		levelExtent.Set(x, y);
	}
}
