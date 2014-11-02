using UnityEngine;

[ExecuteInEditMode]
public class GUIScreenLayout : MonoBehaviour, IScreenLayout {
	
	public bool allowResize = false;
	public Vector2 offset = Vector2.zero;
	public EnumScreenLayout layout = EnumScreenLayout.BOTTOM_LEFT;
	
	private GUICustomElement guiElem; // in case you are using the game object as a GUICustomElement instead as of a GUITexture
	
	void Awake () {
		// if using with a GUITexture then no GUICustomElement musn't be found
		guiElem = GetComponent<GUICustomElement>();
		
		// register this class with ScreenLayoutManager for screen resize event
		GUIScreenLayoutManager.Instance.register(this);
		updateForGUI();
	}
	
	void Start () {
		// the updateForGUI() method was moved to Awake so it does't interfieres with TransitionGUIFX
	}
	
	void OnDestroy () {
		GUIScreenLayoutManager.Instance.remove(this);
	}
	
	public void updateForGUI () {
		// only if we are still using Unity's gui elements
		if (guiTexture != null) {
			// first resize
			if (allowResize)
				GUIScreenLayoutManager.adjustSize(guiTexture);
			// then apply position correction
			GUIScreenLayoutManager.adjustPos(guiTexture, offset, layout);
		}
		else if (guiElem != null) {
			// first resize
			if (allowResize)
				GUIScreenLayoutManager.adjustSize(guiElem);
			// then apply position correction
			GUIScreenLayoutManager.adjustPos(transform, guiElem, offset, layout);
		}
	}

#if UNITY_EDITOR
	void Update () {
		// if not playing from inside the editor: any change in offset or enum layout is applied in real time
		if (!Application.isPlaying)
			updateForGUI();
	}
#endif
}
