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
		if (hitShape.body != null && hitShape.body.velocity.normalized.y < -0.75f)
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
			if (Mathf.Abs(arbiter.GetNormal(i).y) <= 0.75f)
				return false;
		}
		return true;
	}

	public static bool isWallHit (ChipmunkArbiter arbiter) {
		// if normal.x is near to 1 it means it's a plane that can be considered as a wall
		for (int i=0; i < arbiter.contactCount; ++i) {
			if (Mathf.Abs(arbiter.GetNormal(i).x) <= 0.75f)
				return false;
		}
		return true;
	}
	
	public static void setLayerAndChildren (GameObject go, int layer) {
		go.layer = layer;
		Component[] children = go.GetComponentsInChildren<Component>();
		for (int i=0; i < children.Length; ++i)
			children[i].gameObject.layer = layer;
	}
	
	public static void ChipmunkBodyDestroy (GameObject go) {
		ChipmunkBody body = go.GetComponent<ChipmunkBody>();
		if (body != null) {
			body.enabled = false;
			// registering a disable body will remove it from the list
			ChipmunkInterpolationManager._Register(body);
		}
	}
	
	/**
	 * Returns the Rect in screen space the bounds is covering.
	 * Doesn't considers rotation and scale.
	 */
	public static Rect BoundsToScreenRect(Bounds bounds) {
	    // Get mesh origin and farthest extent (this works best with simple convex meshes)
	    Vector3 origin = Camera.main.WorldToScreenPoint(new Vector3(bounds.min.x, bounds.max.y, bounds.center.z));
	    Vector3 extent = Camera.main.WorldToScreenPoint(new Vector3(bounds.max.x, bounds.min.y, bounds.center.z));
	     
	    // Create rect in screen space and return - does not account for camera perspective
	    return new Rect(origin.x, Screen.height - origin.y, extent.x - origin.x, origin.y - extent.y);
    }
	
	/**
	 * Returns the Rect in screen space the bounds is covering.
	 * Better implementation that accounts for rotation and scale.
	 */
	public static Rect GUIRectWithObject(GameObject go) {
		Vector3 cen = go.renderer.bounds.center;
		Vector3 ext = go.renderer.bounds.extents;
		Vector2[] extentPoints = new Vector2[8] {
			Camera.main.WorldToScreenPoint(new Vector3(cen.x-ext.x, cen.y-ext.y, cen.z-ext.z)),
			Camera.main.WorldToScreenPoint(new Vector3(cen.x+ext.x, cen.y-ext.y, cen.z-ext.z)),
			Camera.main.WorldToScreenPoint(new Vector3(cen.x-ext.x, cen.y-ext.y, cen.z+ext.z)),
			Camera.main.WorldToScreenPoint(new Vector3(cen.x+ext.x, cen.y-ext.y, cen.z+ext.z)),
			 
			Camera.main.WorldToScreenPoint(new Vector3(cen.x-ext.x, cen.y+ext.y, cen.z-ext.z)),
			Camera.main.WorldToScreenPoint(new Vector3(cen.x+ext.x, cen.y+ext.y, cen.z-ext.z)),
			Camera.main.WorldToScreenPoint(new Vector3(cen.x-ext.x, cen.y+ext.y, cen.z+ext.z)),
			Camera.main.WorldToScreenPoint(new Vector3(cen.x+ext.x, cen.y+ext.y, cen.z+ext.z))
		};
		 
		Vector2 min = extentPoints[0];
		Vector2 max = extentPoints[0];
		 
		foreach(Vector2 v in extentPoints) {
			min = Vector2.Min(min, v);
			max = Vector2.Max(max, v);
		}
		 
		return new Rect(min.x, min.y, max.x-min.x, max.y-min.y);
	}
}