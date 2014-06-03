using UnityEngine;
using System.Collections;

/// Attach this to any object to make a frames/second indicator.
/// 
/// It calculates frames/second over each updateInterval,
/// so the display does not keep changing wildly.
/// 
/// It is also fairly accurate at very low FPS counts (<10).
/// We do this not by simply counting frames per interval, but
/// by accumulating FPS for each frame. This way we end up with
/// corstartRect overall FPS even if the interval renders something like
/// 5.5 frames.
public class FPSHud : MonoBehaviour
{
	public bool 	useCoroutine = false;	// Use coroutines for PC targets. For mobile targets WaitForSeconds doesn't work.
	public Rect		startRect = new Rect( 1, 1, 75, 25 ); // The rect the GUI text is initially displayed at.
	public bool		updateColor = true; // Do you want the color to change if the FPS gets low
	public float	frequency = 0.33f; // The update frequency of the fps
	public int		nbDecimal = 1; // How many decimal do you want to display
 
	private float	accum   = 0f; // FPS accumulated over the interval
	private int		frames  = 0; // Frames drawn over the interval
	private Color	color = Color.white; // The color of the GUI, depending of the FPS ( R < 10, Y < 30, G >= 30 )
	private string	sFPS = ""; // The fps formatted into a string.
	private float updateTime = 0f;
	
	void Awake () {
		// only keep this object if in debug build
		if (Debug.isDebugBuild)
			DontDestroyOnLoad(gameObject);
		else
			Destroy(gameObject);
	}
	
	void Start () {
		if (useCoroutine)
	    	StartCoroutine( FPS() );
	}
 
	void Update() {
    	accum += Time.timeScale/ Time.deltaTime;
    	++frames;
		if (!useCoroutine && (Time.time - updateTime) > frequency) {
			calculateFPS();
			updateTime = Time.time;
		}
	}
 
	IEnumerator FPS() {
		// Infinite loop executed every "frenquency" secondes.
		while( true )
		{
			calculateFPS();
			yield return new WaitForSeconds( frequency );
		}
	}
 
	private void calculateFPS () {
		// Update the FPS
	    float fps = accum/frames;
	    sFPS = fps.ToString( "f" + Mathf.Clamp( nbDecimal, 0, 10 ) );

		//Update the color
		color = (fps >= 25) ? Color.green : ((fps > 10) ? Color.yellow : Color.red);

        accum = 0.0F;
        frames = 0;
	}
	
	void OnGUI() {
		if (EventType.Repaint == Event.current.type) {
			// GUI.color affects both background and text colors, so back it up
			Color origColor = GUI.color;
			GUI.color = updateColor ? color : Color.white;
			GUI.Label(startRect, sFPS + " FPS");
			GUI.color = origColor;
		}
	}
}