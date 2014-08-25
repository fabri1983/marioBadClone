using UnityEngine;
using System.Collections.Generic;

public class PauseGameManager : MonoBehaviour {
	
	private List<IPausable> sceneOnly = new List<IPausable>();
	private List<IPausable> durables = new List<IPausable>();
		
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
		if (instance != null && instance != this)
			Destroy(this.gameObject);
		else {
			instance = this;
			DontDestroyOnLoad(gameObject);
		}
	}
	
	void OnApplicationQuit () {
		sceneOnly.Clear();
		durables.Clear();
		instance = null;
	}
	
	public void register(IPausable p) {
		if (p.isSceneOnly())
			sceneOnly.Add(p);
		else
			durables.Add(p);
	}
	
	public void remove (IPausable p) {
		int h = p.GetHashCode();
		if (p.isSceneOnly()) {
			for (int i=0, c=sceneOnly.Count; i<c; ++i)
				if (h == sceneOnly[i].GetHashCode()) {
					sceneOnly.RemoveAt(i);
					break;
				}
		}
		else {
			for (int i=0, c=durables.Count; i<c; ++i)
				if (h == durables[i].GetHashCode()) {
					durables.RemoveAt(i);
					break;
				}
		}
	}
	
	public void pause() {
		for (int i=0, c=sceneOnly.Count; i<c; ++i)
			sceneOnly[i].pause();
		for (int i=0, c=durables.Count; i<c; ++i)
			durables[i].pause();
	}
	
	public void resume() {
		for (int i=0, c=sceneOnly.Count; i<c; ++i)
			sceneOnly[i].resume();
		for (int i=0, c=durables.Count; i<c; ++i)
			durables[i].resume();
	}
}
