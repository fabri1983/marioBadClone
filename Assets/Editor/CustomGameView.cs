using System;
using UnityEngine;
using UnityEditor;

/**
 * Undock the Game window and let you set the resolution you want thru a popup window.
 * Allows transform the resolution of the Game window according to mobile's ppi and monitor's ppi.
 * It has a workaround for setting free aspect ratio (top-left option in Game window).
 * 
 * Place this script in YourProject/Assets/Editor.
 */
public class CustomGameView : EditorWindow {
	
	public enum RESOLUTIONS {
		_480_x_320,
		_800_x_480,
		_854_x_480,
		_1024_x_600,
		_1280_x_800
	}
	
	private static int menuOffset = 17; // extra height for new Game window (maybe for top bar?)
	
	// must be set to the ppi (dpi) reported by Screen.dpi, not the actual ppi.
	private static int mobilePPI = 163;
	private static RESOLUTIONS resolution = RESOLUTIONS._480_x_320;
	private static Vector2 mobileRes;
	// physical horizontal size (inch) of the monitor you use to view the Editor. Is used for getting monitor's ppi
	private static float monitorInchX = 15f;
	private static WindowInfos gameWindow = null;
	// calculated when transforming mobile resolution according mobile ppi and monitor's resolution
	private static float scaleFactor = 1;
	private static bool applyScaleFactor = false;
	
	private static CustomGameView popup;
	
	[MenuItem ("Window/Set Custom Window Game")]
	static void Init () {
		// this gets our editor window to let the user enter the configuration
		popup = (CustomGameView)(EditorWindow.GetWindow(typeof(CustomGameView)));
	}
	
	void OnGUI () {
		setNewGameWindow();
	}
	
	private static void setNewGameWindow () {
		// get width and height from user
		resolution = (RESOLUTIONS)EditorGUILayout.EnumPopup("Resolution", resolution);
		switch (resolution) {
			case RESOLUTIONS._480_x_320: {mobileRes.x = 480; mobileRes.y = 320; break;}
			case RESOLUTIONS._800_x_480: {mobileRes.x = 800; mobileRes.y = 480; break;}
			case RESOLUTIONS._854_x_480: {mobileRes.x = 854; mobileRes.y = 480; break;}
			case RESOLUTIONS._1024_x_600: {mobileRes.x = 1024; mobileRes.y = 600; break;}
			case RESOLUTIONS._1280_x_800: {mobileRes.x = 1280; mobileRes.y = 800; break;}
		default: break;
		}
		
		// get mobile ppi
		mobilePPI = EditorGUILayout.IntField("Mobile PPI", mobilePPI);
		
		// get monitor's horizontal inches from user
		monitorInchX = EditorGUILayout.FloatField("Monitor Inches X", monitorInchX);
		
		applyScaleFactor = EditorGUILayout.Toggle("Apply Scale Factor to World Objects", applyScaleFactor);
		
		// wait for user press button
		bool pressed = GUILayout.Button("Set");
		if (!pressed)
			return;
		
		// mobile device horizontal inches
		Vector2 mobileInch;
		mobileInch.x = mobileRes.x / mobilePPI;
		
		// mobile resolution trasnformed to our monitor's resolution
		Vector2 mobileOnMonitorRes = new Vector2(mobileRes.x, mobileRes.y);
		
		if (applyScaleFactor) {
			mobileOnMonitorRes.x = (mobileInch.x * Screen.currentResolution.width) / monitorInchX;
			float aspect = mobileRes.y / mobileRes.x;
			mobileOnMonitorRes.y = mobileOnMonitorRes.x * aspect;
			// this factor to be applied in Editor mode only, multiply the scale of your game world by this
			scaleFactor = mobileOnMonitorRes.x / mobileRes.x;
		}
		
		// get current game window position
		Rect position = GetMainGameView().position;
		// initialize window-specific title data and such (this is cached)
		if (gameWindow == null)
			gameWindow = new WindowInfos();
		gameWindow.game.isOpen = false; //workaround for setting free aspect ratio
		gameWindow.game.position = new Rect(position.x, position.y - 5, mobileOnMonitorRes.x, mobileOnMonitorRes.y+menuOffset); // left,top,width,height
		
		// apply the scale factor? so every object looks like it was in mobile ppi
		if (applyScaleFactor) {
			scaleFactor = scaleFactor * 1f;
			// aplicar escala en XY a game objects y desplazar la main camera en XY por mismo factor
		}
		
		// close the popup window
		if (popup != null)
			popup.Close();
	}
	
	/**
	 * Get current Game window.
	 * This used to retrieve current position
	 */
	private static EditorWindow GetMainGameView() {
		System.Type T = System.Type.GetType("UnityEditor.GameView,UnityEditor");
		System.Reflection.MethodInfo GetMainGameView = T.GetMethod("GetMainGameView",System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
		System.Object Res = GetMainGameView.Invoke(null,null);
		return (EditorWindow)Res;
	}
	
	
	class WindowInfos
    {
        // note: some of this data might need to change a little between different versions of Unity
        public WindowInfo game = new WindowInfo("UnityEditor.GameView", "Game", "Window/Game");
    }
 
    class WindowInfo
    {
        string defaultTitle;
        string menuPath;
        Type type;
 
        public WindowInfo(string typeName, string defaultTitle=null, string menuPath=null, System.Reflection.Assembly assembly=null)
        {
            this.defaultTitle = defaultTitle;
            this.menuPath = menuPath;
 
            if(assembly == null)
                assembly = typeof(UnityEditor.EditorWindow).Assembly;
            type = assembly.GetType(typeName);
            if(type == null)
                Debug.LogWarning("Unable to find type \"" + typeName + "\" in assembly \"" + assembly.GetName().Name + "\".\nYou might want to update the data in WindowInfos.");
        }
 
        public EditorWindow[] FindAll()
        {
            if(type == null)
                return new EditorWindow[0];
            return (EditorWindow[])(Resources.FindObjectsOfTypeAll(type));
        }
 
        public EditorWindow FindFirst()
        {
            foreach(EditorWindow window in FindAll())
                return window;
            return null;
        }
 
        public EditorWindow FindFirstOrCreate()
        {
            EditorWindow window = FindFirst();
            if(window != null)
                return window;
            if(type == null)
                return null;
            if(menuPath != null && menuPath.Length != 0)
                EditorApplication.ExecuteMenuItem(menuPath);
            window = EditorWindow.GetWindow(type, false, defaultTitle);
            return window;
        }
 
        // shortcut for setting/getting the position and size of the first window of this type.
        // when setting the position, if the window doesn't exist it will also be created.
        public Rect position
        {
            get
            {
                EditorWindow window = FindFirst();
                if(window == null)
                    return new Rect(0,0,0,0);
                return window.position;
            }
            set
            {
                EditorWindow window = FindFirstOrCreate();
                if(window != null)
                    window.position = value;
            }
        }
 
        // shortcut for deciding if any windows of this type are open,
        // or for opening/closing windows
        public bool isOpen
        {
            get
            {
                return FindAll().Length != 0;
            }
            set
            {
                if(value)
                    FindFirstOrCreate();
                else
                    foreach(EditorWindow window in FindAll())
                        window.Close();
            }
        }
    }
}
