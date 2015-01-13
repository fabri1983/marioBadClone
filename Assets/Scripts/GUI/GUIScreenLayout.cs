using UnityEngine;

/// <summary>
/// Allows a custom GUI element to be located in any place of the screen given a predefined locations as basis
/// </summary>.
[ExecuteInEditMode]
public class GUIScreenLayout : MonoBehaviour, IGUIScreenLayout {

	public Vector2 offset = Vector2.zero;
	public bool asProportion = false;
	public EnumScreenLayout layout = EnumScreenLayout.BOTTOM_LEFT;
	
	private GUICustomElement guiElem; // in case you are using the game object as a GUICustomElement instead as of a GUITexture
	private Vector2 offsetCalculation; // used to store temporal calculation of offset member as a percentage or pixel-wise of the screen

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

		// if using offset as percentage then convert it as pixels
		if (asProportion) {
			offsetCalculation.x = offset.x * Screen.width;
			offsetCalculation.y = offset.y * Screen.height;
		}
		else {
			offsetCalculation.x = offset.x;
			offsetCalculation.y = offset.y;
		}

		// only if we are still using Unity's GUITexture elements
		if (guiTexture != null) {
			// first resize
			if (guiElem.allowResize)
				GUIScreenLayoutManager.adjustSize(guiTexture);
			// then apply position correction
			GUIScreenLayoutManager.adjustPos(guiTexture, offsetCalculation, layout);
		}
		// we are using GUICustomElement
		else if (guiElem != null) {
			// first resize
			if (guiElem.allowResize)
				GUIScreenLayoutManager.adjustSize(guiElem);
			// then apply position correction
			GUIScreenLayoutManager.adjustPos(transform, guiElem, offsetCalculation, layout);
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
