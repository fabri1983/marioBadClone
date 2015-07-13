
public class QuadNode {
	
	public int level;
	public QuadNode topLeft, topRight, botLeft, botRight;
	public ListenerLists leafContent;
	
	public QuadNode(int _level) {
		level = _level;
	}
	
	public void clear () {
		// is it a leaf?
		if (level == QuadTreeTouchEvent.DEEP_LEVEL_THRESHOLD) {
			leafContent.clear();
			return;
		}
		
		// while not being a leaf then continue cleaning quad nodes
		if (topLeft != null) topLeft.clear();
		if (topRight != null) topRight.clear();
		if (botRight != null) botRight.clear();
		if (botLeft != null) botLeft.clear();
	}
}
