using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Pause Game Manager singleton. This can't be a MonoBehaviour subclass since there is an execution order 
/// when pausable components are destroyed. This manager can't be destroyed before any pausable has executed his
/// remove() operation. So the solution to that was simply create the manager as non MonoBehaviour subclass, thus 
/// it won't appear in game hierarchy.
/// However it needs a non destroyable game object for catching OnApplicationPause() wich is the PauseGameUnityListener.
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
		sceneOnly = null;
		durables = null;
		instance = null;
	}
	
	public bool isPaused () {
		return paused;
	}

	/// <summary>
	/// Register the specified gameobject into the pausable components list
	/// Call this method in Start(). Calling from Awake() will fail to correctly get children components.
	/// </summary>
	/// <param name="p">Pausable implementation</param>
	/// <param name="go">GameObject</param>
	public void register(IPausable p, GameObject go) {
		// extract all MonoBehaviour components in one big array
		MonoBehaviour[] comps = go.GetComponentsInChildren<MonoBehaviour>();
		MonoBehaviour[] combined = new MonoBehaviour[comps.Length];
		Array.Copy(comps, 0, combined, 0, comps.Length);
		/*MonoBehaviour[] comps = go.GetComponents<MonoBehaviour>();
		MonoBehaviour[] compsChildren = go.GetComponentsInChildren<MonoBehaviour>();
		MonoBehaviour[] combined = new MonoBehaviour[comps.Length + compsChildren.Length];
		Array.Copy(comps, 0, combined, 0, comps.Length);
		Array.Copy(compsChildren, 0, combined, comps.Length, compsChildren.Length);*/

		if (p.isSceneOnly()) {
			sceneOnly.Add(p);
			sceneOnlyMonos.Add(combined);
		}
		else {
			durables.Add(p);
			durablesMonos.Add(combined);
		}
	}
	
	/// <summary>
	/// Register the specified MonoBehaviour into the pausable components list
	/// Call this method in Start(). Calling from Awake() will fail to correctly get children components.
	/// </summary>
	/// <param name="p">Pausable implementation</param>
	/// <param name="mono">MonoBehaviour</param>
	public void register(IPausable p, MonoBehaviour mono) {
		MonoBehaviour[] monoArray = new MonoBehaviour[1];
		monoArray[0] = mono;
		if (p.isSceneOnly()) {
			sceneOnly.Add(p);
			sceneOnlyMonos.Add(monoArray);
		}
		else {
			durables.Add(p);
			durablesMonos.Add(monoArray);
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
			sceneOnly[i].beforePause();
			MonoBehaviour[] monos = sceneOnlyMonos[i];
			bool alreadyDisabled = true;
			for (int j=0, cc=monos.Length; j < cc; ++j) {
				if (!monos[j].enabled)
					continue;
				monos[j].enabled = false;
				alreadyDisabled = false;
			}
			sceneOnly[i].DoNotResume = alreadyDisabled;
		}
		for (int i=0, c=durables.Count; i<c; ++i) {
			durables[i].beforePause();
			MonoBehaviour[] monos = durablesMonos[i];
			bool alreadyDisabled = true;
			for (int j=0, cc=monos.Length; j < cc; ++j) {
				if (!monos[j].enabled)
					continue;
				monos[j].enabled = false;
				alreadyDisabled = false;
			}
			durables[i].DoNotResume = alreadyDisabled;
		}
	}
	
	/// <summary>
	/// Traverse all registered components to be resumed. For each one of them it enables all MonoBehaviour scripts, 
	/// and then calls resume() method.
	/// </summary>
	public void resume () {
		for (int i=0, c=sceneOnly.Count; i<c; ++i) {
			if (sceneOnly[i].DoNotResume)
				continue;
			MonoBehaviour[] monos = sceneOnlyMonos[i];
			for (int j=0, cc=monos.Length; j < cc; ++j) {
				monos[j].enabled = true;
			}
			sceneOnly[i].afterResume();
		}
		for (int i=0, c=durables.Count; i<c; ++i) {
			if (durables[i].DoNotResume)
				continue;
			MonoBehaviour[] monos = durablesMonos[i];
			for (int j=0, cc=monos.Length; j < cc; ++j) {
				monos[j].enabled = true;
			}
			durables[i].afterResume();
		}
		
		paused = false;
	}
}
