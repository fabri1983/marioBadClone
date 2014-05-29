using UnityEngine;

/**
 * This class intended to keep needed info of a spawn place. 
 */
public class SpawnPositionSpot {

	private Vector2 position;
	private int priority;
	
	public SpawnPositionSpot (int pPriotity, Vector2 pPos) {
		position = pPos;
		priority = pPriotity;
	}
	
	public Vector2 getPosition () {
		return position;
	}
	
	public int getPriority () {
		return priority;
	}
}
