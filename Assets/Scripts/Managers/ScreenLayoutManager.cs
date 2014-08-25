using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
public class ScreenLayoutManager : MonoBehaviour {
	
	private List<IScreenLayout> consumers = new List<IScreenLayout>();
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
		consumers.Clear();
		instance = null;
    }
	
	public void register (IScreenLayout sl) {
		consumers.Add(sl);
	}
	
	public void remove (IScreenLayout sl) {
		consumers.Remove(sl);
	}
	
	void OnGUI () {
		// if screen is resized then need to change background texture size
		if (EventType.Repaint == Event.current.type) {
			if (Screen.width != lastScreenWidth || Screen.height != lastScreenHeight) {
				for (int i=0, c=consumers.Count; i<c; ++i)
					consumers[i].updateSizeAndPosition();
				lastScreenWidth = Screen.width;
				lastScreenHeight = Screen.height;
			}
		}
	}
	
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
	
	public static void adjustPos (Transform tr, Texture tex, Vector2 offset, EnumScreenLayout layout) {
		Vector3 p = tr.localPosition;
		
		switch (layout) {
		case EnumScreenLayout.TOP_LEFT: {
			p.x = offset.x;
			p.y = Screen.height - tex.height + offset.y;
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
