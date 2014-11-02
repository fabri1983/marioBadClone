
/**
 * Exposes signature for pausable game objects. 
 */
public interface IPausable
{
	void pause ();
	void resume ();
	bool isSceneOnly (); // used for allocation in subscriber lists managed by PauseGameManager
}
