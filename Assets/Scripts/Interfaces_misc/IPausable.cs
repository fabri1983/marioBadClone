
/// <summary>
/// Pause behavior for game objects and monobehavior scripts.
/// It also adds some needed behavior to avoid resuming an element that was not meant to be resumed.
/// For example, an element whose enabled state is false before the pause event so when resuming it 
/// has not need to be enabled.
/// </summary>
public interface IPausable {
	
	/// <summary>
	/// This acts as a interface property which is used internally by the PauseGameManager.
	/// Classes must only implement this property but no use it at all.
	/// </summary>
	bool DoNotResume
	{
		get;
		set;
	}
	
	/// <summary>
	/// Called just before going into pause.
	/// </summary>
	void beforePause ();
	
	/// <summary>
	/// Called just after going back from pause.
	/// </summary>
	void afterResume ();
	
	/// <summary>
	/// Used for allocation in subscriber lists managed by PauseGameManager.
	/// If the element only exist per scene then returns true, else returns false.
	/// </summary>
	/// <returns>bool</returns>
	bool isSceneOnly ();
}