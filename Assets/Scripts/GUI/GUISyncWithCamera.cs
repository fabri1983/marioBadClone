using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Attach this Script to main camera.
/// This script updates all GUI elements which need to be alligned with the Main Camera for correct display.
/// This only has sense for GUI custom elements.
/// </summary>
public class GUISyncWithCamera : MonoBehaviour {
	
	// Containers for GUI custom elements (background, foreground, buttons, text, etc)
	// They are only used for organizational purpose
    private Transform guiContainer_so; // scene only GUI container
    private Transform guiContainer_nd; // non destroyable GUI container
    
    void Awake () {
        // NOTE: this works fine here only if this script is created per scene.
        guiContainer_so = getGUIContainerSceneOnly();
        guiContainer_nd = getGUIContainerNonDestroyable();
	}
	
	void Start () {
		// update the GUI containers transform since they depend on the camera position and rotation
		updateGUITransforms();
	}
	
	/**
	 * LateUpdate is called after all Update functions have been called.
	 * Dependant objects might have moved during Update.
	 */
	void LateUpdate () {
		updateGUITransforms();
	}
	
	/// <summary>
	/// Updates the transform of the GUI containers which depend on the camera position and rotation.
	/// </summary>
	public void updateGUITransforms () {

		if (guiContainer_so != null) {
            guiContainer_so.position = transform.position;
            guiContainer_so.rotation = transform.rotation;
        }
        if (guiContainer_nd != null) {
            guiContainer_nd.position = transform.position;
            guiContainer_nd.rotation = transform.rotation;
        }
	}
	
	/// <summary>
	/// Gets all the GUIParallax registered components and configure them according to 
	/// the extension of the level: min world position and max world position.
	/// Not all registered componenets may have a GUIParallax component.
	/// </summary>
	/// <param name='playerSpawnPos'>
	/// Player's current spawn position.
	/// </param>
	/// <param name='levelExtent'>
	/// Level dimension in world coordinates. Used to configure the parallax scripts.
	/// </param>
	public void setParallaxProperties (Vector2 playerSpawnPos, Rect levelExtent) {
		
		float length = Mathf.Abs(levelExtent.xMin - levelExtent.xMax);
		float height = Mathf.Abs(levelExtent.yMin - levelExtent.yMax);
        
        for (int k=0; k<2; ++k) {
            // get the GUIParallax components
            GUIParallax[] parallax = null;
            if (k==0) {
                if (guiContainer_so == null)
                    continue;
                parallax = guiContainer_so.GetComponentsInChildren<GUIParallax>();
            }
            else {
                if (guiContainer_nd == null)
                    continue;
                parallax = guiContainer_so.GetComponentsInChildren<GUIParallax>();
            }
            
            for (int i=0,c=parallax.Length; i<c;++i) {
                parallax[i].setLevelExtentWorldUnits(length, height);
                parallax[i].setOffsetWorldCoords(playerSpawnPos.x, playerSpawnPos.y);
            }
        }

	}
	
	/// <summary>
	/// Gets the transform of the game object named GUI_Container_nd which contains all the 
	/// GUI elements in the scene that aren't destroyable.
	/// </summary>
	/// <returns>
	/// A Transform element for applying any operation on it
	/// </returns>
	private Transform getGUIContainerNonDestroyable () {
	    GameObject container = GameObject.Find("GUI_Container_nd");
	    if (container == null)
	        return null;
	    return container.transform;
	}
	
	/// <summary>
	/// Gets the transform of the game object named GUI_Container_so which contains all the 
	/// GUI elements in the scene that only exist during the liveness of the scene (destroyables).
	/// </summary>
	/// <returns>
	/// A Transform element for applying any operation on it
	/// </returns>
	private Transform getGUIContainerSceneOnly () {
	    GameObject container = GameObject.Find("GUI_Container_so");
	    if (container == null)
	        return null;
	    return container.transform;
	}
}
