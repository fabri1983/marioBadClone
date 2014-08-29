using UnityEngine;

/// <summary>
/// Automaticaly sets the "layers" property for all the ChipmunkShape shapes this game object contains. 
/// Chipmunk Physics concept of layers is: what layer collides with each other layer.
/// Also this class lets add other layers defined by the user, as in the Unity's layer concept, this way the 
/// game object can belong to several layers.
/// </summary>
public class CollisionLayersCP : MonoBehaviour {
	
	private const int LAYERS = 16; // set here what is the max layer slot you defined on Unity's Layers window
	
	// add from the Inspector all the layers you want this game object belongs to
	public int[] otherLayersBelongTo;
	
	public void Start(){
		// start from current game object's layer (the one seen in Inspector)
		uint currentMask = (uint)(1 << gameObject.layer);

		// adds to current mask all the other additional layers added by the user
		for (int i=0; i < otherLayersBelongTo.Length; ++i)
			currentMask |= (uint)(1 << otherLayersBelongTo[i]);
		
		// generate a collision mask, as per Chipmunk's layers definition, for game object's current mask
		uint mask = 0;

		/// For every layer this game object belongs: ask what other layer/s it collides with.
		/// When it collides then set the correct bit mask
		for (int i=0; i < LAYERS; ++i) {
			// if bit i isn't 1 then continue
			if (((currentMask >> i) & 1) != 1)
				continue;
			for (int j=0; j < LAYERS; ++j) {
				/// NOTE: - first 8 layers are built-in layers.
				///       - built-in layers without name aren't ignored whit other built-in layers
				///         hence you will see them as 1 in the matrix
				if (!Physics.GetIgnoreLayerCollision(i,j))
					mask |= (uint)(1 << j);
			}
		}
		
		// Chipmunk use ~(uint)0 as mask to hit with all shapes
		
		GameObjectTools.setLayerForShapes(gameObject, mask);
	}
}
