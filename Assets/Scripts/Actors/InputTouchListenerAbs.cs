using UnityEngine;

public abstract class InputTouchListenerAbs : MonoBehaviour, ITouchListener {

	private bool _doPropagation = true;
	private bool _consumed = false;
	
	public bool stopPropagation() {
		/// only stop propagation if flag was set to false on listener's registration 
		/// and  if listener has consumed the touch event
		
		if (_doPropagation || !_consumed)
			return false;
		return true;
	}
	
	public void setPropagationFlag (bool val) {
		_doPropagation = val;
	}
	
	public void setConsumed (bool val) {
		_consumed = val;
	}
	
	public abstract GameObject getGameObject ();
	
	public abstract void OnBeganTouch (Touch t);
	
	public abstract void OnStationaryTouch (Touch t);
	
	public abstract void OnEndedTouch (Touch t);
}
