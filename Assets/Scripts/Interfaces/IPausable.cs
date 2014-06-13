
/**
 * Exposes signature for pausable game objects. 
 */
public interface IPausable
{
	void pause ();
	void resume ();
	bool isSceneOnly ();
}
