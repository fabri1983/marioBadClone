using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manager singleton which sends update events for GUI elements.
/// Currently only when an repaint event is fired by Unity.
/// It also provides a lot of utils for locating and resizing of GUI custom elements.
/// </summary>
[ExecuteInEditMode]
public class GUIScreenLayoutManager : MonoBehaviour {

	public static Vector2 MIN_RESOLUTION = new Vector2(480f, 320f);
	public static Matrix4x4 unityGUIMatrix = Matrix4x4.identity; // initialized with identity to allow earlier calls of OnGUI() that use this matrix

	private List<IGUIScreenLayout> listeners = new List<IGUIScreenLayout>();
	[SerializeField] private float lastScreenWidth;
	[SerializeField] private float lastScreenHeight;
	private const float GUI_NEAR_PLANE_OFFSET = 0.01f;
	private const float DEG_2_RAD_0_5 = Mathf.Deg2Rad * 0.5f;
	
	private static Vector3 zero = Vector3.zero;
	private static Quaternion identity = Quaternion.identity;
	private static Vector3 scale = Vector3.zero;
	
	private static GUIScreenLayoutManager instance = null;
	private static bool duplicated = false; // usefull to avoid onDestroy() execution on duplicated instances being destroyed
	
	public static GUIScreenLayoutManager Instance {
        get {
            if (instance == null) {
				// creates a game object with this script component
				instance = new GameObject("GUIScreenLayoutManager").AddComponent<GUIScreenLayoutManager>();
			}
            return instance;
        }
    }
	
	void Awake () {
		if (instance != null && instance != this) {
			duplicated = true;
			#if UNITY_EDITOR
			DestroyImmediate(this.gameObject);
			#else
			Destroy(this.gameObject);
			#endif
		}
		else {
			instance = this;
			DontDestroyOnLoad(gameObject);
			initialize();
		}
	}
	
	private void initialize () {
		// NOTE: set as current resolution so the first OnGUI event isn't a resized event. 
		// That would cause a callback to all subscribers before correct initialization of them
		lastScreenWidth = Screen.width;
		lastScreenHeight = Screen.height;
		
		// update the GUI matrix used by several Unity GUI elements as a shortcut for GUI elems resizing
		setupUnityGUIMatrixForResizing();
	}
	
	void OnDestroy() {
		listeners.Clear();
		// this is to avoid nullifying or destroying static variables. Intance variables can be destroyed before this check
		if (duplicated) {
			duplicated = false; // reset the flag for next time
			return;
		}
		instance = null;
    }
	
	public void register (IGUIScreenLayout sl) {
		listeners.Add(sl);
	}
	
	public void remove (IGUIScreenLayout sl) {
		listeners.Remove(sl);
	}

	private void setupUnityGUIMatrixForResizing () {
		float widthRatio = ((float)Screen.width) / MIN_RESOLUTION.x;
		float heightRatio = ((float)Screen.height) / MIN_RESOLUTION.y;
		scale.Set(widthRatio, heightRatio, 1f);
		unityGUIMatrix = Matrix4x4.TRS(zero, identity, scale);
	}

	void OnGUI () {
		if (EventType.Repaint == Event.current.type) {
			// if screen is resized then need to notice all listeners
			if (Screen.width != lastScreenWidth || Screen.height != lastScreenHeight) {

				// update GUI matrix used by several Unity GUI elements as a shortcut for GUI elems resizing
				setupUnityGUIMatrixForResizing();

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
#if UNITY_EDITOR
		if (float.IsNegativeInfinity(p.x) || float.IsInfinity(p.x) || float.IsNaN(p.x))
			p.x = 0f;
		if (float.IsNegativeInfinity(p.y) || float.IsInfinity(p.y) || float.IsNaN(p.y))
			p.y = 0f;
#endif
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
	/// <param name='offsetInPixels'>
	/// Offset measured in pixels.
	/// </param>
	/// <param name='layout'>
	/// Layout.
	/// </param>
	public static void adjustPos (Transform tr, GUICustomElement guiElem, Vector2 offsetInPixels, EnumScreenLayout layout)
	{
		Vector3 p = tr.localPosition;

		// assume the GUI element has its size as pixels
		Vector2 size = guiElem.virtualSize.x != 0 ? guiElem.virtualSize : guiElem.size;
		// convert to pixels if GUI element's size is set as a proportion
		if (!guiElem.sizeAsPixels) {
			size.x *= Screen.width;
			size.y *= Screen.height;
		}
		
		switch (layout) {
		case EnumScreenLayout.TOP_LEFT: {
			Vector2 temp = screenToGUI(offsetInPixels.x + size.x/2f, Screen.height - size.y/2f + offsetInPixels.y);
			p.x = temp.x;
			p.y = temp.y;
			break; }
		case EnumScreenLayout.TOP: {
			Vector2 temp = screenToGUI(Screen.width/2 + offsetInPixels.x, Screen.height - size.y/2f + offsetInPixels.y);
			p.x = temp.x;
			p.y = temp.y;
			break; }
		case EnumScreenLayout.TOP_RIGHT: {
			Vector2 temp = screenToGUI(Screen.width + offsetInPixels.x - size.x/2f, Screen.height - size.y/2f + offsetInPixels.y);
			p.x = temp.x;
			p.y = temp.y;
			break; }
		case EnumScreenLayout.CENTER_LEFT: {
			Vector2 temp = screenToGUI(offsetInPixels.x + size.x/2f, Screen.height/2 + offsetInPixels.y);
			p.x = temp.x;
			p.y = temp.y;
			break; }
		case EnumScreenLayout.CENTER: {
			p.x = Screen.width/2 - size.x/2 + offsetInPixels.x;
			p.y = Screen.height/2 - size.y/2 + offsetInPixels.y;
			Vector2 temp = screenToGUI(Screen.width/2 + offsetInPixels.x, Screen.height/2 + offsetInPixels.y);
			p.x = temp.x;
			p.y = temp.y;
			break; }
		case EnumScreenLayout.CENTER_RIGHT: {
			Vector2 temp = screenToGUI(Screen.width + offsetInPixels.x - size.x/2f, Screen.height/2 + offsetInPixels.y);
			p.x = temp.x;
			p.y = temp.y;
			break; }
		case EnumScreenLayout.BOTTOM_LEFT: {
			Vector2 temp = screenToGUI(offsetInPixels.x + size.x/2f, size.y/2f + offsetInPixels.y);
			p.x = temp.x;
			p.y = temp.y;
			break; }
		case EnumScreenLayout.BOTTOM: {
			Vector2 temp = screenToGUI(Screen.width/2 + offsetInPixels.x, size.y/2f + offsetInPixels.y);
			p.x = temp.x;
			p.y = temp.y;
			break; }
		case EnumScreenLayout.BOTTOM_RIGHT: {
			Vector2 temp = screenToGUI(Screen.width + offsetInPixels.x - size.x/2f, size.y/2f + offsetInPixels.y);
			p.x = temp.x;
			p.y = temp.y;
			break; }
		default: break;
		}
#if UNITY_EDITOR
		if (float.IsNegativeInfinity(p.x) || float.IsInfinity(p.x) || float.IsNaN(p.x))
			p.x = 0f;
		if (float.IsNegativeInfinity(p.y) || float.IsInfinity(p.y) || float.IsNaN(p.y))
			p.y = 0f;
#endif
		tr.localPosition = p;
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
		// NOTE: x and y are overwritten on consequent lines
		result = posAhead * Camera.main.transform.forward + Camera.main.transform.position;

		// calculate Width and Height of our virtual plane z-positioned in nearClipPlane + 0.01f
		// keep z untouch (z-position of the GUI element), set width and height
		if (Camera.main.orthographic) {
			result.y = Camera.main.orthographicSize * 2f;
			result.x = result.y / Screen.height * Screen.width;
		}
		else {
			result.y = 2f * Mathf.Tan(Camera.main.fieldOfView * DEG_2_RAD_0_5) * posAhead;
			result.x = result.y * Camera.main.aspect;
		}

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
	public static Vector2 GUIToScreen (float guiX, float guiY)
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
	
	public static Vector2 sizeInGUI (Vector2 size)
	{
		// get z position and GUI space coordinates. The GUI coordinates act as scale factors
		Vector3 guiZposAndDimension = getZLocationAndScaleForGUI();
		// imagine the size in pixels as a rect with min = (0,0) and max = size
		// and convert it to GUI space
		Vector2 result = screenToGUI(size.x, size.y);
		// the GUI space is a box with its (0,0) centered in screen, so we need to correct that
		result.x += guiZposAndDimension.x / 2f;
		result.y += guiZposAndDimension.y / 2f;
		
		return result;
	}
	
	public static Rect getPositionInScreen (GUICustomElement guiElem)
	{
		Vector3 guiPos = guiElem.transform.localPosition; // GUI custom element uses localPosition for correctly on screen location
		Vector2 sizeInGUI = guiElem.getSizeInGUI();
		Vector2 sizeInPixels = guiElem.getSizeInPixels();
		Vector2 pixelMin = GUIToScreen(guiPos.x - sizeInGUI.x/2f, guiPos.y - sizeInGUI.y/2f);
		return new Rect(pixelMin.x, pixelMin.y, sizeInPixels.x, sizeInPixels.y);
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
		Vector3 guiZPosAndDimension = getZLocationAndScaleForGUI();
		
		Vector3 thePos = tr.position;
		thePos.z = guiZPosAndDimension.z;
		tr.position = thePos;
		
		// apply scale to locate in GUI space. Consider user defined size
		Vector3 theScale = tr.localScale;
		theScale.x = guiZPosAndDimension.x * sizeInPixels.x / (float)Screen.width;
		theScale.y = guiZPosAndDimension.y * sizeInPixels.y / (float)Screen.height;
		theScale.z = 0f;
		tr.localScale = theScale;
		
		// modify local position (not world position)
		/*Vector3 theLocalPos = tr.localPosition;
		// x position doesn't work yet
		theLocalPos.y = guiZPosAndDimension.y * 0.5f * (1f - Mathf.Abs(sizeInPixels.y)) * Mathf.Sign(sizeInPixels.y);
		tr.localPosition = theLocalPos;*/
	}
}
