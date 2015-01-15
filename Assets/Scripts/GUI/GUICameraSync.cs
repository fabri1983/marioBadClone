using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Attach this Script to main camera. Needs to be created per scene.
/// This script updates all GUI elements which need to be alligned with the Main Camera for correct display.
/// This only has sense for GUI custom elements.
/// </summary>
[ExecuteInEditMode]
public class GUICameraSync : MonoBehaviour {
	
	// Use Update to avoid wrong Touch Event Manager updates.
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
		GameObject guiContainer_so = LevelManager.getGUIContainerSceneOnly();
		GameObject guiContainer_nd = LevelManager.getGUIContainerNonDestroyable();
		// update according camera's current transform
		guiContainer_so.transform.position = camPos;
		guiContainer_so.transform.rotation = camRot;
#if UNITY_EDITOR
		// /in Editor Mode the game object guiContainer_nd only exists in first scene, so will be null while editing another scene.
		// This is made to keep gui objects in sync with camera in Editor Mode only.
		if (!UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode) {
			if (guiContainer_nd != null) {
				guiContainer_nd.transform.position = camPos;
				guiContainer_nd.transform.rotation = camRot;
			}
		}
#else
		guiContainer_nd.transform.position = camPos;
		guiContainer_nd.transform.rotation = camRot;
#endif
	}
}
