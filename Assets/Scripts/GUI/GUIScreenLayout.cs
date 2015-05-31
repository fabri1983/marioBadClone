using UnityEngine;

/// <summary>
/// Allows a custom GUI element to be located in any place of the screen given a predefined locations as basis
/// </summary>.
[ExecuteInEditMode]
public class GUIScreenLayout : MonoBehaviour, IGUIScreenLayout {

	public Vector2 offset = Vector2.zero;
	public bool xAsPixels = false;
	public bool yAsPixels = false;
	public EnumScreenLayout layout = EnumScreenLayout.BOTTOM_LEFT;
	
	private GUICustomElement guiElem; // in case you are using the game object as a GUICustomElement instead as of a GUITexture
	private Vector2 offsetInPixels; // used to store temporal calculation of offset member as a percentage or pixel-wise of the screen

	void Awake () {
		// if using with a GUITexture then no GUICustomElement musn't be found
		guiElem = GetComponent<GUICustomElement>();
		
		// register this class with ScreenLayoutManager for screen resize event
		GUIScreenLayoutManager.Instance.register(this as IGUIScreenLayout);
	}
	
	void OnDestroy () {
#if UNITY_EDITOR
		// Note: scripts with [ExecuteInEditMode] should not call managers that also runs in editor mode.
		// That is to avoid a NullPointerException since the managers instance has been destroyed just before entering Play mode
#else
		GUIScreenLayoutManager.Instance.remove(this as IGUIScreenLayout);
#endif
	}

#if UNITY_EDITOR
	void Update () {
		// when not in Editor Play Mode: any change in offset or enum layout is applied in real time
		if (!Application.isPlaying)
			updateForGUI();
	}
#endif

	public void updateForGUI () {
		// if using offset as proportion then convert it as pixels
		if (!xAsPixels)
			offsetInPixels.x = offset.x * Screen.width;
		else
			offsetInPixels.x = offset.x;
		
		// if using offset as propportion then convert it as pixels
		if (!yAsPixels)
			offsetInPixels.y = offset.y * Screen.height;
		else
			offsetInPixels.y = offset.y;

		// then apply position correction
		GUIScreenLayoutManager.adjustPos(transform, guiElem, offsetInPixels, layout);
	}
}
