using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// A console to display Unity's debug logs in-game.
/// </summary>
public class LoggingConsole : MonoBehaviour {
#if DEBUG
	struct Log {
		public string message;
		public string stackTrace;
		public LogType type;
	}
	
	/// <summary>
	/// The hotkey to show and hide the console window.
	/// </summary>
	public KeyCode toggleKey = KeyCode.BackQuote;
	
	/// <summary>
	/// Whether to open the window by shaking the device (mobile-only).
	/// </summary>
	public bool shakeToOpen = true;
	
	/// <summary>
	/// The (squared root) acceleration above which the window should open.
	/// </summary>
	public float shakeAcceleration = 2f;
	
	readonly List<Log> logs = new List<Log>();
	Vector2 scrollPosition;
	bool visible;
	bool collapse;
	
	static readonly Dictionary<LogType, Color> logTypeColors = new Dictionary<LogType, Color> {
		{ LogType.Assert, Color.white },
		{ LogType.Error, Color.red },
		{ LogType.Exception, Color.red },
		{ LogType.Log, Color.white },
		{ LogType.Warning, Color.yellow },
	};
	
	const string windowTitle = "Console";
	static readonly GUIContent clearLabel = new GUIContent("Clear", "Clear the contents of the console.");
	static readonly GUIContent collapseLabel = new GUIContent("Collapse", "Hide repeated messages.");
	static GUIStyle currentStyle = null;
	
	Rect titleBarRect;
	Rect windowRect;
	float lastScreenWidth;
	float lastScreenHeight;
	float shakedTimes;
	
	void OnEnable () {
		#if UNITY_5_0
		Application.logMessageReceived += HandleLog;
		#else
		Application.RegisterLogCallback(HandleLog);
		#endif
	}
	
	void OnDisable () {
		#if UNITY_5_0
		Application.logMessageReceived -= HandleLog;
		#else
		Application.RegisterLogCallback(null);
		#endif
	}
	
	void Update () {
		if (Input.GetKeyDown(toggleKey)) {
			visible = !visible;
		}
		
		if (shakeToOpen && Mathf.Sqrt(Input.acceleration.sqrMagnitude) > shakeAcceleration) {
			if (shakedTimes > 5f) {
				visible = !visible;
				shakedTimes = 0f;
			} else {
				++shakedTimes;
			}
		}
	}
	
	void OnGUI () {
		if (EventType.Repaint == Event.current.type) {
			// if screen is resized then need to notice all listeners
			if (Screen.width != lastScreenWidth || Screen.height != lastScreenHeight) {
				titleBarRect = new Rect(0, 0, Screen.width - 20*2, 20);
				windowRect = new Rect(20, 20, Screen.width - 20*2, Screen.height - 60);
				// update screen dimension
				lastScreenWidth = Screen.width;
				lastScreenHeight = Screen.height;
			}
		}
		
		if (!visible)
			return;
		
		InitStyles();
		windowRect = GUILayout.Window(123456, windowRect, ConsoleWindow, windowTitle, currentStyle);
	}
	
	/// <summary>
	/// A window that displayss the recorded logs.
	/// </summary>
	/// <param name="windowID">Window ID.</param>
	void ConsoleWindow (int windowID) {
		GUILayout.BeginHorizontal();
		
		if (GUILayout.Button(clearLabel))
			logs.Clear();
		
		collapse = GUILayout.Toggle(collapse, collapseLabel, GUILayout.ExpandWidth(false));
		
		GUILayout.EndHorizontal();
		
		scrollPosition = GUILayout.BeginScrollView(scrollPosition);
		
		// Iterate through the recorded logs.
		for (int i = 0; i < logs.Count; i++) {
			Log log = logs[i];
			
			// Combine identical messages if collapse option is chosen.
			if (collapse) {
				bool messageSameAsPrevious = i > 0 && log.message == logs[i - 1].message;
				if (messageSameAsPrevious)
					continue;
			}
			
			GUI.contentColor = logTypeColors[log.type];
			if (LogType.Error == log.type)
				GUILayout.Label(log.message + "\n" + log.stackTrace);
			else
				GUILayout.Label(log.message);
		}
		
		GUILayout.EndScrollView();
		
		GUI.contentColor = Color.white;
		
		// Allow the window to be dragged by its title bar.
		GUI.DragWindow(titleBarRect);
	}
	
	/// <summary>
	/// Records a log from the log callback.
	/// </summary>
	/// <param name="message">Message.</param>
	/// <param name="stackTrace">Trace of where the message came from.</param>
	/// <param name="type">Type of message (error, exception, warning, assert).</param>
	void HandleLog (string message, string stackTrace, LogType type) {
		logs.Add(new Log {
			message = message,
			stackTrace = stackTrace,
			type = type,
		});
	}
	
	private void InitStyles() {
		if (currentStyle == null) {
			currentStyle = new GUIStyle(GUI.skin.window);
			currentStyle.normal.background = MakeTex(2, 2, new Color(0f, 0f, 0f, 0.6f));
		}
	}
	
	private Texture2D MakeTex( int width, int height, Color col ) {
		Color[] pix = new Color[width * height];
		for( int i = 0; i < pix.Length; ++i ) {
			pix[ i ] = col;
		}
		Texture2D result = new Texture2D( width, height );
		result.SetPixels( pix );
		result.Apply();
		return result;
	}
#endif
}
