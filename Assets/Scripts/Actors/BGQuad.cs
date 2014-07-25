using UnityEngine;

/// <summary>
/// Renders a fullscreen Quad with a texture on it.
/// On screen redimension the quad z position and scaling is adjusted.
/// </summary>
[ExecuteInEditMode]
public class BGQuad : MonoBehaviour, IScreenLayout {
	
	public Texture2D bgTexture;
	
	void Awake () {
		if (!bgTexture) {
			gameObject.SetActiveRecursively(false);
			return;
		}
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
#if UNITY_EDITOR
		if (bgTexture)
			renderer.sharedMaterial.mainTexture = bgTexture;
#endif
	}
	
	private void fillScreen () {
		GameObjectTools.setScreenCoverage(Camera.main, this.gameObject);
	}
	
	public void updateSizeAndPosition () {
		fillScreen();
	}
}
