using UnityEngine;

/// <summary>
/// Set the game object in such way that covers all the sreen.
/// On screen redimension the quad z position and scaling is adjusted.
/// </summary>
[ExecuteInEditMode]
public class BGQuad : MonoBehaviour, IScreenLayout {
	
	public Texture2D bgTexture;
	public Vector2 coverage = Vector2.one;
	
	void Awake () {
		if (!bgTexture) {
			gameObject.SetActiveRecursively(false);
			return;
		}
		else
			gameObject.SetActiveRecursively(true);
		
		// register this class with ScreenLayoutManager for screen resize event
		ScreenLayoutManager.Instance.register(this);
	}
	
	void Start () {
		if (!bgTexture)
			return;
		fillScreen(); // make this game object to fill the viewport
		renderer.sharedMaterial.mainTexture = bgTexture;
	}

	void OnDestroy () {
		ScreenLayoutManager.Instance.remove(this);
#if UNITY_EDITOR
		if (bgTexture)
			renderer.sharedMaterial.mainTexture = bgTexture;
#endif
	}
	
	private void fillScreen () {
		GameObjectTools.setScreenCoverage(Camera.main, transform, coverage);
	}
	
	public void updateSizeAndPosition () {
		fillScreen();
	}
}
