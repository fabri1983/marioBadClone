
public class Walk : WalkAbs {
	
	public override void reset () {}
	
	public override void walk (float velocity) {
		if (!base.enabled)
			return;
		gain = 1f;
		base._walk(velocity);
	}

	protected override void stopWalking () {}
}
