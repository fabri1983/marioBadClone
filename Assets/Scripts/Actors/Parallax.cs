using UnityEngine;

/// <summary>
/// Scales the game object acording to screen size and user defined coverage.
/// This version has scroll texture option according to camera position or manual offset.
/// On screen redimension the game object is adjusted to cover the screen or part of it.
/// </summary>
[ExecuteInEditMode]
public class Parallax : MonoBehaviour, IScreenLayout {

	public Texture2D bgTexture;
	public float speed = 1f; // factor to be applied to offset calculation
	public Vector2 manualStep = Vector2.zero; // use 0 if you want the scroll happens according main cam movement
	public Vector2 tiling = Vector2.one; // this scales the texture if you want to show just a portion of it
	public TextureWrapMode wrapMode = TextureWrapMode.Repeat;
	public Vector2 screenCoverage = Vector2.one;
	
	private const float epsilon = 0.01f; // used only when manual step is not (0,0)
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
		else
			gameObject.SetActiveRecursively(true);
		
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
		ScreenLayoutManager.Instance.remove(this);
		
		if (bgTexture) {
			renderer.sharedMaterial.SetTextureOffset("_MainTex", Vector2.zero);
#if UNITY_EDITOR
			// this is in case this script is used in editor mode
			renderer.sharedMaterial.mainTexture = bgTexture;
#endif
		}
	}

	void LateUpdate () {
		updateOffset();
	}
	
	private void updateOffset () {
		Vector2 camPos = Camera.main.transform.position; // get the scene camera position

		// if manualOffset is not (0,0) then apply a fixed offset if camera is moving
		if (!Vector2.zero.Equals(manualStep)) {
			Vector2 diff = camPos - oldCamPos;
			// cam is going right, then offset left
			if (diff.x > epsilon)
				accumOffset.x += manualStep.x;
			// cam is going left, then offset right
			else if (diff.x < -epsilon)
				accumOffset.x -= manualStep.x;
			// cam is going up, then offset downwards
			if (diff.y > epsilon)
				accumOffset.y += manualStep.y;
			// cam is going down, then offset upwards
			else if (diff.y < -epsilon)
				accumOffset.y -= manualStep.y;

			accumOffset += offset;
		}
		// apply an offset according player's position inside the level extent
		else {
			accumOffset.x = (camPos.x / levelExtent.x);
			accumOffset.y = (camPos.y / levelExtent.y);
		}
		
		renderer.sharedMaterial.SetTextureOffset("_MainTex", accumOffset * speed);
		oldCamPos.Set(camPos.x, camPos.y);
	}
	
	private void fillScreen () {
		GameObjectTools.setScreenCoverage(Camera.main, transform, screenCoverage);
	}
	
	public void updateSizeAndPosition () {
		fillScreen();
	}
	
	/// <summary>
	/// Takes a world position (x,y) and converts it to texture offset.
	/// Use this to set the initial offset according player's spawn position.
	/// </summary>
	public void setOffsetWorldCoords (float x, float y) {
		// if tilling is 1 then avoid the offset
		if (tiling.x == 1f)
			offset.x = 0;
		else
			offset.x = x / levelExtent.x;
		// if tilling is 1 then avoid the offset
		if (tiling.y == 1f)
			offset.y = 0;
		else
			offset.y = y / levelExtent.y;
	}
	
	/// <summary>
	/// Set the length and height in world units of the level where the parallax background will scroll.
	/// </summary>
	public void setLevelExtentWorldUnits (float x, float y) {
		levelExtent.Set(x, y);
	}
}
