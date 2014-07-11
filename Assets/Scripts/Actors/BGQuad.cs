using UnityEngine;

[ExecuteInEditMode]
public class BGQuad : MonoBehaviour, IScreenLayout {
	
	public Texture bgTexture;
	
	void Awake () {
		ScreenLayoutManager.Instance.register(this);
	}
	
	void Start () {
		fillScreen(); // set the quad to fill the viewport
		renderer.sharedMaterial.mainTexture = bgTexture;
	}
	
	private void fillScreen () {
		Camera cam = Camera.main;
		float pos = (cam.nearClipPlane + 0.01f);
		transform.position = cam.transform.position + cam.transform.forward * pos;
		float h = Mathf.Tan(cam.fov*Mathf.Deg2Rad*0.5f)*pos*2f;
		transform.localScale = new Vector3(h*cam.aspect,h,0f);
	}
	
	public void updateSizeAndPosition () {
		fillScreen();
	}
}
