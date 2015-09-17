using UnityEngine;

/// <summary>
/// IReloadable. Used by LevelManager for allowing registering of components which have to 
/// be setup when current scene is reloaded (ie: player dies).
/// </summary>
public interface IReloadable {
	
	void onReloadLevel (Vector3 spawnPos);
	
}
