using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Quad Tree for input touch listeners. Considers on screen positions.
/// </summary>
public class InputTouchQuadTree {
	
	private const int DEEP_LEVEL_THRESHOLD = 0;
	
	// this is the object to be saved in the Quad Tree's leaves
	public ListenerLists quadContent;
	
	private int level;
	private InputTouchQuadTree topLeft, topRight, botLeft, botRight;
	
	/// <summary>
	/// Initializes a new instance of the <see cref="InputTouchQuadTree"/> class.
	/// </summary>
	/// <param name='_level'>
	/// _level: 0 means no division, 1 means first quads division, etc
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
			if (quadContent == null) {
				quadContent = new ListenerLists();
				lists.Add(quadContent);
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
	
	private static int quadrantTest (Rect screenBounds, float x0, float y0, float x1, float y1) {
		return 0;
	}
	
	public ListenerLists getLists (Vector2 screenPos) {

		// if is leaf then return the list of listeners
		if (quadContent != null)
			return quadContent;
		
		// search in quad tree the leaf containing the screen position
		
		return null;
	}
	
	public void clear () {
		// traverse all listener lists and call clear()
	}
}
