using UnityEngine;
using System.Collections.Generic;

public class PauseGameManager : MonoBehaviour {
	
	private List<IPausable> sceneOnly = new List<IPausable>();
	private List<IPausable> durables = new List<IPausable>();
	private List<MonoBehaviour[]> sceneOnlyMonos = new List<MonoBehaviour[]>();
	private List<MonoBehaviour[]> durablesMonos = new List<MonoBehaviour[]>();
	
	private bool paused = false;
	
	private static PauseGameManager instance = null;
	
	public static PauseGameManager Instance {
        get {
            if (instance == null) {
				// creates a game object with this script component.
				instance = new GameObject("PauseGameManager").AddComponent<PauseGameManager>();
			}
            return instance;
        }
    }
	
	void Awake () {
		paused = false;
		if (instance != null && instance != this)
			Destroy(this.gameObject);
		else {
			instance = this;
			DontDestroyOnLoad(gameObject);
		}
	}
	
	void OnDestroy () {
		sceneOnly.Clear();
		durables.Clear();
		instance = null;
	}
	
	public bool isPaused () {
		return paused;
	}
	
	public void register(IPausable p, GameObject go) {
		if (p.isSceneOnly()) {
			sceneOnly.Add(p);
			sceneOnlyMonos.Add(go.GetComponents<MonoBehaviour>());
		}
		else {
			durables.Add(p);
			durablesMonos.Add(go.GetComponents<MonoBehaviour>());
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
	public void pause() {
		
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
	public void resume() {
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
