using UnityEngine;
using System.Collections;

public class Hide : MonoBehaviour {
	
	public float hideColliderProportion = 0.80f;
	
	private bool hidden;
	private AnimateTiledConfig hideAC;
	private float colliderCenterY, centerOffsetY;
	private ChipmunkBoxShape box;
	
	// Use this for initialization
	void Awake () {
		hideAC = AnimateTiledConfig.getByName(gameObject, EnumAnimateTiledName.Hide, true);
		hidden = false;
		
		// take the collider and some useful values
		ChipmunkBoxShape[] boxes = GetComponents<ChipmunkBoxShape>();
		for (int i=0,c=boxes.Length; i<c; ++i) {
			box = boxes[i];
			// the koopa has two chipmunk boxes, take the correct one
			if ("KoopaTroopa".Equals(box.collisionType))
				break;
		}
		colliderCenterY = box.center.y;
		centerOffsetY = ((1f - hideColliderProportion)*0.5f) * box.size.y;
	}
	
	void Update () {
		// set the correct sprite animation
		if (hidden)
			hideAC.setupAndPlay();
	}
	
	public void hide () {
		if (!hidden) {
			// resize the collider
			Vector3 theSize = box.size;
			theSize.y *= hideColliderProportion;
			box.size = theSize;
			// transform the collider
			Vector3 theCenter = box.center;
			theCenter.y -= centerOffsetY;
			box.center = theCenter;
		}
		hidden = true;
		hideAC.setupAndPlay();
	}
	
	public void unHide () {
		// restore the collider's size and position
		if (hidden) {
			// transform the collider
			Vector3 theCenter = box.center;
			theCenter.y = colliderCenterY;
			box.center = theCenter;
			// resize the collider
			Vector3 theSize = box.size;
			theSize.y /= hideColliderProportion;
			box.size = theSize;
		}
		hidden = false;
	}
	
	public bool isHidden () {
		return hidden;
	}
}
