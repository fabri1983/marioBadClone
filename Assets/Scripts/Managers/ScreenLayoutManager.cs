using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
public class ScreenLayoutManager : MonoBehaviour {
	
	private List<IScreenLayout> listeners = new List<IScreenLayout>();
	private float lastScreenWidth, lastScreenHeight;
	
	private static ScreenLayoutManager instance = null;

	public static ScreenLayoutManager Instance {
        get {
            if (instance == null) {
				instance = FindObjectOfType(typeof(ScreenLayoutManager)) as ScreenLayoutManager;
				if (instance == null) {
					// creates a game object with this script component
					instance = new GameObject("ScreenLayoutManager").AddComponent<ScreenLayoutManager>();
				}
			}
            return instance;
        }
    }
	
	void Awake () {
		if (instance != null && instance != this)
#if UNITY_EDITOR
			DestroyImmediate(this.gameObject);
#else
			Destroy(this.gameObject);
#endif
		else {
			instance = this;
			DontDestroyOnLoad(gameObject);
		}
		
		lastScreenWidth = Screen.width;
		lastScreenHeight = Screen.height;
	}

	void OnDestroy() {
		listeners.Clear();
#if UNITY_EDITOR
#else
		instance = null;
#endif
    }
	
	public void register (IScreenLayout sl) {
		listeners.Add(sl);
	}
	
	public void remove (IScreenLayout sl) {
		listeners.Remove(sl);
	}
	
	void OnGUI () {
		// if screen is resized then need to notice all listeners
		if (EventType.Repaint == Event.current.type) {
			if (Screen.width != lastScreenWidth || Screen.height != lastScreenHeight) {
				for (int i=0, c=listeners.Count; i<c; ++i)
					listeners[i].updateSizeAndPosition();
				lastScreenWidth = Screen.width;
				lastScreenHeight = Screen.height;
			}
		}
	}
	
	/// <summary>
	/// Locates the GUITexture element in the screen according the layout value.
	/// It modifies the GUITexture's pixel inset.
	/// </summary>
	/// <param name='gt'>
	/// Gt. Unity'sGUITexture
	/// </param>
	/// <param name='offset'>
	/// Offset
	/// </param>
	/// <param name='layout'>
	/// Layout
	/// </param>
	public static void adjustPos (GUITexture gt, Vector2 offset, EnumScreenLayout layout) {
		Rect p = gt.pixelInset;
		
		switch (layout) {
		case EnumScreenLayout.TOP_LEFT: {
			p.x = offset.x;
			p.y = Screen.height - p.height + offset.y;
			break; }
		case EnumScreenLayout.TOP: {
			p.x = Screen.width/2 - p.width/2 + offset.x;
			p.y = Screen.height - p.height + offset.y;
			break; }
		case EnumScreenLayout.TOP_RIGHT: {
			p.x = Screen.width - p.width + offset.x;
			p.y = Screen.height - p.height + offset.y;
			break; }
		case EnumScreenLayout.CENTER_LEFT: {
			p.x = offset.x;
			p.y = Screen.height/2 - p.height/2 + offset.y;
			break; }
		case EnumScreenLayout.CENTER: {
			p.x = Screen.width/2 - p.width/2 + offset.x;
			p.y = Screen.height/2 - p.height/2 + offset.y;
			break; }
		case EnumScreenLayout.CENTER_RIGHT: {
			p.x = Screen.width - p.width + offset.x;
			p.y = Screen.height/2 - p.height/2 + offset.y;
			break; }
		case EnumScreenLayout.BOTTOM_LEFT: {
			p.x = offset.x;
			p.y = offset.y;
			break; }
		case EnumScreenLayout.BOTTOM: {
			p.x = Screen.width/2 - p.width + offset.x;
			p.y = offset.y;
			break; }
		case EnumScreenLayout.BOTTOM_RIGHT: {
			p.x = Screen.width - p.width + offset.x;
			p.y = offset.y;
			break; }
		default: break;
		}
		
		gt.pixelInset = p;
	}
	
	/// <summary>
	/// Locates the transform element in the screen according the layout value.
	/// It depends on the way the transform originally was z-located and xy-scaled to act as a GUI element.
	/// </summary>
	/// <param name='tr'>
	/// Tr. Transform
	/// </param>
	/// <param name='tex'>
	/// Tex. Texture used to consider actual size
	/// </param>
	/// <param name='offset'>
	/// Offset.
	/// </param>
	/// <param name='layout'>
	/// Layout.
	/// </param>
	public static void adjustPos (Transform tr, Texture tex, Vector2 offset, EnumScreenLayout layout)
	{
		Vector3 p = tr.localPosition;
		Vector3 temp;
		// gets z-position and width and height for a GUI element
		Vector3 guiPosAndDim = GameObjectTools.getZLocationAndDimensionForGUI(Camera.main);
		
		switch (layout) {
		case EnumScreenLayout.TOP_LEFT: {
			temp.x = offset.x + tex.width/2;
			temp.y = Screen.height - tex.height + offset.y;
			temp.z = p.z;
			temp = Camera.main.ScreenToWorldPoint(temp);
			p.x = temp.x;
			p.y = temp.y;
			break; }
		case EnumScreenLayout.TOP: {
			p.x = Screen.width/2 - tex.width/2 + offset.x;
			p.y = Screen.height - tex.height + offset.y;
			break; }
		case EnumScreenLayout.TOP_RIGHT: {
			p.x = Screen.width - tex.width + offset.x;
			p.y = Screen.height - tex.height + offset.y;
			break; }
		case EnumScreenLayout.CENTER_LEFT: {
			p.x = offset.x;
			p.y = Screen.height/2 - tex.height/2 + offset.y;
			break; }
		case EnumScreenLayout.CENTER: {
			p.x = Screen.width/2 - tex.width/2 + offset.x;
			p.y = Screen.height/2 - tex.height/2 + offset.y;
			break; }
		case EnumScreenLayout.CENTER_RIGHT: {
			p.x = Screen.width - tex.width + offset.x;
			p.y = Screen.height/2 - tex.height/2 + offset.y;
			break; }
		case EnumScreenLayout.BOTTOM_LEFT: {
			p.x = offset.x;
			p.y = offset.y;
			break; }
		case EnumScreenLayout.BOTTOM: {
			p.x = Screen.width/2 - tex.width + offset.x;
			p.y = offset.y;
			break; }
		case EnumScreenLayout.BOTTOM_RIGHT: {
			p.x = Screen.width - tex.width + offset.x;
			p.y = offset.y;
			break; }
		default: break;
		}
		
		tr.localPosition = p;
	}
	
	public static void adjustSize (GUITexture gt) {
		/*float lastScreenWidth = Instance.lastScreenWidth;
		float lastScreenHeight = Instance.lastScreenHeight;
		float textureHeight = gt.pixelInset.height;
		float textureWidth = gt.pixelInset.width;
		float screenHeight = Screen.height;
		float screenWidth = Screen.width;
		
		// NOTE: only works when size increases
		float screenAspectRatio = screenHeight / screenWidth;
		float wChange = (screenWidth - lastScreenWidth) / screenWidth;
		float hChange = (screenHeight - lastScreenHeight) / screenHeight;
		int scaledWidth, scaledHeight;
		if (screenAspectRatio <= 1f) {
			float factor = screenAspectRatio * Mathf.Max(Mathf.Abs(wChange), Mathf.Abs(hChange));
			scaledWidth = (int)(textureWidth * (1f + Mathf.Sign(wChange)*factor));
			scaledHeight = (int)(textureHeight * (1f + Mathf.Sign(hChange)*factor));
		}
		else {
			scaledWidth = (int)(textureWidth / screenAspectRatio);
			scaledHeight = (int)(textureHeight / screenAspectRatio);
		}
		
		Rect p = gt.pixelInset;
		p.width = scaledWidth;
		p.height = scaledHeight;
		gt.pixelInset = p;*/
	}
}
