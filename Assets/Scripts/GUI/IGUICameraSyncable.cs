
/// <summary>
/// Interface to be implemented by those scripts that modify camera's transform which is same camera that GUI elements depend on.
/// This way any GUI operation dependant on camera's transform is performed after the camera has been updated.
/// </summary>
public interface IGUICameraSyncable {

	/// <summary>
	/// Callback used when camera has been updated
	/// </summary>
	void updateCamera();

	/// <summary>
	/// Gets the priority used when updating due to GUICameraSync.
	/// </summary>
	/// <returns>The priority.</returns>
	int getPriority();
}
