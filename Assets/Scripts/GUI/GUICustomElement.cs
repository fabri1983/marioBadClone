using UnityEngine;

/// <summary>
/// Scales the game object according to screen size and user defined coverage to act as a GUI element.
/// On screen redimension the game object's z position and its scale are adjusted.
/// </summary>
[ExecuteInEditMode]
public class GUICustomElement : MonoBehaviour, IScreenLayout {
	
	public Texture2D bgTexture;
	public Vector2 size = Vector2.one;
	
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
		locateInScreen(); // make this game object to be located correctly in viewport
		renderer.sharedMaterial.mainTexture = bgTexture;
	}

	void OnDestroy () {
		ScreenLayoutManager.Instance.remove(this);
#if UNITY_EDITOR
		// this is in case this script is used in editor mode
		if (!bgTexture)
			renderer.sharedMaterial.mainTexture = bgTexture;
#endif
	}

#if UNITY_EDITOR
	void Update () {
		// if in editor mode we change the texture this will update the material
		if (bgTexture && !bgTexture.name.Equals(renderer.sharedMaterial.mainTexture.name))
			renderer.sharedMaterial.mainTexture = bgTexture;
		// only in Editor Mode: update in case any change from Inspector
		if (!Application.isPlaying)
			locateInScreen();
	}
#endif
	
	private void locateInScreen () {
		GameObjectTools.setScreenLocation(Camera.main, transform, size);
	}
	
	public void updateSizeAndPosition () {
		locateInScreen();
	}
}
