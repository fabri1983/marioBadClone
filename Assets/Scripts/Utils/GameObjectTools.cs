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
	 * Returns the Axis Alligned rect in screen space this bound is covering.
	 * Doesn't considers rotation and scale.
	 * It uses Camera.main for screen space conversion.
	 */
	public static Rect BoundsToScreenRectAA(Bounds bounds) {
	    // Get mesh origin and farthest extent (this works best with simple convex meshes)
	    Vector3 origin = Camera.main.WorldToScreenPoint(new Vector3(bounds.min.x, bounds.max.y, bounds.center.z)); // maybe use 0f in z
	    Vector3 extent = Camera.main.WorldToScreenPoint(new Vector3(bounds.max.x, bounds.min.y, bounds.center.z)); // maybe use 0f in z
	     
	    // Create rect in screen space and return - does not account for camera perspective
	    return new Rect(origin.x, Screen.height - origin.y, extent.x - origin.x, origin.y - extent.y);
    }
	
	/**
	 * Returns the Rect in screen space the bounds is covering.
	 * Better implementation that accounts for rotation and scale.
	 * It uses Camera.main for screen space conversion.
	 */
	public static Rect BoundsToScreenRect(Bounds bounds) {
		Vector3 cen = bounds.center;
		Vector3 ext = bounds.extents;
		Camera cam = Camera.main;
		Vector2[] extentPoints = new Vector2[8] {
			cam.WorldToScreenPoint(new Vector3(cen.x-ext.x, cen.y-ext.y, cen.z-ext.z)),
			cam.WorldToScreenPoint(new Vector3(cen.x+ext.x, cen.y-ext.y, cen.z-ext.z)),
			cam.WorldToScreenPoint(new Vector3(cen.x-ext.x, cen.y-ext.y, cen.z+ext.z)),
			cam.WorldToScreenPoint(new Vector3(cen.x+ext.x, cen.y-ext.y, cen.z+ext.z)),
			 
			cam.WorldToScreenPoint(new Vector3(cen.x-ext.x, cen.y+ext.y, cen.z-ext.z)),
			cam.WorldToScreenPoint(new Vector3(cen.x+ext.x, cen.y+ext.y, cen.z-ext.z)),
			cam.WorldToScreenPoint(new Vector3(cen.x-ext.x, cen.y+ext.y, cen.z+ext.z)),
			cam.WorldToScreenPoint(new Vector3(cen.x+ext.x, cen.y+ext.y, cen.z+ext.z))
		};
		 
		Vector2 min = extentPoints[0];
		Vector2 max = extentPoints[0];
		 
		foreach(Vector2 v in extentPoints) {
			min = Vector2.Min(min, v);
			max = Vector2.Max(max, v);
		}
		 
		return new Rect(min.x, min.y, max.x-min.x, max.y-min.y);
	}
	
	public static void drawGUIBox (Bounds b) {
		Camera cam = Camera.main;
 		float margin = 0;
		
		// is object behind us?
		if (cam.WorldToScreenPoint (b.center).z < 0) return;
		 
		//All 8 vertices of the bounds
		Vector3[] pts = new Vector3[8];
		pts[0] = cam.WorldToScreenPoint (new Vector3 (b.center.x + b.extents.x, b.center.y + b.extents.y, b.center.z + b.extents.z));
		pts[1] = cam.WorldToScreenPoint (new Vector3 (b.center.x + b.extents.x, b.center.y + b.extents.y, b.center.z - b.extents.z));
		pts[2] = cam.WorldToScreenPoint (new Vector3 (b.center.x + b.extents.x, b.center.y - b.extents.y, b.center.z + b.extents.z));
		pts[3] = cam.WorldToScreenPoint (new Vector3 (b.center.x + b.extents.x, b.center.y - b.extents.y, b.center.z - b.extents.z));
		pts[4] = cam.WorldToScreenPoint (new Vector3 (b.center.x - b.extents.x, b.center.y + b.extents.y, b.center.z + b.extents.z));
		pts[5] = cam.WorldToScreenPoint (new Vector3 (b.center.x - b.extents.x, b.center.y + b.extents.y, b.center.z - b.extents.z));
		pts[6] = cam.WorldToScreenPoint (new Vector3 (b.center.x - b.extents.x, b.center.y - b.extents.y, b.center.z + b.extents.z));
		pts[7] = cam.WorldToScreenPoint (new Vector3 (b.center.x - b.extents.x, b.center.y - b.extents.y, b.center.z - b.extents.z));
		 
		//Get them in GUI space
		for (int i=0;i<pts.Length;i++) pts[i].y = Screen.height-pts[i].y;
		 
		//Calculate the min and max positions
		Vector3 min = pts[0];
		Vector3 max = pts[0];
		for (int i=1;i<pts.Length;i++) {
			min = Vector3.Min (min, pts[i]);
			max = Vector3.Max (max, pts[i]);
		}
		 
		//Construct a rect of the min and max positions and apply some margin
		Rect r = Rect.MinMaxRect (min.x,min.y,max.x,max.y);
		r.xMin -= margin;
		r.xMax += margin;
		r.yMin -= margin;
		r.yMax += margin;
		 
		//Render the box
		GUI.Box (r,"This is a box covering the player");
	}
}