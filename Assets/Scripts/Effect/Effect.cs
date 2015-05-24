using UnityEngine;
using System.Collections.Generic;

public abstract class Effect : MonoBehaviour {
	
	public int priority = 0;
	
	private Effect nextEffect = null;
	private List<IEffectListener> listeners = null;
	private bool isPriorizable = false;
	
	void Awake () {
		if (GetComponent<EfectPrioritizer>() != null) {
			this.enabled = false;
			isPriorizable = true;
		}
		ownAwake();
	}
	
	void Start () {
		// if the effect is not priorizable then it can start immediatly
		if (!isPriorizable)
			executeEffect();
	}
	
	protected abstract void ownAwake ();
	
	protected abstract void ownEffectStarts ();
	
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
}
