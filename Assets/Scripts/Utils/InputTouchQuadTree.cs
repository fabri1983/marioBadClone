using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Quad Tree for input touch listeners. Considers on screen positions.
/// </summary>
public class InputTouchQuadTree {
	
	private const int DEEP_LEVEL_THRESHOLD = 0;
	
	// this is the object to be saved in the Quad Tree's leaves
	public ListenerLists content;
	
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
		
		// reached max level?
		if (level == DEEP_LEVEL_THRESHOLD) {
			// allocate space for the listener lists
			if (content == null) {
				content = new ListenerLists();
				lists.Add (content);
			}
			// the caller object is responsible to allocate the listener instance in the correct listener list
			return lists;
		}
		
		/// test four points of rect screenBounds to get the destiny quad/s. It can falls in four regions
		///     -----------
		///    |  0  |  1  |
		///     -----------
		///    |  3  |  2  |
		/// Y   -----------
		///  X
		
		// use actual level+1 or whatever to calculate fallen quad
		int test = 0;
		// depending on which quadrant/s it falls we need to allocate space for the quad/s tree node
		if (((test >> 0) & 1) == 1) {
			topLeft = new InputTouchQuadTree(level + 1);
			lists.Add(topLeft.addRecursive(screenBounds.xMin, screenBounds.yMax));
		}
		if (((test >> 1) & 1) == 1) {
			topRight = new InputTouchQuadTree(level + 1);
			lists.Add(topRight.addRecursive(screenBounds.xMax, screenBounds.yMax));
		}
		if (((test >> 2) & 1) == 1) {
			botRight = new InputTouchQuadTree(level + 1);
			lists.Add(botRight.addRecursive(screenBounds.xMax, screenBounds.yMin));
		}
		if (((test >> 3) & 1) == 1) {
			botLeft = new InputTouchQuadTree(level + 1);
			lists.Add(botLeft.addRecursive(screenBounds.xMin, screenBounds.yMin));
		}
		
		return lists;
	}
	
	private ListenerLists addRecursive (float x, float y) {
		
		// reached max level?
		if (level == DEEP_LEVEL_THRESHOLD) {
			// allocate space for the listener lists
			if (content == null)
				content = new ListenerLists();
			// the caller object is responsible to allocate the listener instance in the correct listener list
			return content;
		}
		
		// use actual level+1 or whatever to calculate fallen quad
		int test = 0;
		if (((test >> 0) & 1) == 1) {
			topLeft = new InputTouchQuadTree(level + 1);
			return topLeft.addRecursive(x, y);
		}
		else if (((test >> 1) & 1) == 1) {
			topRight = new InputTouchQuadTree(level + 1);
			return topRight.addRecursive(x, y);
		}
		else if (((test >> 2) & 1) == 1) {
			botRight = new InputTouchQuadTree(level + 1);
			return botRight.addRecursive(x, y);
		}
		
		botLeft = new InputTouchQuadTree(level + 1);
		return botLeft.addRecursive(x, y);
	}
	
	public ListenerLists getLists (Vector2 screenPos) {

		// if is leaf then return the list of listeners
		if (content != null)
			return content;
		
		// search in quad tree the leaf containing the screen position
		
		return null;
	}
	
	public void clear () {
		// traverse all listener lists and call clear()
	}
}
