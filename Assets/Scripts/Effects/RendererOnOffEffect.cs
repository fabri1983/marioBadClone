using UnityEngine;
using System.Collections;

/// <summary>
/// This effect only set the renderer status to enabled/disabled in order to 
/// show/hide the GUI element attached to the game object.
/// </summary>
public class RendererOnOffEffect : Effect {

	public RENDER_STATUS startRendering = RENDER_STATUS.OFF;
	public RENDER_STATUS doEffectRendering = RENDER_STATUS.ON;
	
	protected override void ownAwake () {
		renderer.enabled = startRendering == RENDER_STATUS.ON;
	}
	
	protected override void ownStartEffect () {
		renderer.enabled = startRendering == RENDER_STATUS.ON;
	}
	
	protected override void ownEndEffect () {
		renderer.enabled = doEffectRendering == RENDER_STATUS.ON;
	}
	
	protected override void ownOnDestroy () {
	}
}

public enum RENDER_STATUS {
	ON, OFF
}