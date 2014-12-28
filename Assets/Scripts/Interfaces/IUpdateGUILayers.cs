
/// <summary>
/// Interface to be implemented by those scripts that modify camera's transform and want to update 
/// the GUI components that depends on that camera.
/// </summary>
public interface IUpdateGUILayers
{
	void updateGUILayers ();
}
