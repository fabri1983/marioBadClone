using UnityEngine;

/**
 * This class intended to keep needed info of a spawn place. 
 */
public class SpawnPositionSpot {

	private Vector3 position;
	private int priority;
	
	public SpawnPositionSpot (int pPriotity, Vector3 pPos) {
		position = pPos;
		priority = pPriotity;
	}
	
	public Vector3 getPosition () {
		return position;
	}
	
	public int getPriority () {
		return priority;
	}
}
