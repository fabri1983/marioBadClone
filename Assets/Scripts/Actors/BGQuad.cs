using UnityEngine;

[ExecuteInEditMode]
public class BGQuad : MonoBehaviour, IScreenLayout {
	
	public Texture bgTexture;
	
	private float pos, height;
	
	void Awake () {
		ScreenLayoutManager.Instance.register(this);
	}
	
	void Start () {
		Camera cam = Camera.main;
		pos = (cam.nearClipPlane + 0.01f);
		fillScreen(); // set the quad to fill the viewport
		renderer.sharedMaterial.mainTexture = bgTexture;
	}
	
	private void fillScreen () {
		Camera cam = Camera.main;
		transform.position = cam.transform.position + cam.transform.forward * pos;
		float h = Mathf.Tan(cam.fov*Mathf.Deg2Rad*0.5f)*pos*2f;
		transform.localScale = new Vector3(h*cam.aspect,h,0f);
	}
	
	public void updateSizeAndPosition () {
		fillScreen();
	}
}
