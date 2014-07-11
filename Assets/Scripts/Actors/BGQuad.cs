using UnityEngine;

[ExecuteInEditMode]
public class BGQuad : MonoBehaviour, IScreenLayout {
	
	public Texture bgTexture;
	
	void Awake () {
		if (!bgTexture) {
			gameObject.SetActiveRecursively(false);
			return;
		}
		ScreenLayoutManager.Instance.register(this);
	}
	
	void Start () {
		if (!bgTexture)
			return;
		// set the quad to fill the viewport
		fillScreen();
		renderer.sharedMaterial.mainTexture = bgTexture;
	}

#if UNITY_EDITOR
	void OnDestroy () {
		if (bgTexture)
			renderer.sharedMaterial.mainTexture = bgTexture;
	}
#endif
	
	private void fillScreen () {
		Camera cam = Camera.main;
		float pos = (cam.nearClipPlane + 0.01f);
		transform.position = cam.transform.position + cam.transform.forward * pos;
		
		if(!cam.orthographic) {
			float h = Mathf.Tan(cam.fov * Mathf.Deg2Rad * 0.5f) * pos * 2f;
			float w = h * cam.aspect;
			Vector3 theScale = transform.localScale;
			theScale.x = w;
			theScale.y = h;
			theScale.z = 0f;
			transform.localScale = theScale;
		}
		else {
			float h = cam.orthographicSize * 2f;
			float w = h / Screen.height * Screen.width;
			Vector3 theScale = transform.localScale;
			theScale.x = w;
			theScale.y = h;
			theScale.z = 0f;
			transform.localScale = theScale;
		}
	}
	
	public void updateSizeAndPosition () {
		fillScreen();
	}
}
