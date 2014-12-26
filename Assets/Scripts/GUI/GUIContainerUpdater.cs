using UnityEngine;
using System.Collections;

public class GUIContainerUpdater : MonoBehaviour, IUpdateGUILayers {

	// containers for GUI cusotm elements (background, foreground, buttons, text, etc)
	private Transform guiLayers_so;  // scene only GUI container
	private Transform guiLayers_nd;  // non destroyable GUI container
	
	void Awake () {
		// NOTE: this works fine here only if this script is created per scene.
		guiLayers_so = LevelManager.getGUILayersSceneOnly();
		guiLayers_nd = LevelManager.getGUILayersNonDestroyable();
	}
	
	void Start () {
		// update the GUI transforms since they depend on the camera position and rotation
		updateGUILayers();
	}
	
	/**
	 * LateUpdate is called after all Update functions have been called.
	 * Dependant objects might have moved during Update.
	 */
	void LateUpdate () {
		updateGUILayers();
	}
	
	/// <summary>
	/// Updates the GUILayers transform since they depend on the camera position and rotation.
	/// </summary>
	public void updateGUILayers () {
		if (guiLayers_so != null) {
			guiLayers_so.position = transform.position;
			guiLayers_so.rotation = transform.rotation;
		}
		if (guiLayers_nd != null) {
			guiLayers_nd.position = transform.position;
			guiLayers_nd.rotation = transform.rotation;
		}
	}
}
