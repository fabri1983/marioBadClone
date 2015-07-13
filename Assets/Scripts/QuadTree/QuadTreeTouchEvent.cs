using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Quad Tree for input touch listeners. Considers on screen positions and axis alligned screen bounds.
/// Accepted screen bound:   ----
/// 						|    |
/// 						|    |
/// 						 ----
/// Not porperly handled screen bound:   /\
/// 									 \/
/// (because is not axis alligned)
/// 
/// </summary>
public class QuadTreeTouchEvent {
	
	public const int FIRST_LEVEL = 0;
	public const int DEEP_LEVEL_THRESHOLD = 1;
	
	private QuadNode root;
	private ListenerLists[] tempArr = new ListenerLists[4];
	// this temporal list is created once per quad tree root instance
	private int tempIndex = 0;
	
	/// <summary>
	/// Initializes a new instance of the <see cref="QuadTreeTouchEvent"/> class.
	/// </summary>
	public QuadTreeTouchEvent () {
		root = new QuadNode(FIRST_LEVEL);
	}
	
	/// <summary>
	/// Tests the Rect screenBounds and returns the list of listeners that it affects.
	/// There is a max of four lists.
	/// </summary>
	/// <param name='screenBounds'>
	/// Screen bounds.
	/// </param>
	public ListenerLists[] add (Rect screenBounds) {
		clear(tempArr);
		addRecursive(root, screenBounds, tempArr, 0,0, Screen.width,Screen.height);
		return tempArr;
	}
	
	private void addRecursive (QuadNode n, Rect screenBounds, ListenerLists[] arr, float x0, float y0, float x1, float y1) {
		
		// reached max level?
		if (n.level == DEEP_LEVEL_THRESHOLD) {
			if (n.leafContent == null)
				n.leafContent = new ListenerLists();
			arr[tempIndex++] = n.leafContent;
			// the caller object is responsible to allocate the listener instance in the many listeners list
			return;
		}
		
		/// Test four points of rect screenBounds to get the destiny quad/s. 
		/// It can falls in as much as four quads.
		///                   y1
		///     -------------   x1
		///    |  001 | 010  |
		///     -------------
		///    | 1000 | 100  |
		/// y0  -------------
		///  x0
		/// 
		// use actual limits to calculate destiny quadrant/s
		int test = quadrantTest(screenBounds, x0,y0,x1,y1);
		
		/// depending on which quadrant/s it falls we need to allocate space for the 
		/// quad/s node and continue testing recursively
		if (((test >> 0) & 1) == 1) {
			if (n.topLeft == null)
				n.topLeft = new QuadNode(n.level + 1);
			addRecursive(n.topLeft, screenBounds, arr, x0,y1/2f, x1/2f,y1);
		}
		if (((test >> 1) & 1) == 1) {
			if (n.topRight == null)
				n.topRight = new QuadNode(n.level + 1);
			addRecursive(n.topRight, screenBounds, arr, x1/2f,y1/2f, x1,y1);
		}
		if (((test >> 2) & 1) == 1) {
			if (n.botRight == null)
				n.botRight = new QuadNode(n.level + 1);
			addRecursive(n.botRight, screenBounds, arr, x1/2f,y0, x1,y1/2f);
		}
		if (((test >> 3) & 1) == 1) {
			if (n.botLeft == null)
				n.botLeft = new QuadNode(n.level + 1);
			addRecursive(n.botLeft, screenBounds, arr, x0,y0, x1/2f,y1/2f);
		}
	}
	
	private static int quadrantTest (Rect bounds, float x0, float y0, float x1, float y1) {
		int t = 0;
		
		/// Executes the test for each 4 points of screen bound element.
		/// This way we ensure the bound lement falls in as many quadrants it can
		
		// minX is on left side?
		if (bounds.xMin < x1/2f) {
			if (bounds.yMin > y1/2f)
				t |= 1; // quad topLeft
			else t |= 8; // quad botLeft
			
			if (bounds.yMax > y1/2f)
				t |= 1; // quad topLeft
			// other case processed above because it means minY also was <= y1/2f
			
			// whenever maxX is on left, means minX is on side too
			if (bounds.xMax < x1/2f)
				return t;
			// the other case is processed outside
		}
		// minX is on right
		else {
			if (bounds.yMin > y1/2f)
				t |= 2; // quad topRight
			else t |= 4; // quad botRight
			
			if (bounds.yMax > y1/2f)
				t |= 2; // quad topRight
			// other case processed above because it means minY also was <= y1/2f
			
			// whenever minX lies on right side, then maxX is also on right side
			return t;
		}
		
		// last case: maxX is on right
		if (bounds.yMin > y1/2f)
			t |= 2; // quad topRight
		else t |= 4; // quad botRight
		if (bounds.yMax > y1/2f)
			t |= 2; // quad topRight
		// other case processed above because it means minY also was <= y1/2f
		
		return t;
	}
	
	public ListenerLists traverse (Vector2 screenPos) {
		return traverseRecursive(root, screenPos, 0,0, Screen.width,Screen.height);
	}
	
	public ListenerLists[] traverse (Rect bounds) {
		clear(tempArr);
		
		// if is leaf then return the list of listeners
		if (root.level == DEEP_LEVEL_THRESHOLD) {
			tempArr[tempIndex++] = root.leafContent;
			return tempArr;
		}
		
		ListenerLists ll;
		Vector2 v2aux;
		
		// do some check if we can save some traverse work, for example when the entire bound is in one quadrant
		
		v2aux.x = bounds.xMin;
		v2aux.y = bounds.yMax;
		ll = traverse(v2aux);
		if (ll != null) tempArr[tempIndex++] = ll;
		
		v2aux.x = bounds.xMax;
		v2aux.y = bounds.yMax;
		ll = traverse(v2aux);
		if (ll != null) tempArr[tempIndex++] = ll;
		
		v2aux.x = bounds.xMax;
		v2aux.y = bounds.yMin;
		ll = traverse(v2aux);
		if (ll != null) tempArr[tempIndex++] = ll;
		
		v2aux.x = bounds.xMin;
		v2aux.y = bounds.yMin;
		ll = traverse(v2aux);
		if (ll != null) tempArr[tempIndex++] = ll;
		
		return tempArr;
	}
	
	private ListenerLists traverseRecursive (QuadNode n, Vector2 p, float x0, float y0, float x1, float y1) {
		// if is leaf then return the list of listeners
		if (n.level == DEEP_LEVEL_THRESHOLD)
			return n.leafContent;
		
		// search in quad tree a leaf containing the screen position
		
		if (p.x < x1/2f) {
			if (p.y > y1/2f)
				return n.topLeft==null? null : traverseRecursive(n.topLeft, p, x0,y1/2f, x1/2f,y1);
			else
				return n.botLeft==null? null : traverseRecursive(n.botLeft, p, x0,y0, x1/2f,y1/2f);
		}
		else {
			if (p.y > y1/2f)
				return n.topRight==null? null : traverseRecursive(n.topRight, p, x1/2f,y1/2f, x1,y1);
			else
				return n.botRight==null? null : traverseRecursive(n.botRight, p, x1/2f,y0, x1,y1/2f);
		}
	}
	
	private void clear (ListenerLists[] arr) {
		tempIndex = 0;
		arr[0]= arr[1]= arr[2]= arr[3]= null;
	}
	
	public void clear () {
		root.clear();
	}
}
