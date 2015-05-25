using UnityEngine;
using System.Collections.Generic;

public abstract class Effect : Pausable {
	
	public int priority = 0;
	public float startDelaySecs = 0f;
	
	private Effect nextEffect = null;
	private List<IEffectListener> listeners = null;
	private bool isPriorizable = false;
	
	void Awake () {
		if (GetComponent<EfectPrioritizer>() != null) {
			this.enabled = false;
			isPriorizable = true;
		}
		PauseGameManager.Instance.register(this as Pausable, this as MonoBehaviour);
		ownAwake();
	}
	
	void Start () {
		// if the effect is not priorizable then it can start immediatly
		if (!isPriorizable)
			executeEffect();
	}
	
	void OnDestroy () {
		PauseGameManager.Instance.remove(this as Pausable);
		ownOnDestroy();
	}
	
	protected abstract void ownAwake ();
	
	protected abstract void ownEffectStarts ();
	
	protected abstract void ownOnDestroy ();
	
	public void addNextEffect (Effect next) {
		nextEffect = next;
	}
	
	public void addNextListener (IEffectListener listener) {
		if (listeners == null)
			listeners = new List<IEffectListener>();
		listeners.Add(listener);
	}
	
	public void executeEffect() {
		this.enabled = true;
		ownEffectStarts();
	}
	
	protected void effectEnded () {
		this.enabled = false;
		// the next line only has sense if this Effect is the last one in the chain
		executeListeners();
		// call the next Effect in the chain
		executeNextEffect();
	}
	
	private void executeListeners () {
		if (listeners != null) {
			for (int i=0, c=listeners.Count; i < c; ++i)
				listeners[i].onLastEffectEnd();
		}
	}
	
	private void executeNextEffect () {
		if (nextEffect != null)
			nextEffect.executeEffect();
	}
	
	public override void beforePause () {}
	
	public override void afterResume () {}
	
	public override bool isSceneOnly () {
		// used for allocation in subscriber lists managed by PauseGameManager
		return false;
	}
}
