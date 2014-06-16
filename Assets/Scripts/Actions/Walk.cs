
public class Walk : WalkAbs {
	
	public override void reset () {
		//if (idle != null) idle.setIdle(true);
		walking = false;
		stop = false;
	}
	
	public override void walk (float velocity) {
		if (stop)
			return;
		gain = 1f;
		base._walk(velocity);
	}
}
