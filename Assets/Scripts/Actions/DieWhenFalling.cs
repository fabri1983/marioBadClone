using UnityEngine;

public class DieWhenFalling : MonoBehaviour {
	
	private IMortalFall mortalGO;
	
	void Awake () {
		mortalGO = (IMortalFall)GetComponent(typeof(IMortalFall));
	}
	
	// Update is called once per frame
	void Update () {
		// if falling beyond a theshold then execute "callback" function for correct behavior
		if (transform.position.y <= LevelManager.ENDING_DIE_ANIM_Y_POS)
			mortalGO.dieWhenFalling();
	}
}
