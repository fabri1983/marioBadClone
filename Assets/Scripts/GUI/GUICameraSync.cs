using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Attach this Script to main camera. Needs to be created per scene.
/// This script updates all GUI elements which need to be alligned with the Main Camera for correct display.
/// This only has sense for GUI custom elements.
/// </summary>
[ExecuteInEditMode]
public class GUICameraSync : MonoBehaviour {

	/**
	 * Used for sorting the listeners by priority
	 */
	private sealed class PriorityComparator : IComparer<IGUICameraSyncable> {
		int IComparer<IGUICameraSyncable>.Compare(IGUICameraSyncable synca, IGUICameraSyncable syncb) {
			int a = synca.getPriority();
			int b = syncb.getPriority();
			if (a < b)
				return -1;
			else if (a > b)
				return 1;
			return 0;
		}
	}

	private PriorityComparator comparator = new PriorityComparator();
	private List<IGUICameraSyncable> listeners = new List<IGUICameraSyncable>();

	void OnDestroy () {
		// since this script lives per scene, then clean the list of subscribers
		listeners.Clear();
		listeners = null;
	}

	public void register (IGUICameraSyncable syncable) {
		listeners.Add(syncable);
		listeners.Sort(comparator);
	}

	// Use Update to avoid wrong Touch Event Manager updates.
	void Update () {
		// call listeners to update the camera
		for (int i=0, c=listeners.Count; i < c; ++i)
			listeners[i].updateCamera();

		// if this is invoked in LateUpdate then some gui custom elements doesn't work with TouchEventManager
		transformGUIContainers();
	}

	/// <summary>
	/// Updates the transform of the GUI containers which depend on the camera position and rotation.
	/// </summary>
	public void transformGUIContainers () {
		Vector3 camPos = transform.position;
		Quaternion camRot = transform.rotation;
		GameObject guiContainer_so = LevelManager.getGUIContainerSceneOnly();
		GameObject guiContainer_nd = LevelManager.getGUIContainerNonDestroyable();
		
		// update according camera's current transform
		guiContainer_so.transform.position = camPos;
		guiContainer_so.transform.rotation = camRot;
		if (guiContainer_nd != null) {
			guiContainer_nd.transform.position = camPos;
			guiContainer_nd.transform.rotation = camRot;
		}
	}
}
