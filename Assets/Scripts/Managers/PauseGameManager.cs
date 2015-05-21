using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Pause Game Manager singleton. This can't be a MonoBehaviour subclass since there is an execution order 
/// when pausable components are destroyed. This manager can't be destroyed before any pausable has executed his
/// remove() operation. So the solution to that was simply create the manager as non MonoBehaviour subclass, thus 
/// it won't appear in game hierarchy.
/// However it creates a non destroyable game object for catching OnApplicationPause().
/// </summary>
public class PauseGameManager {
	
	private List<IPausable> sceneOnly = new List<IPausable>();
	private List<IPausable> durables = new List<IPausable>();
	private List<MonoBehaviour[]> sceneOnlyMonos = new List<MonoBehaviour[]>();
	private List<MonoBehaviour[]> durablesMonos = new List<MonoBehaviour[]>();
	
	private bool paused = false;
	
	private static PauseGameManager instance = null;
	
	public static PauseGameManager Instance {
        get {
			warm();
            return instance;
        }
    }

	public static void warm () {
		// in case the class wasn't instantiated yet from another script
		if (instance == null) {
			// creates the innner instance
			instance = new PauseGameManager();
		}
	}
	
	private PauseGameManager () {
		paused = false;
		// instantiates the game object which will catch OnApplicationPause() event and communicates to this manager
		GameObject.Instantiate(Resources.Load("Prefabs/PauseGameUnityListener"));
	}

	~PauseGameManager () {
		sceneOnly.Clear();
		durables.Clear();
		instance = null;
	}
	
	public bool isPaused () {
		return paused;
	}

	/// <summary>
	/// Register the specified gameobject into the pausable components list
	/// Call this method in Start(). Calling from Awake() will fail to correctly get children componenents.
	/// </summary>
	/// <param name="p">IPausable implementation</param>
	/// <param name="go">gameobject</param>
	public void register(IPausable p, GameObject go) {
		// extract all MonoBehaviour components and
		MonoBehaviour[] comps = go.GetComponents<MonoBehaviour>();
		MonoBehaviour[] compsChildren = go.GetComponentsInChildren<MonoBehaviour>();
		MonoBehaviour[] combined = new MonoBehaviour[comps.Length + compsChildren.Length];
		Array.Copy(comps, 0, combined, 0, comps.Length);
		Array.Copy(compsChildren, 0, combined, comps.Length, compsChildren.Length);

		if (p.isSceneOnly()) {
			sceneOnly.Add(p);
			sceneOnlyMonos.Add(combined);
		}
		else {
			durables.Add(p);
			durablesMonos.Add(combined);
		}
	}
	
	public void remove (IPausable p) {
		int h = p.GetHashCode();
		if (p.isSceneOnly()) {
			for (int i=0, c=sceneOnly.Count; i<c; ++i)
				if (h == sceneOnly[i].GetHashCode()) {
					sceneOnly.RemoveAt(i);
					sceneOnlyMonos.RemoveAt(i);
					break;
				}
		}
		else {
			for (int i=0, c=durables.Count; i<c; ++i)
				if (h == durables[i].GetHashCode()) {
					durables.RemoveAt(i);
					durablesMonos.RemoveAt(i);
					break;
				}
		}
	}
	
	/// <summary>
	/// Traverse all registered components to be paused. For each one of them it calls pause() method, 
	/// and then disables all MonoBehaviour scripts.
	/// </summary>
	public void pause () {
		paused = true;
		for (int i=0, c=sceneOnly.Count; i<c; ++i) {
			sceneOnly[i].pause();
			MonoBehaviour[] monos = sceneOnlyMonos[i];
			for (int j=0, cc=monos.Length; j < cc; ++j)
				monos[j].enabled = false;
		}
		for (int i=0, c=durables.Count; i<c; ++i) {
			durables[i].pause();
			MonoBehaviour[] monos = durablesMonos[i];
			for (int j=0, cc=monos.Length; j < cc; ++j)
				monos[j].enabled = false;
		}
	}
	
	/// <summary>
	/// Traverse all registered components to be resumed. For each one of them it enables all MonoBehaviour scripts, 
	/// and then calls resume() method.
	/// </summary>
	public void resume () {
		for (int i=0, c=sceneOnly.Count; i<c; ++i) {
			MonoBehaviour[] monos = sceneOnlyMonos[i];
			for (int j=0, cc=monos.Length; j < cc; ++j)
				monos[j].enabled = true;
			sceneOnly[i].resume();
		}
		for (int i=0, c=durables.Count; i<c; ++i) {
			MonoBehaviour[] monos = durablesMonos[i];
			for (int j=0, cc=monos.Length; j < cc; ++j)
				monos[j].enabled = true;
			durables[i].resume();
		}
		
		paused = false;
	}
}
