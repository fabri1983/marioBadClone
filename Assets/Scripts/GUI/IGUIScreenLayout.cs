
/// <summary>
/// Interface to be implemented by those GUI custom elements which needs to be notified when 
/// the GUIScreenLayoutManager decides to fire an update event (i.e. screen resize).
/// </summary>
public interface IGUIScreenLayout {
	
	void updateForGUI();
}
