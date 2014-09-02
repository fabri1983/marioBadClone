using UnityEngine;

[ExecuteInEditMode]
public class ScreenLayout : MonoBehaviour, IScreenLayout {
	
	public bool allowResize = false;
	public Vector2 offset = Vector2.zero;
	public EnumScreenLayout layout = EnumScreenLayout.BOTTOM_LEFT;
	
	private GUICustomElement guiElement; // in case you are using the game object as a GUICustomElement instead as of a GUITexture
	
	void Awake () {
		// if using with a GUITexture then no GUICustomElement musn't be found
		guiElement = GetComponent<GUICustomElement>();
		
		ScreenLayoutManager.Instance.register(this);
		updateSizeAndPosition();
	}
	
	void OnDestroy () {
		ScreenLayoutManager.Instance.remove(this);
	}
	
	public void updateSizeAndPosition() {
		// first resize
		if (allowResize)
			ScreenLayoutManager.adjustSize(guiTexture);
		// then apply position correction
		if (guiTexture != null)
			ScreenLayoutManager.adjustPos(guiTexture, offset, layout);
		else if (guiElement != null)
			ScreenLayoutManager.adjustPos(transform, guiElement, offset, layout);
	}

#if UNITY_EDITOR
	// only for Editor mode so any change in offset or enum layout is applied in real time
	void Update () {
		if (Application.isPlaying)
			return;
		updateSizeAndPosition();
	}
#endif
}
