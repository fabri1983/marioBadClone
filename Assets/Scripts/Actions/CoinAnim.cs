using UnityEngine;

public class CoinAnim : MonoBehaviour {
	
	// Update is called once per frame
	void Update () {
		// moves the coin upwards
		transform.Translate(0f, 4f * Time.deltaTime, 0f, Space.World);
	}
}
