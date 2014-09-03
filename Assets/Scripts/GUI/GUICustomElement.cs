using UnityEngine;

/// <summary>
/// Scales the game object according to screen size and user defined coverage to act as a GUI element.
/// On screen redimension the game object's z position and its scale are adjusted.
/// </summary>
[ExecuteInEditMode]
public class GUICustomElement : MonoBehaviour, IScreenLayout {
	
	public Texture2D texture;
	public TextureWrapMode wrapMode = TextureWrapMode.Repeat;
	public Vector2 size = Vector2.one; // pixels or proportion
	public bool sizeAsPixels = false;
	
	void Awake () {
		// deactivate the game object if no texture
		if (!texture) {
			gameObject.SetActiveRecursively(false);
			return;
		}
		else
			gameObject.SetActiveRecursively(true);
		
		// register this class with ScreenLayoutManager for screen resize event
		ScreenLayoutManager.Instance.register(this);
		
		renderer.sharedMaterial.mainTexture = texture;
		renderer.sharedMaterial.mainTexture.wrapMode = wrapMode;
	}
	
	void Start () {
		if (!texture)
			return;
		updateForGUI(); // make this game object to be located correctly in viewport
	}

	void OnDestroy () {
		ScreenLayoutManager.Instance.remove(this);
#if UNITY_EDITOR
		// this is in case this script is used in editor mode
		if (!texture)
			renderer.sharedMaterial.mainTexture = texture;
#endif
	}

#if UNITY_EDITOR
	void Update () {
		// if in editor mode we change the texture this will update the material
		if (texture != null && !texture.name.Equals(renderer.sharedMaterial.mainTexture.name))
			renderer.sharedMaterial.mainTexture = texture;
		
		// only in Editor Mode: update in case any change from Inspector
		if (!Application.isPlaying)
			updateForGUI();
	}
#endif
	
	public void updateForGUI () {
		ScreenLayoutManager.worldToScreenForGUI(transform, size, sizeAsPixels);
	}
}
