
/**
 * Exposes signature for pausable game objects. 
 */
public interface IPausable
{
	void pause (); // if the implementation script needs a special behaviour before going in pause
	void resume (); // if the implementation script needs a special behaviour after comming back from pause
	bool isSceneOnly (); // used for allocation in subscriber lists managed by PauseGameManager
}
