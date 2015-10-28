using UnityEngine;

/// <summary>
/// Sneak. Is controled from a MoveAbs type of script.
/// </summary>
public class Sneak : MonoBehaviour {
	
	// NOTE: no animFPSBoost variable since this script uses is handled from the move component and it has such a variable (if any)
	
	private bool sneaking = false;
	private AnimateTiledConfig sneakAC;
	
	void Awake () {
		sneakAC = AnimateTiledConfig.getByName(gameObject, EnumAnimateTiledName.Sneak, true);
	}
	
	public void sneak () {
		if (sneaking)
			return;
		
		sneaking = true;
		sneakAC.setupAndPlay();
	}
	
	public bool isSneaking () {
		return sneaking;
	}
	
	public void stopSneaking () {
		sneaking = false;
		sneakAC.stop();
	}
}
