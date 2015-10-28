using UnityEngine;

public class SpawnPositionTrigger : MonoBehaviour {
	
	// the priority says which is the first spawn position used to locate the player when level is first time loaded
	public int priority = 0;
	
	private SpawnPositionSpot sps;
	
	void Awake () {
		sps.priority = priority;
		sps.position = transform.position;
	}
	
	public static bool beginCollisionWithPlayer (ChipmunkArbiter arbiter) {
		ChipmunkShape shape1, shape2;
	    arbiter.GetShapes(out shape1, out shape2);
		
		LevelManager.Instance.updateLastSpawnPosition(shape2.GetComponent<SpawnPositionTrigger>().sps);
		
		// Returning false from a begin callback means to ignore the collision response for these two colliding shapes 
		// until they separate. Also for current frame. Ignore() does the same but next fixed step.
		return false;
	}
	
	public SpawnPositionSpot getSpawnPos () {
		// LevelManager can call this method before the Awake() of this game object happens
		if (sps.priority == LevelManager.INVALID_PRIORITY) {
			sps.priority = priority;
			sps.position = transform.position;
		}
		return sps;
	}
}
