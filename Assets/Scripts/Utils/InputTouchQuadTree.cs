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
public class InputTouchQuadTree {
	
	private const int DEEP_LEVEL_THRESHOLD = 0;
	
	// this is the object to be saved in the Quad Tree's leaves
	private ListenerLists leafContent;
	
	private int level;
	private InputTouchQuadTree topLeft, topRight, botLeft, botRight;
	
	/// <summary>
	/// Initializes a new instance of the <see cref="InputTouchQuadTree"/> class.
	/// </summary>
	/// <param name='_level'>
	/// _level: 0 means no division, 1 means first quads division, and so on in division levels
	/// </param>
	public InputTouchQuadTree (int _level) {
		level = _level;
	}
	
	/// <summary>
	/// According to the Rect screenBounds returns the list of listeners that it affects.
	/// It can be a max of four lists.
	/// </summary>
	/// <param name='screenBounds'>
	/// Screen bounds.
	/// </param>
	public List<ListenerLists> add (Rect screenBounds) {
		
		List<ListenerLists> lists = new List<ListenerLists>(4);
		addRecursive(screenBounds, lists, 0,0, Screen.width,Screen.height);
		return lists;
	}
	
	private void addRecursive (Rect screenBounds, List<ListenerLists> lists, float x0, float y0, float x1, float y1) {
		
		// reached max level?
		if (level == DEEP_LEVEL_THRESHOLD) {
			// if content is null it means it wasn't added into the listeners list
			if (leafContent == null) {
				leafContent = new ListenerLists();
				lists.Add(leafContent);
			}
			// the caller object is responsible to allocate the listener instance in the many listeners list
			return;
		}
		
		/// Test four points of rect screenBounds to get the destiny quad/s. 
		/// It can falls in as much as four quads.
		///                 y1
		///     -----------   x1
		///    |  0  |  1  |
		///     -----------
		///    |  3  |  2  |
		/// y0  -----------
		///  x0
		/// 
		// use actual limits to calculate destiny quadrant/s
		int test = quadrantTest(screenBounds, x0,y0,x1,y1);
		
		/// depending on which quadrant/s it falls we need to allocate space for the 
		/// quad/s node and continue testing recursively
		if (((test >> 0) & 1) == 1) {
			if (topLeft == null)
				topLeft = new InputTouchQuadTree(level + 1);
			topLeft.addRecursive(screenBounds, lists, x0,y1/2f, x1/2f,y1);
		}
		if (((test >> 1) & 1) == 1) {
			if (topRight == null)
				topRight = new InputTouchQuadTree(level + 1);
			topRight.addRecursive(screenBounds, lists, x1/2f,y1/2f, x1,y1);
		}
		if (((test >> 2) & 1) == 1) {
			if (botRight == null)
				botRight = new InputTouchQuadTree(level + 1);
			botRight.addRecursive(screenBounds, lists, x1/2f,y0, x1,y1/2f);
		}
		if (((test >> 3) & 1) == 1) {
			if (botLeft == null)
				botLeft = new InputTouchQuadTree(level + 1);
			botLeft.addRecursive(screenBounds, lists, x0,y0, x1/2f,y1/2f);
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
		return traverseRecursive(screenPos, 0,0, Screen.width,Screen.height);
	}
	
	private ListenerLists traverseRecursive (Vector2 p, float x0, float y0, float x1, float y1) {
		// if is leaf then return the list of listeners
		if (leafContent != null)
			return leafContent;
		
		// search in quad tree the leaf containing the screen position
		
		if (p.x < x1/2f) {
			if (p.y > y1/2f)
				return topLeft.traverseRecursive(p, x0,y1/2f, x1/2f,y1);
			else
				return botLeft.traverseRecursive(p, x0,y0, x1/2f,y1/2f);
		}
		else {
			if (p.y > y1/2f)
				return topRight.traverseRecursive(p, x1/2f,y1/2f, x1,y1);
			else
				return botRight.traverseRecursive(p, x1/2f,y0, x1,y1/2f);
		}
	}
	
	public void clear () {
		// is it a leaf?
		if (leafContent != null)
			leafContent.clear();
		
		if (topLeft != null) topLeft.clear();
		if (topRight != null) topRight.clear();
		if (botRight != null) botRight.clear();
		if (botLeft != null) botLeft.clear();
	}
}
