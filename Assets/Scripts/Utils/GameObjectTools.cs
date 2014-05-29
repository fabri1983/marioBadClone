using UnityEngine;

/// <summary>
/// Game object tools.
/// </summary>
static public class GameObjectTools
{
	///////////////////////////////////////////////////////////
	// Essentially a reimplementation of
	// GameObject.GetComponentInChildren< T >()
	// Major difference is that this DOES NOT skip deactivated game objects
	///////////////////////////////////////////////////////////
	public static TType GetComponentInChildren< TType >( GameObject objRoot ) where TType : Component
	{
		// if we don't find the component in this object
		// recursively iterate children until we do
		TType tRetComponent = objRoot.GetComponent< TType >();
		 
		if( null != tRetComponent )
			return tRetComponent;
		
		// transform is what makes the hierarchy of GameObjects, so
		// need to access it to iterate children
		Transform trnsRoot = objRoot.transform;
		int iNumChildren = trnsRoot.childCount;
		 
		// could have used foreach(), but it causes GC churn
		for( int iChild = 0; iChild < iNumChildren; ++iChild )
		{
			// recursive call to this function for each child
			// break out of the loop and return as soon as we find
			// a component of the specified type
			tRetComponent = GetComponentInChildren< TType >( trnsRoot.GetChild( iChild ).gameObject );
			if( null != tRetComponent )
				break;
		}
	 
		return tRetComponent;
	}
	
	public static bool isHitFromAbove (float sourceMaxY, ChipmunkShape hitShape, ChipmunkArbiter arbiter) {
		bool cameFromAbove = false;

		// ask for body's velocity
		if (hitShape.body != null && hitShape.body.velocity.normalized.y < -0.7f)
			cameFromAbove = true;
		
		// if came from above then check collision points to be all above goomba's height
		if (cameFromAbove) {
			for (int i=0; i < arbiter.contactCount; ++i) {
				if (sourceMaxY > arbiter.GetPoint(i).y + (-1f*arbiter.GetDepth(i)))
					return false;
			}
			return true;
		}
		return false;
	}
	
	public static bool isGrounded (ChipmunkArbiter arbiter) {
		// if normal.y is near to 1 it means it's a grounded plane
		for (int i=0; i < arbiter.contactCount; ++i) {
			if (Mathf.Abs(arbiter.GetNormal(i).y) <= 0.7)
				return false;
		}
		return true;
	}

	public static bool isWallHit (ChipmunkArbiter arbiter) {
		// if normal.x is near to 1 it means it's a plane that can be considered as a wall
		for (int i=0; i < arbiter.contactCount; ++i) {
			if (Mathf.Abs(arbiter.GetNormal(i).x) <= 0.7)
				return false;
		}
		return true;
	}
	
	public static void ChipmunkBodyDestroy (GameObject go) {
		ChipmunkBody body = go.GetComponent<ChipmunkBody>();
		if (body != null) {
			body.enabled = false;
			// registering a disable body will remove it from the list
			ChipmunkInterpolationManager._Register(body);
		}
	}
}