using UnityEngine;

/// <summary>
/// Renders a fullscreen Quad with a texture on it.
/// This version has scroll texture option according to camera position or manual offset.
/// On screen redimension the quad's mesh (vers and UVs) is adjusted to cover the screen.
/// </summary>
[ExecuteInEditMode]
public class BGQuadParallax : MonoBehaviour, IScreenLayout {

	public Texture2D bgTexture;
	public float speed = 1f; // factor to be applied to offset calculation
	public Vector2 manualStep = Vector2.zero; // use 0 if you want the scroll happens according main cam movement
	public Vector2 tiling = Vector2.one; // this scales the texture if you want to show just a portion of it
	public TextureWrapMode wrapMode = TextureWrapMode.Repeat;
	
	private const float epsilon = 0.09f; // used when getting difference 
	private Vector2 oldCamPos = Vector2.zero;
	private Vector2 accumOffset = Vector2.zero;
	private Vector2 offset = Vector2.zero; // offset depending on player's spawn position
	private Vector2 levelExtent = Vector2.zero; // level length according the game objects that define the level extent
	
	void Awake () {
		// deactivate the game object if no texture
		if (!bgTexture) {
			gameObject.SetActiveRecursively(false);
			return;
		}
		
		// set texture wrap mode
		renderer.sharedMaterial.mainTexture.wrapMode = wrapMode;
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
		
		fillScreen(); // make this game object to fill the viewport
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

		// if manualOffset is not (0,0) then apply a fixed offset if camera is moving
		if (!Vector2.zero.Equals(manualStep)) {
			Vector2 diff = camPos - oldCamPos;
			// cam is going right, then offset left
			if (diff.x > epsilon)
				accumOffset.x += manualStep.x * speed;
			// cam is going left, then offset right
			else if (diff.x < -epsilon)
				accumOffset.x -= manualStep.x * speed;
			// cam is going up, then offset downwards
			if (diff.y > epsilon)
				accumOffset.y += manualStep.y * speed;
			// cam is going down, then offset upwards
			else if (diff.y < -epsilon)
				accumOffset.y -= manualStep.y * speed;
		}
		// apply an offset according 
		else {
			accumOffset.x = (camPos.x / levelExtent.x) * speed;
			accumOffset.y = (camPos.y / levelExtent.y) * speed;
		}
		
		renderer.sharedMaterial.SetTextureOffset("_MainTex", accumOffset + offset);
		oldCamPos.Set(camPos.x, camPos.y);
	}
	
	private void fillScreen () {
		GameObjectTools.setScreenCoverage(Camera.main, this.gameObject);
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
