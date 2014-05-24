using UnityEngine;

/// <summary>
/// Interface Input listener.
/// Interface to be implemented by those classes that register itself against the InputTouchManager 
/// and want to be notified whenever a touch phase is fired.
/// </summary>
/// 
public interface ITouchListener
{
	/// <summary>
	/// Callback when a began touch phase fired.
	/// </summary>
	/// <param name='t'>
	/// ie: the touch event just fired.
	/// </param>
	void OnBeganTouch (Touch t);
	
	/// <summary>
	/// Callback when a stationary touch phase fired.
	/// </summary>
	/// <param name='t'>
	/// ie: the touch event just fired.
	/// </param>
	void OnStationaryTouch (Touch t);
	
	/// <summary>
	/// Callback when an ended touch phase fired.
	/// </summary>
	/// <param name='t'>
	/// ie: the touch event just fired.
	/// </param>
	void OnEndedTouch (Touch t);
	
	/// <summary>
	/// Returns the game object that implement this interface.
	/// </summary>
	/// <returns>
	/// The game object.
	/// </returns>
	GameObject getGameObject ();
	
	/// <summary>
	/// Useful to know if the listener doesn't need to be called again for next touch phase.
	/// </summary>
	/// <returns>
	/// True if the listener doesn't need to be called again for next touch phase
	/// </returns>
	bool stopPropagation ();
	
	/// <summary>
	/// Remember that multitouch events are valid, and this let you set if you want that the listener 
	/// avoids to be called again if a previous touch phase already called the listener callback methods.
	/// </summary>
	/// <param name='val'>
	/// Value.
	/// </param>
	void setPropagationFlag (bool val);
	
	/// <summary>
	/// It lets you set if the event was consumed or not. Needed for stop propagation behavior.
	/// </summary>
	void setConsumed (bool val);
}
