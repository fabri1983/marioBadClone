using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
public class GUIScreenLayoutManager : MonoBehaviour {
	
	private List<IScreenLayout> listeners = new List<IScreenLayout>();
	private float lastScreenWidth, lastScreenHeight;
	
	private const float GUI_NEAR_PLANE_OFFSET = 0.01f;
	private const float DEG_2_RAD_0_5 = Mathf.Deg2Rad * 0.5f;
	
	private static GUIScreenLayoutManager instance = null;

	public static GUIScreenLayoutManager Instance {
        get {
            if (instance == null) {
				instance = FindObjectOfType(typeof(GUIScreenLayoutManager)) as GUIScreenLayoutManager;
				if (instance == null) {
					// creates a game object with this script component
					instance = new GameObject("ScreenLayoutManager").AddComponent<GUIScreenLayoutManager>();
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
		instance = null;
    }
	
	public void register (IScreenLayout sl) {
		listeners.Add(sl);
	}
	
	public void remove (IScreenLayout sl) {
		listeners.Remove(sl);
	}
	
	void OnGUI () {
		if (EventType.Repaint == Event.current.type) {
			// if screen is resized then need to notice all listeners
			if (Screen.width != lastScreenWidth || Screen.height != lastScreenHeight) {
				// notify to all listeners
				for (int i=0, c=listeners.Count; i<c; ++i)
					listeners[i].updateForGUI();
				// update screen dimension
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
	/// Transform
	/// </param>
	/// <param name='guiElem'>
	/// Is the GUI element containing the info about size in pixels or as a proportion
	/// </param>
	/// <param name='offset'>
	/// Offset.
	/// </param>
	/// <param name='layout'>
	/// Layout.
	/// </param>
	public static void adjustPos (Transform tr, GUICustomElement guiElem, Vector2 offset, EnumScreenLayout layout)
	{
		Vector3 p = tr.localPosition;
		
		// assume the GUI element has its size as pixels
		Vector2 tex = guiElem.size;
		// is GUI element size set as a proportion?
		if (!guiElem.sizeAsPixels) {
			tex.x = Screen.width * guiElem.size.x;
			tex.y = Screen.height * guiElem.size.y;
		}
		
		switch (layout) {
		case EnumScreenLayout.TOP_LEFT: {
			Vector2 temp = screenToGUI(offset.x + tex.x/2f, Screen.height - tex.y/2f + offset.y);
			p.x = temp.x;
			p.y = temp.y;
			break; }
		case EnumScreenLayout.TOP: {
			Vector2 temp = screenToGUI(Screen.width/2 + offset.x, Screen.height - tex.y/2f + offset.y);
			p.x = temp.x;
			p.y = temp.y;
			break; }
		case EnumScreenLayout.TOP_RIGHT: {
			Vector2 temp = screenToGUI(Screen.width + offset.x - tex.x/2f, Screen.height - tex.y/2f + offset.y);
			p.x = temp.x;
			p.y = temp.y;
			break; }
		case EnumScreenLayout.CENTER_LEFT: {
			Vector2 temp = screenToGUI(offset.x + tex.x/2f, Screen.height/2 + offset.y);
			p.x = temp.x;
			p.y = temp.y;
			break; }
		case EnumScreenLayout.CENTER: {
			p.x = Screen.width/2 - tex.x/2 + offset.x;
			p.y = Screen.height/2 - tex.y/2 + offset.y;
			Vector2 temp = screenToGUI(Screen.width/2 + offset.x, Screen.height/2 + offset.y);
			p.x = temp.x;
			p.y = temp.y;
			break; }
		case EnumScreenLayout.CENTER_RIGHT: {
			Vector2 temp = screenToGUI(Screen.width + offset.x - tex.x/2f, Screen.height/2 + offset.y);
			p.x = temp.x;
			p.y = temp.y;
			break; }
		case EnumScreenLayout.BOTTOM_LEFT: {
			Vector2 temp = screenToGUI(offset.x + tex.x/2f, tex.y/2f + offset.y);
			p.x = temp.x;
			p.y = temp.y;
			break; }
		case EnumScreenLayout.BOTTOM: {
			Vector2 temp = screenToGUI(Screen.width/2 + offset.x, tex.y/2f + offset.y);
			p.x = temp.x;
			p.y = temp.y;
			break; }
		case EnumScreenLayout.BOTTOM_RIGHT: {
			Vector2 temp = screenToGUI(Screen.width + offset.x - tex.x/2f, tex.y/2f + offset.y);
			p.x = temp.x;
			p.y = temp.y;
			break; }
		default: break;
		}
		
		tr.localPosition = p;
	}
	
	public static void adjustSize (GUITexture gt) {
		
	}
	
	public static void adjustSize (GUICustomElement gui) {
		
	}
	
	/// <summary>
	/// Gets the z-pos and (w,h) scale factors for a custom GUI positioning in screen. 
	/// The game object which wants to be transformed as a GUI element needs to be positioned to the Z xoordinate 
	/// and be scaled as this method return values specify.
	/// NOTE: the virtual plane for locating the game object is centered on screen and located in z = nearClipPlane + 0.01f.
	/// It's a virtual box with coordinates (-w/2, -h/2) to (w/2, h/2).
	/// </summary>
	/// <returns>
	/// Vector3. x,y = Width,Height factors. z = transform position for that axis.
	/// </returns>
	private static Vector3 getZLocationAndScaleForGUI ()
	{
		Vector3 result;
		
		// position barely ahead from near clip plane
		float posAhead = (Camera.main.nearClipPlane + GUI_NEAR_PLANE_OFFSET);
		// consider current cameras's facing direction (it can be rotated)
		// we're only interesting in z coordinate
		result.z = (posAhead * Camera.main.transform.forward + Camera.main.transform.position).z;

		// calculate Width and Height of our virtual plane z-positioned in nearClipPlane + 0.01f
		float h, w;
		if (Camera.main.orthographic) {
			h = Camera.main.orthographicSize * 2f;
			w = h / Screen.height * Screen.width;
		}
		else {
			h = 2f * Mathf.Tan(Camera.main.fov * DEG_2_RAD_0_5) * posAhead;
			w = h * Camera.main.aspect;
		}
		
		// keep z untouch (z-position of the GUI element), set width and height
		result.x = w;
		result.y = h;
		return result;
	}
	
	/// <summary>
	/// Converts pixel (x,y) coordinates from screen to custom GUI space.
	/// </summary>
	/// <returns>
	/// The x,y pixel coordinates where a transform component is located to as a GUI element.
	/// </returns>
	/// <param name='pixelX'>
	/// Pixel x coordinate.
	/// </param>
	/// <param name='pixelY'>
	/// Pixel y coordinate.
	/// </param>
	public static Vector2 screenToGUI (float pixelX, float pixelY)
	{
		Vector3 guiZposAndDimension = getZLocationAndScaleForGUI();
		Vector2 result;
		float guiW = guiZposAndDimension.x;
		float guiH = guiZposAndDimension.y;
		
		// maps pixel coordinate to a box with coordinates (-w/2, -h/2) to (w/2, h/2). This is the custom GUI viewport
		result.x = guiW * (pixelX / Screen.width - 0.5f);
		result.y = guiH * (pixelY / Screen.height - 0.5f);

		return result;
	}
	
	/// <summary>
	/// Maps a GUI (x,y) coordinate to a (x,y) screen pixel coordinate.
	/// </summary>
	/// <returns>
	/// Vector2. Screen pixel X,Y
	/// </returns>
	/// <param name='guiX'>
	/// GUI x. X Coordinate in GUI space
	/// </param>
	/// <param name='guiY'>
	/// GUI y. Y Coordinate in GUI space
	/// </param>
	public static Vector2 guiToScreen (float guiX, float guiY)
	{
		Vector3 guiZposAndDimension = getZLocationAndScaleForGUI();
		Vector2 result;
		float guiW = guiZposAndDimension.x;
		float guiH = guiZposAndDimension.y;
		
		// maps GUI coordinate to screen pixels
		result.x = (guiX/guiW + 0.5f) * Screen.width;
		result.y = (guiY/guiH + 0.5f) * Screen.height;
		
		return result;
	}
	
	public static Vector2 sizeInGUI (Vector2 size, bool isInPixels)
	{
		// get z position and GUI space coordinates. The GUI coordinates act as scale factors
		Vector3 guiZposAndDimension = getZLocationAndScaleForGUI();
		// imagine the size in pixels as a rect with min = (0,0) and max = size (in pixels)
		// and convert it to GUI space
		Vector2 result = screenToGUI(isInPixels? size.x : size.x * (float)Screen.width, isInPixels? size.y : size.y * (float)Screen.height);
		// the GUI space is a box with its (0,0) centered in screen, so we need to correct that
		result.x += guiZposAndDimension.x / 2f;
		result.y += guiZposAndDimension.y / 2f;
		
		return result;
	}
	
	/// <summary>
	/// Sets z-position and scale (w,h) to a transform object for positioning in front of 
	/// camera to act like a GUI element.
	/// The size parameter scales even more the transform to satisfy a desired size of the transform.
	/// NOTE: transform will be centered on screen.
	/// </summary>
	/// <param name='tr'>
	/// Transform of the game object to be modified for being a GUI element
	/// </param>
	/// <param name='sizeInPixels'>
	/// Size the texture covers in screen
	/// </param>
	public static void locateForGUI (Transform tr, Vector2 sizeInPixels)
	{
		// get z position and GUI space coordinates. The GUI coordinates act as scale factors
		Vector3 guiZposAndDimension = getZLocationAndScaleForGUI();
		
		Vector3 thePos = tr.position;
		thePos.z = guiZposAndDimension.z;
		tr.position = thePos;
		
		// apply scale to locate in GUI space. Consider user defined size
		Vector3 theScale = tr.localScale;
		theScale.x = guiZposAndDimension.x * sizeInPixels.x / Screen.width;
		theScale.y = guiZposAndDimension.y * sizeInPixels.y / Screen.height;
		theScale.z = 0f;
		tr.localScale = theScale;
		
		// modify local position (not world position)
		/*Vector3 theLocalPos = tr.localPosition;
		// x position doesn't work yet
		theLocalPos.y = guiZposAndDimension.y * 0.5f * (1f - Mathf.Abs(sizeInPixels.y)) * Mathf.Sign(sizeInPixels.y);
		tr.localPosition = theLocalPos;*/
	}
}
