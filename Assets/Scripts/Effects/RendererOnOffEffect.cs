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
		
		// once the component did the effect reset the delay property so it is not delayed when coming back to same option.
		base.startDelaySecs = 0f;

		if (beforeLoadNextScene)
			endEffect();
	}
	
	protected override void ownEndEffect () {
		renderer.enabled = invertBehaviour ? true : false;
	}
	
	protected override void ownOnDestroy () {
	}
}
