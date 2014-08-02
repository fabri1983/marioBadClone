using UnityEngine;

/// <summary>
/// Game object tools.
/// </summary>
static public class GameObjectTools
{
	public static float COS_45 = Mathf.Cos(45);
	public static float DEG_2_RAD_0_5 = Mathf.Deg2Rad * 0.5f;
	
	///////////////////////////////////////////////////////////
	// Essentially a reimplementation of
	// GameObject.GetComponentInChildren< T >()
	// Major difference is that this DOES NOT skip deactivated game objects
	///////////////////////////////////////////////////////////
	public static TType GetComponentInChildren< TType >( GameObject objRoot ) where TType : Component {
		// if we don't find the component in this object
		// recursively iterate children until we do
		TType tRetComponent = objRoot.GetComponent< TType >();
		 
		if( null != tRetComponent )
			return tRetComponent;
		
		// transform is what makes the hierarchy of GameObjects, so we need to access it to iterate children
		Transform trnsRoot = objRoot.transform;
		 
		// could have used foreach(), but it causes GC churn
		for( int iChild=0, c=trnsRoot.childCount; iChild < c; ++iChild )
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
	
	public static bool isHitFromAbove (float sourceMaxY, ChipmunkBody target, ChipmunkArbiter arbiter) {
		/// The collision normal is the direction of the surfaces where the two objects collided.
		/// Keep in mind that the normal points out of the first object and into the second. 
		/// If you switch the order of your collision types in the method name, it will flip the normal around.
		
		// came from above?
		if (target.velocity.normalized.y <= -COS_45) {
			// check collision points to be all above goomba's height
			for (int i=0, c=arbiter.contactCount; i < c; ++i) {
				if (sourceMaxY > (arbiter.GetPoint(i).y - arbiter.GetDepth(i)))
					return false;
			}
			return true;
		}
		return false;
	}
	
	public static bool isGrounded (ChipmunkArbiter arbiter) {
		/// The collision normal is the direction of the surfaces where the two objects collided.
		/// Keep in mind that the normal points out of the first object and into the second. 
		/// If you switch the order of your collision types in the method name, it will flip the normal around.
		
		// if normal.y is near to 1 it means it's a grounded plane
		if (Mathf.Abs(arbiter.GetNormal(0).y) >= COS_45)
			return true;
		return false;
	}

	public static bool isWallHit (ChipmunkArbiter arbiter) {
		/// The collision normal is the direction of the surfaces where the two objects collided.
		/// Keep in mind that the normal points out of the first object and into the second. 
		/// If you switch the order of your collision types in the method name, it will flip the normal around.
		
		// if normal.x is near to 1 it means it's a plane that can be considered as a wall
		if (Mathf.Abs(arbiter.GetNormal(0).x) >= COS_45)
			return true;
		return false;
	}
	
	/**
	 * Set the layer to game object and also ot their chidlren. So only considers two levels of deep.
	 */
	public static void setLayer (GameObject go, int layer) {
		go.layer = layer;
		Component[] children = go.GetComponentsInChildren<Component>();
		for (int i=0,c=children.Length; i < c; ++i)
			children[i].gameObject.layer = layer;
	}
	
	/**
	 * Set the layer to the shape and also ot their chidlren shapes. So only considers two levels of deep.
	 */
	public static void setLayerForShapes (GameObject go, uint mask) {
		// first: sets game object's shape layer 
		ChipmunkShape s = go.GetComponent<ChipmunkShape>();
		if (s != null)
			s.layers = mask;
		
		// second: set chidlren's shape layer
		ChipmunkShape[] shapes = go.GetComponentsInChildren<ChipmunkShape>();
		for (int i=0,c=shapes.Length; i < c; ++i)
			shapes[i].layers = mask;
	}
	
	public static void ChipmunkBodyDestroy (ChipmunkBody b) {
		if (b != null) {
			b.enabled = false;
			// registering a disable body will remove it from the list
			ChipmunkInterpolationManager._Register(b);
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
	public static Rect BoundsToScreenRect(Bounds bounds, Camera cam) {
		Vector3 cen = bounds.center;
		Vector3 ext = bounds.extents;
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
	
	public static void drawGUIBox (Bounds b, Camera cam) {
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
		GUI.Box (r,"");
	}
	
	/**
	 * Sets position and factor scale (w,h) to a game object, ie a quad, for covering the screen size.
	 */ 
	public static void setScreenCoverage (Camera cam, GameObject go, Vector2 coverage) {
		
		// position barely ahead from near clip plane
		float pos = (cam.nearClipPlane + 0.01f);
		// position the camera in that position (following current orientation)
		go.transform.position = pos * cam.transform.forward + cam.transform.position;
		
		float h, w;
		if(!cam.orthographic) {
			h = 2f * Mathf.Tan(cam.fov * DEG_2_RAD_0_5) * pos;
			w = h * cam.aspect;
		}
		else {
			h = cam.orthographicSize * 2f;
			w = h / Screen.height * Screen.width;
		}
		
		// scale the game object to adjust it according screen bounds
		Vector3 theScale = go.transform.localScale;
		theScale.x = w * Mathf.Abs(coverage.x);
		theScale.y = h * Mathf.Abs(coverage.y);
		theScale.z = 0f;
		go.transform.localScale = theScale;
		
		// local position of the game object containing this script
		Vector3 thePos = go.transform.localPosition;
		thePos.y = h * 0.5f * (1f - Mathf.Abs(coverage.y)) * Mathf.Sign(coverage.y);
		go.transform.localPosition = thePos;
	}
}