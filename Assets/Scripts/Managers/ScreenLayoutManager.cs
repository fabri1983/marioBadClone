using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
public class ScreenLayoutManager : MonoBehaviour {
	
	private List<ScreenLayout> consumers = new List<ScreenLayout>();
	private float lastScreenWidth, lastScreenHeight;
	
	private static ScreenLayoutManager instance = null;
	
	public static ScreenLayoutManager Instance {
        get {
            if (instance == null) {
				instance = (ScreenLayoutManager)FindObjectOfType(typeof(ScreenLayoutManager));
				if (instance == null)
					// creates a game object with this script component
					instance = new GameObject("ScreenLayoutManager").AddComponent<ScreenLayoutManager>();
				DontDestroyOnLoad(instance);
			}
            return instance;
        }
    }
	
	void Awake () {		
		lastScreenWidth = Screen.width;
		lastScreenHeight = Screen.height;
	}
	
	void OnApplicationQuit() {
		consumers.Clear();
#if UNITY_EDITOR
#else
		instance = null;
#endif
    }
	
	public void register (ScreenLayout sl) {
		consumers.Add(sl);
	}
	
	public void remove (ScreenLayout sl) {
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
}
