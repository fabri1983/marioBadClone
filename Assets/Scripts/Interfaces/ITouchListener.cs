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
	/// Gets the screen Rect element representing the bounds of the listener's game object.
	/// The Rect element is Axis Aligned, so it's like a square in screen with no rotation.
	/// </summary>
	Rect getScreenBoundsAA ();
	
	/// <summary>
	/// Since in Runtime (no Editor mode) the gameObject.isStatic property isn't valid, use this 
	/// method to define if listener's gameObject is static.
	/// </summary>
	bool isStatic ();
}
