using UnityEngine;
using System.Collections;

/// <summary>
/// This effect only set the renderer status to enabled/disabled in order to show/hide 
/// the GUI element attached to the game object.
/// AS default it initializes as disabled. Use the invertBehaviour variable to control it.
/// </summary>
public class RendererOnOffEffect : Effect {

	public bool invertBehaviour = false;
	
	protected override void ownAwake () {
		renderer.enabled = invertBehaviour ? true : false;
	}
	
	protected override void ownStartEffect () {
		renderer.enabled = invertBehaviour ? false : true;
	}
	
	protected override void ownEndEffect () {
		renderer.enabled = invertBehaviour ? true : false;
	}
	
	protected override void ownOnDestroy () {
	}
}
