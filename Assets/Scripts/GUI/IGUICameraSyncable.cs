
/// <summary>
/// Interface to be implemented by those scripts that modify camera's transform which is same camera that GUI elements depend on.
/// This way any GUI coperation that depends on camera's transform is performed after the camera has been updated.
/// </summary>
public interface IGUICameraSyncable {

	void updateCamera();
}
