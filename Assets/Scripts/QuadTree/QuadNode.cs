
public struct QuadNode {
	
	public int level;
	public QuadNodeMutable topLeft, topRight, botLeft, botRight;
	public ListenerLists leafContent;
	
	public void clear () {
		// is it a leaf?
		if (level == QuadTreeTouchEvent.DEEP_LEVEL_THRESHOLD) {
			leafContent.clear();
			return;
		}
		
		// while not being a leaf then continue cleaning quad nodes
		if (topLeft != null) topLeft.s.clear();
		if (topRight != null) topRight.s.clear();
		if (botRight != null) botRight.s.clear();
		if (botLeft != null) botLeft.s.clear();
	}
}

/// <summary>
/// This class to break struct cycle layout
/// </summary>
public sealed class QuadNodeMutable {
	public QuadNode s;
	
	public QuadNodeMutable(int _level) {
		s.level = _level;
	}
}
