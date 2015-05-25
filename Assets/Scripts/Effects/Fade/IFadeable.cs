
/**
 * Exposes signature for fadeable game objects. 
 * Is thought as for camera fades between or in scene.
 */
public interface IFadeable
{
	void startFading (EnumFadeDirection direction);
	void stopFading ();
	bool isTransitionFinished ();
	EnumFadeDirection getFadingDirection ();
}

