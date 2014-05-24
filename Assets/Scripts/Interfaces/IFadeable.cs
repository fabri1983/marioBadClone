
/**
 * Exposes signature for fedeable game objects. 
 * Although is thought as for camera fades between scene to black and viceversa
 */
public interface IFadeable
{
	void startFading (EnumFadeDirection direction);
	void stopFading ();
	bool isTransitionFinished ();
	EnumFadeDirection getFadingDirection ();
}

