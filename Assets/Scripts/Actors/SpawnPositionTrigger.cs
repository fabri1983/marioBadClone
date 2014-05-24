using UnityEngine;

public class SpawnPositionTrigger : MonoBehaviour {
	
	// the priority says which is the first spawn position used to locate the player
	public int priority = 0;
	
	private SpawnPositionSpot sps;
	
	void OnTriggerEnter (Collider collider) {
		
		if (collider.tag.Equals("Mario")) {
			// update the latest spawn position index for current level
			LevelManager.Instance.updateLastSpawnPosition(getSpawnPos());
		}
	}
	
	void OnTriggerExit (Collider collider) {
		
		if (collider.tag.Equals("Mario")) {
			// update the latest spawn position index for current level
			LevelManager.Instance.updateLastSpawnPosition(getSpawnPos());
		}
	}
	
	public SpawnPositionSpot getSpawnPos () {
		// cache the spot position
		if (sps == null)
			sps = new SpawnPositionSpot(priority, transform.position);
		return sps;
	}
}
