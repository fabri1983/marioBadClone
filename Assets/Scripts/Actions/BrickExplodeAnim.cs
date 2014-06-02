using UnityEngine;

public class BrickExplodeAnim : MonoBehaviour {
	
	private float destroyAfterSecs = 1.5f;
	private float explosionPow = 600;
	
	// Use this for initialization
	void Start () {
		
		GameObject.Destroy(this.gameObject, destroyAfterSecs);
		// apply a small +y translation to avoid appearing this brick lower than original brick
		transform.Translate(0f, 2f, 0f);
		
		explode();
	}
	
	private void explode () {

		// Note: the game object is compound of 4 blocks

		for (int i=0; i < transform.childCount; ++i)
		{	
			Transform child = transform.GetChild(i);
			float rand = Random.value;
			if (rand < 0.3f)
				rand = 0.3f;
			if (child.tag.Equals("Left")) {
		    	child.rigidbody.AddRelativeForce(-explosionPow * rand, explosionPow * rand, 0);
				child.rigidbody.AddRelativeTorque(0f, -explosionPow, explosionPow);
			}
			else {
				child.rigidbody.AddRelativeForce(explosionPow * rand, explosionPow * rand, 0);
				child.rigidbody.AddRelativeTorque(0f, explosionPow, -explosionPow);
			}
		}
	}
}
