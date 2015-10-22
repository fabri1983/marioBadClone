using UnityEngine;
using System.Collections.Generic;

public abstract class Effect : MonoBehaviour, IPausable {
	
	private static int idGen = 0;
	public int priority = 0;
	public float startDelaySecs = 0f;
	public bool beforeLoadNextScene = false;
	
	private int _id; // internal identificacion for Equals() and GetHashCode()
	private Effect nextEffect = null;
	private List<IEffectListener> listeners = null;
	private bool isPriorizable = false;
	private bool doNotResume; // used by the pause manager
	
	void Awake () {
		_id = idGen++;
		
		// if this game object has the EffectPrioritizerChain component it means it will be part of 
		// an execution chain so start it as disabled
		if (GetComponent<EffectPrioritizerChain>() != null)
			isPriorizable = true;

		// if effect is marked as execute before load next scene then register it in the correct listener
		if (beforeLoadNextScene)
			BeforeLoadNextSceneManager.Instance.register(this);
		
		PauseGameManager.Instance.register(this as IPausable, this as MonoBehaviour);
		ownAwake();
	}
	
	void Start () {
		if (isPriorizable || beforeLoadNextScene) {
			this.enabled = false;
		} else {
			// effect is not priorizable and not executed before load next scene then it starts immediatly
			startEffect();
		}
	}
	
	void OnDestroy () {
		if (beforeLoadNextScene)
			BeforeLoadNextSceneManager.Instance.remove(this);
		PauseGameManager.Instance.remove(this as IPausable);
		ownOnDestroy();
	}
	
	public override bool Equals (object obj) {
		if (obj == null)
			return false;
		Effect other = obj as Effect;
		return other._id == this._id;
	}
	
	public override int GetHashCode() {
		return 17 * 23 + _id;
	}
	
	protected abstract void ownAwake ();
	
	protected abstract void ownStartEffect (); // only call from this startEffect()
	
	protected abstract void ownEndEffect (); // only call from this endEffect()
	
	protected abstract void ownOnDestroy ();
	
	public void setNextEffect (Effect next) {
		nextEffect = next;
	}
	
	public void addNextListener (IEffectListener listener) {
		if (listeners == null)
			listeners = new List<IEffectListener>();
		listeners.Add(listener);
	}
	
	public void startEffect() {
		this.enabled = true;
		if (startDelaySecs > 0f)
			Invoke("ownStartEffect", startDelaySecs);
		else
			ownStartEffect();
	}
	
	public void endEffect () {
		this.enabled = false;
		ownEndEffect();
		// the next line only has sense if this Effect is the last one in the chain
		executeListeners();
		// call the next Effect in the chain
		executeNextEffect();
	}
	
	private void executeListeners () {
		if (listeners != null) {
			for (int i=0, c=listeners.Count; i < c; ++i)
				listeners[i].onLastEffectEnd();
			
			// execute the listeners of this effect just once
			listeners.Clear();
		}
	}
	
	private void executeNextEffect () {
		if (nextEffect != null)
			nextEffect.startEffect();
	}
	
	public bool DoNotResume {
		get {return doNotResume;}
		set {doNotResume = value;}
	}
	
	public void beforePause () {}
	
	public void afterResume () {}
	
	public bool isSceneOnly () {
		// used for allocation in subscriber lists managed by PauseGameManager
		return false;
	}
}
