#if !(UNITY_3_0 || UNITY_3_0_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5)
#define UNITY_4_AND_LATER
#endif
using UnityEngine;

/// <summary>
/// Scales the game object according to screen size and user defined coverage to act as a GUI element.
/// On screen redimension the game object's Z position and scale are adjusted.
/// </summary>
[ExecuteInEditMode]
public class GUICustomElement : MonoBehaviour, IGUIScreenLayout {

	public Texture2D texture;
	public TextureWrapMode wrapMode = TextureWrapMode.Repeat;
	public Vector2 size = Vector2.one; // pixels or screen proportion
	public Vector2 virtualSize = Vector2.zero; // this emulates a GUI size for correct screen location in situations where elem position is relative to another one
	public bool sizeAsPixels = false;
	public bool newMaterialInstance = false; // currently no atlas usage, so every game object instance has its own material instance

	private Vector2 casheSizeInGUI = -1f * Vector2.one;

	void Awake () {
		// deactivate the game object if no texture
		if (!texture) {
#if UNITY_4_AND_LATER
			gameObject.SetActive(false);
#else
			gameObject.SetActiveRecursively(false);
#endif
			return;
		}
		else {
#if UNITY_4_AND_LATER
			gameObject.SetActive(true);
#else
			gameObject.SetActiveRecursively(true);
#endif
		}

		// register this class with ScreenLayoutManager for screen resize event
		GUIScreenLayoutManager.Instance.register(this as IGUIScreenLayout);
		
		if (newMaterialInstance) {
            // create the new material and assing it to the renderer
			if (renderer.sharedMaterial == null) {
				renderer.sharedMaterial = new Material(renderer.material);
				Debug.LogWarning("No material. A Default one will is created. Verify your prefab or gameobject config. " + gameObject.name);
			}
			else
            	renderer.sharedMaterial = new Material(renderer.sharedMaterial);
        }
		
		renderer.sharedMaterial.mainTexture = texture;
		renderer.sharedMaterial.mainTexture.wrapMode = wrapMode;
	}
	
	/*void Start () {
		if (!texture)
			return;
		updateForGUI(); // make this game object to be located correctly in viewport
	}*/

	void OnDestroy () {
#if UNITY_EDITOR
		// this is in case this script is used in editor mode
		if (!texture)
			renderer.sharedMaterial.mainTexture = texture;
		
		// Note: scripts with [ExecuteInEditMode] should not call managers that also runs in editor mode.
		// That is to avoid a NullPointerException since the managers instance has been destroyed just before entering Play mode
		
		// only call the manager if is playing
		if (Application.isPlaying)
			GUIScreenLayoutManager.Instance.remove(this as IGUIScreenLayout);
#else
		GUIScreenLayoutManager.Instance.remove(this as IGUIScreenLayout);
#endif
	}

#if UNITY_EDITOR
	void Update () {
		// in case we change the texture this will update the material
		if (texture != null && renderer.sharedMaterial != null && !texture.name.Equals(renderer.sharedMaterial.mainTexture.name))
			renderer.sharedMaterial.mainTexture = texture;
		
		// when not in Editor Play Mode: update in case any change happens from Inspector or IDE
		if (!Application.isPlaying)
			updateForGUI();
	}
#endif
	
	public void updateForGUI () {
		// reset the cache since maybe the sice in GUI will be used ahead
		casheSizeInGUI.x = -1f;
		// locate GUI element according new screen size (or whatever event fire this method)
		GUIScreenLayoutManager.locateForGUI(transform, getSizeInPixels());
	}
	
	/// <summary>
	/// Gets the size in GUI space.
	/// </summary>
	/// <returns>
	/// The size in GUI space
	/// </returns>
	public Vector2 getSizeInGUI () {
		// if size was not caluclated yet then proceed and cache it
		if (casheSizeInGUI.x == -1f)
			casheSizeInGUI = GUIScreenLayoutManager.sizeInGUI(getSizeInPixels());
		return casheSizeInGUI;
	}
	
	/// <summary>
	/// Gets the size in pixels this element covers at screen. 
	/// It internally converts from proportion to pixels if needed.
	/// </summary>
	/// <returns>The size in pixels.</returns>
	public Vector2 getSizeInPixels () {
		Vector2 result;
		result.x = sizeAsPixels? size.x : Screen.width * size.x;
		result.y = sizeAsPixels? size.y : Screen.height * size.y;
		return result;
	}
}
