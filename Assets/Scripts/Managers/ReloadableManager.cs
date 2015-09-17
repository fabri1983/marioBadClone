using UnityEngine;

public sealed class ReloadableManager {

	private static IReloadable[] reloadables = new IReloadable[14]; // set size experimentally
	private static Vector3[] reloadablesSpawnPos = new Vector3[14]; // same size than reseteables structure
	
	private static ReloadableManager instance = null;
	
	public static ReloadableManager Instance {
		get {
			// in case the class wasn't instantiated yet from another script
			if (instance == null) {
				// creates the innner instance
				instance = new ReloadableManager();
			}
			return instance;
		}
	}
	
	private ReloadableManager () {
	}
	
	~ReloadableManager () {
		for (int i=0,c=reloadables.Length; i < c; ++i)
			reloadables[i] = null;
		reloadables = null;
		reloadablesSpawnPos = null;
		instance = null;
	}
	
	public void register (IReloadable listener, Vector3 pos) {
		bool inserted = false;
		for (int i=0,c=reloadables.Length; i < c; ++i) {
			if (reloadables[i] == null) {
				reloadables[i] = listener;
				reloadablesSpawnPos[i].Set(pos.x, pos.y, pos.z);
				inserted = true;
				break;
			}
		}
		if (!inserted) {
			Debug.LogError("Reloadables container for listeners is full. Increment size one more unit.");
		}
	}
	
	public void remove (IReloadable listener) {
		int id = listener.GetHashCode();
		for (int i=0,c=reloadables.Length; i < c; ++i) {
			if (reloadables[i] == null)
				continue;
			if (id == reloadables[i].GetHashCode()) {
				reloadables[i] = null;
				break;
			}
		}
	}
	
	public void reloadActors () {
		for (int i=0,c=reloadables.Length; i < c; ++i) {
			if (reloadables[i] != null) {
				reloadables[i].onReloadLevel(reloadablesSpawnPos[i]);
			}
		}
	}
}
