#if !(UNITY_3_0 || UNITY_3_0_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5)
#define UNITY_4_AND_LATER
#endif

using UnityEngine;

/// <summary>
/// Utilitary static class.
/// It's like a bag of useful functions.
/// Refactor in case it turns into a god class.
/// </summary>
static public class GameObjectTools
{
	public static float COS_45 = Mathf.Cos(45f);
	
	public static void toogleEnableAllScripts (GameObject go, bool enabled) {
		// gets all MonoBehaviour components attached to a game object and updates its enable property
		MonoBehaviour[] monos = go.GetComponents<MonoBehaviour>();
		for (int i=0, c=monos.Length; i < c; ++i)
			monos[i].enabled = enabled;
	}
	
	public static bool isActive (GameObject go) {
		#if UNITY_4_AND_LATER
		return go.activeSelf;
		#else
		return go.active;
		#endif
	}
	
	public static void setActive (GameObject go, bool val) {
		#if UNITY_4_AND_LATER
		go.SetActive(val);
		#else
		go.SetActiveRecursively(val);
		#endif
	}

	public static bool isHitFromAbove (float sourceMaxY, ChipmunkBody target, ChipmunkArbiter arbiter) {
		/// The collision normal is the direction of the surfaces where the two objects collided.
		/// Keep in mind that the normal points out of the first object and into the second. 
		/// If you switch the order of your collision types in the method name, it will flip the normal around.
		
		// came from above?
		if (target.velocity.normalized.y < -COS_45) {
			// check collision points to be all above collider's height
			for (int i=0, c=arbiter.contactCount; i < c; ++i) {
				if (sourceMaxY > (arbiter.GetPoint(i).y - arbiter.GetDepth(i)))
					return false;
			}
			return true;
		}
		return false;
	}

	public static bool isHitFromBelow (ChipmunkArbiter arbiter) {
		/// The collision normal is the direction of the surfaces where the two objects collided.
		/// Keep in mind that the normal points out of the first object and into the second. 
		/// If you switch the order of your collision types in the method name, it will flip the normal around.
		
		// if normal.y is NOT near to -1 it means it's a hit from below
		if (arbiter.GetNormal(0).y > -COS_45)
			return true;
		return false;
	}

	public static bool isGrounded (ChipmunkArbiter arbiter) {
		/// The collision normal is the direction of the surfaces where the two objects collided.
		/// Keep in mind that the normal points out of the first object and into the second. 
		/// If you switch the order of your collision types in the method name, it will flip the normal around.
		
		// if normal.y is near to 1 it means it's a hit from above
		if (Mathf.Abs(arbiter.GetNormal(0).y) > COS_45)
			return true;
		return false;
	}
	
	public static bool isCeiling (ChipmunkArbiter arbiter) {
		/// The collision normal is the direction of the surfaces where the two objects collided.
		/// Keep in mind that the normal points out of the first object and into the second. 
		/// If you switch the order of your collision types in the method name, it will flip the normal around.
		
		if (-arbiter.GetNormal(0).y < -COS_45)
			return true;
		return false;
	}

	public static bool isWallHit (ChipmunkArbiter arbiter) {
		/// The collision normal is the direction of the surfaces where the two objects collided.
		/// Keep in mind that the normal points out of the first object and into the second. 
		/// If you switch the order of your collision types in the method name, it will flip the normal around.
		
		// if normal.x is near to 1 it means it's a plane that can be considered as a wall
		if (Mathf.Abs(arbiter.GetNormal(0).x) > COS_45)
			return true;
		return false;
	}
	
	/**
	 * Set the layer to the game object and also their chidlren.
	 */
	public static void setLayer (GameObject go, int layer) {
		go.layer = layer;
		Component[] children = go.GetComponentsInChildren<Component>();
		for (int i=0,c=children.Length; i < c; ++i)
			children[i].gameObject.layer = layer;
	}
	
	/**
	 * Set the layer to the shape and also ot their chidlren shapes.
	 */
	public static void setLayerForShapes (GameObject go, uint mask) {
		// first: set game object's shape layer 
		ChipmunkShape s = go.GetComponent<ChipmunkShape>();
		if (s != null)
			s.layers = mask;
		
		// second: set chidlren's shape layer
		ChipmunkShape[] shapes = go.GetComponentsInChildren<ChipmunkShape>();
		for (int i=0,c=shapes.Length; i < c; ++i)
			shapes[i].layers = mask;
	}
	
	public static void ChipmunkBodyDestroy (ChipmunkBody b, ChipmunkShape s) {
		if (s!= null)
			s.enabled = false; // makes the shape to be removed from the space
		
		if (b != null) {
			b.enabled = false;
			// registering a disable body will remove it from the list
			ChipmunkInterpolationManager._Register(b);
		}
	}
	
	/**
	 * Returns the Axis Alligned rect in screen space this bound is covering.
	 * Doesn't considers rotation and scale.
	 * It uses the camera passed by argument for screen space conversion.
	 */
	public static Rect BoundsToScreenRectAA(Bounds bounds, Camera cam) {
	    // Get mesh origin and farthest extent (this works best with simple convex meshes)
	    Vector3 origin = cam.WorldToScreenPoint(new Vector3(bounds.min.x, bounds.max.y, bounds.center.z)); // maybe use 0f in z
	    Vector3 extent = cam.WorldToScreenPoint(new Vector3(bounds.max.x, bounds.min.y, bounds.center.z)); // maybe use 0f in z
	     
	    // Create rect in screen space and return - does not account for camera perspective
	    return new Rect(origin.x, Screen.height - origin.y, extent.x - origin.x, origin.y - extent.y);
    }
	
	/**
	 * Returns the Rect in screen space the bounds is covering.
	 * Better implementation that accounts for rotation and scale.
	 * It uses camera passed by argument for screen space conversion.
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
	
	/// <summary>
	/// Tests the hit from screen position to a given transform.
	/// </summary>
	/// <returns>
	/// True if hit from screen position.
	/// </returns>
	/// <param name='t'>
	/// The target transform to test hit against to.
	/// </param>
	/// <param name='screenPos'>
	/// Screen position in pixel coordinates.
	/// </param>
	public static bool testHitFromScreenPos (Transform t, Vector2 screenPos)
	{
		Ray ray = Camera.main.ScreenPointToRay(screenPos);
		Vector3 origin = t.InverseTransformPoint(ray.origin);
		Vector3 direction = t.InverseTransformDirection(ray.direction);
		Vector3 zeroCross = origin - direction * (origin.z/direction.z);
		return zeroCross.magnitude < 0.5f;
	}
	
	/// <summary>
	/// Tests the hit from mouse screen position to a given transform.
	/// Mouse position is inverted in Y axis. This method takes care of it.
	/// </summary>
	/// <returns>
	/// True if hit from mouse screen position.
	/// </returns>
	/// <param name='t'>
	/// The target transform to test hit against to.
	/// </param>
	/// <param name='mouseScreenPos'>
	/// Mouse sreen position in pixel coordinates.
	/// </param>
	public static bool testHitFromMousePos (Transform t, Vector2 mouseScreenPos)
	{
		Vector2 mousePosInverted;
		mousePosInverted.x = mouseScreenPos.x;
		// mouse position is in GUI space which has inverted Y axis
		mousePosInverted.y = Screen.height - mouseScreenPos.y;
		Ray ray = Camera.main.ScreenPointToRay(mousePosInverted);
		Vector3 origin = t.InverseTransformPoint(ray.origin);
		Vector3 direction = t.InverseTransformDirection(ray.direction);
		Vector3 zeroCross = origin - direction * (origin.z/direction.z);
		return zeroCross.magnitude < 0.5f;
	}
}