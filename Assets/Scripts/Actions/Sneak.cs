using UnityEngine;

/// <summary>
/// Sneak. Is controled from a MoveAbs type of script.
/// </summary>
public class Sneak : AnimateTiledConfig {
	
	// NOTE: no animFPSBoost variable since this script uses is handled from the move component and it has such a variable (if any)
	
	private bool sneaking = false;
	
	private Crouch crouch;
	
	public void sneak () {
		if (sneaking)
			return;
		
		sneaking = true;
		
		if (animComponent != null) {
			animComponent.setRowLimits(rowStartAnim, rowLengthAnim);
			animComponent.setColLimits(maxColsAnimInRow, colStartAnim, colLengthAnim);
			animComponent.setPingPongAnim(pingPongAnim);
			animComponent.Play();
		}
	}
	
	public bool isSneaking () {
		return sneaking;
	}
	
	public void stopSneaking () {
		sneaking = false;
	}
}
