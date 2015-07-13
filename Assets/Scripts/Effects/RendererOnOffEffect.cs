using UnityEngine;
using System.Collections;

/// <summary>
/// This effect only set the renderer status to enabled/disabled in order to 
/// show/hide the GUI element attached to the game object.
/// </summary>
public class RendererOnOffEffect : Effect {

	public bool startRendererOn = false;
	
	protected override void ownAwake () {
	}
	
	protected override void ownStartEffect () {
		renderer.enabled = startRendererOn;
	}
	
	protected override void ownEndEffect () {
		renderer.enabled = !startRendererOn;
	}
	
	protected override void ownOnDestroy () {
	}
}
