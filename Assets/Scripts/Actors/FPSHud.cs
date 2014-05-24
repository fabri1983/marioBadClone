using UnityEngine;
using System.Collections;

public class FPSHud : MonoBehaviour
{
	// Attach this to any object to make a frames/second indicator.
	//
	// It calculates frames/second over each updateInterval,
	// so the display does not keep changing wildly.
	//
	// It is also fairly accurate at very low FPS counts (<10).
	// We do this not by simply counting frames per interval, but
	// by accumulating FPS for each frame. This way we end up with
	// corstartRect overall FPS even if the interval renders something like
	// 5.5 frames.

	public Rect		startRect = new Rect( 1, 1, 75, 25 ); // The rect the GUI text is initially displayed at.
	public bool		updateColor = true; // Do you want the color to change if the FPS gets low
	public float	frequency = 0.5f; // The update frequency of the fps
	public int		nbDecimal = 1; // How many decimal do you want to display
 
	private float	accum   = 0f; // FPS accumulated over the interval
	private int		frames  = 0; // Frames drawn over the interval
	private Color	color = Color.white; // The color of the GUI, depending of the FPS ( R < 10, Y < 30, G >= 30 )
	private string	sFPS = ""; // The fps formatted into a string.
 
	void Awake () {
		if (Debug.isDebugBuild == false)
			Destroy(gameObject);
	}
	
	void Start ()
	{
	    StartCoroutine( FPS() );
	}
 
	void Update()
	{
    	accum += Time.timeScale/ Time.deltaTime;
    	++frames;
	}
 
	IEnumerator FPS()
	{
		// Infinite loop executed every "frenquency" secondes.
		while( true )
		{
			// Update the FPS
		    float fps = accum/frames;
		    sFPS = fps.ToString( "f" + Mathf.Clamp( nbDecimal, 0, 10 ) );
 
			//Update the color
			color = (fps >= 25) ? Color.green : ((fps > 10) ? Color.yellow : Color.red);
 
	        accum = 0.0F;
	        frames = 0;
 
			yield return new WaitForSeconds( frequency );
		}
	}
 
	void OnGUI()
	{
		if (EventType.Repaint == Event.current.type) {
			GUI.color = updateColor ? color : Color.white;
			GUI.Label(startRect, sFPS + " FPS");
		}
	}
}