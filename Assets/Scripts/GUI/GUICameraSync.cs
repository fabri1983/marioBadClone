using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Attach this Script to main camera. Needs to be created per scene.
/// This script updates all GUI elements which need to be alligned with the Main Camera for correct display.
/// This only has sense for GUI custom elements.
/// </summary>
public class GUICameraSync : MonoBehaviour {
	
	void Update () {
		// if this is invoked in LateUpdate then some gui custom elements doesn't work with TouchEventManager
		updateGUITransforms();
	}
	
	/// <summary>
	/// Updates the transform of the GUI containers which depend on the camera position and rotation.
	/// </summary>
	public void updateGUITransforms () {
		Vector3 camPos = transform.position;
		Quaternion camRot = transform.rotation;
		Transform guiContainer_so = LevelManager.getGUIContainerSceneOnly().transform;
		Transform guiContainer_nd = LevelManager.getGUIContainerNonDestroyable().transform;
		// update according camera's current transform
        guiContainer_so.position = camPos;
        guiContainer_so.rotation = camRot;
        guiContainer_nd.position = camPos;
        guiContainer_nd.rotation = camRot;
	}
}
