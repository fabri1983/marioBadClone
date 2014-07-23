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
		fillScreen(); // set the quad to fill the viewport
		renderer.sharedMaterial.mainTexture = bgTexture;
	}

#if UNITY_EDITOR
	void OnDestroy () {
		if (bgTexture)
			renderer.sharedMaterial.mainTexture = bgTexture;
	}
#endif
	
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
}
