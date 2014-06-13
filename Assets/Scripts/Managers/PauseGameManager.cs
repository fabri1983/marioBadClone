using UnityEngine;
using System.Collections;

public class PauseGameManager : MonoBehaviour {
	
	private IPausable[] arrSceneOnly = null;
	private IPausable[] arrDurables = null;
		
	private static PauseGameManager instance = null;
	
	public static PauseGameManager Instance {
        get {
            if (instance == null) {
				// creates a game object with this script component.
				instance = new GameObject("PauseGameManager").AddComponent<PauseGameManager>();
				DontDestroyOnLoad(instance);
				instance.collectDurables();
			}
            return instance;
        }
    }
	
	public void collectDurables() {
		// collects all pausable components in the scene
		IPausable[] arr = (IPausable[])FindObjectsOfType(typeof(IPausable));
		// count durable objects and create the array
		
	}
	
	public void collectSceneOnly() {
		arrSceneOnly = null;
		// collects all pausable components in the scene
		IPausable[] arr = (IPausable[])FindObjectsOfType(typeof(IPausable));
		// count durable objects and create the array
		
	}
	
	public void pause() {
		if (arrSceneOnly != null) {
			for (int i=0, c=arrSceneOnly.Length; i<c; ++i)
				arrSceneOnly[i].pause();
		}
		if (arrDurables != null) {
			for (int i=0, c=arrDurables.Length; i<c; ++i)
				arrDurables[i].pause();
		}
	}
	
	public void resume() {
		if (arrSceneOnly != null) {
			for (int i=0, c=arrSceneOnly.Length; i<c; ++i)
				arrSceneOnly[i].resume();
		}
		if (arrDurables != null) {
			for (int i=0, c=arrDurables.Length; i<c; ++i)
				arrDurables[i].resume();
		}
	}
}
