using UnityEngine;

/**
 * This script will keep your desired aspect ratio, adding bars if needed.
 * http://gamedesigntheory.blogspot.ie/2010/09/controlling-aspect-ratio-in-unity.html
 * Set this script to your main camera.
 * The bars are shown as a second camera:
 * 	1. Create a camera
 *  2. Set the camera's depth value to -2 so it's rendered underneath the main camera (whose depth value defaults to -1).
 *  3. Set background to the desired color (black).
 *  4. Set the Clear Flags to "Solid Color", set the Culling Mask to "Nothing".
 */
public class CamAspectRatio : MonoBehaviour, IGUIScreenLayout {
	
	private Rect screenRect;
	
	// Use this for initialization
	void Start () 
	{
		if (!LevelManager.keepAspectRatio)
			return;
		screenRect = new Rect(0, 0, Screen.width, Screen.height);
		setAspectRatio();
		
		GUIScreenLayoutManager.Instance.register(this as IGUIScreenLayout);
	}
	
	void OnDestroy () {
		GUIScreenLayoutManager.Instance.remove(this as IGUIScreenLayout);
	}
	
	public void updateForGUI () {
		if (!LevelManager.keepAspectRatio)
			return;
		
		setAspectRatio();
		screenRect.x = 0f;
		screenRect.y =	0f;
		screenRect.width = Screen.width;
		screenRect.height = Screen.height;
	}
	
	public void setAspectRatio () {
		if (!LevelManager.keepAspectRatio)
			return;
		
	    // set the desired aspect ratio (the values in this example are
	    // hard-coded for 16:10, but you could make them into public
	    // variables instead so you can set them at design time)
	    float targetAspect = LevelManager.ASPECT_W / LevelManager.ASPECT_H;
	
	    // determine the game window's current aspect ratio
	    float windowAspect = (float)Screen.width / (float)Screen.height;
	
	    // current viewport height should be scaled by this amount
	    float scaleHeight = windowAspect / targetAspect;
	
	    // obtain camera component so we can modify its viewport
	    Camera camera = GetComponent<Camera>();
	
	    // if scaled height is less than current height, add letterbox
	    if (scaleHeight < 1.0f)
	    {  
	        Rect rect = camera.rect;
	
	        rect.width = 1.0f;
	        rect.height = scaleHeight;
	        rect.x = 0;
	        rect.y = (1.0f - scaleHeight) / 2.0f;
	        
	        camera.rect = rect;
	    }
	    else // add pillarbox
	    {
	        float scalewidth = 1.0f / scaleHeight;
	
	        Rect rect = camera.rect;
	
	        rect.width = scalewidth;
	        rect.height = 1.0f;
	        rect.x = (1.0f - scalewidth) / 2.0f;
	        rect.y = 0;
	
	        camera.rect = rect;
	    }
	}
}
