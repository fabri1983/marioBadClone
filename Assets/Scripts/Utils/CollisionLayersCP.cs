using UnityEngine;

/// <summary>
/// Automaticaly sets the "layers" property for all the ChipmunkShape shapes this game object contains. 
/// Chipmunk Physics concept of layers is: what layer collides with each other layer.
/// Also this class lets add other layers defined by the user, as in the Unity's layer concept, this way the 
/// game object can belong to several layers.
/// </summary>
public class CollisionLayersCP : MonoBehaviour {
	
	// add from the Inspector all the layers you want this game object belongs to
	public int[] otherLayersBelongTo;
	
	public void Start ()
	{		
		// start from current game object's layer (the one seen in Inspector)
		uint currentMask = (uint)(1 << gameObject.layer);

		// adds to current mask all the other additional layers added by the user
		for (int i=0; i < otherLayersBelongTo.Length; ++i)
			currentMask |= (uint)(1 << otherLayersBelongTo[i]);
		
		// generate a collision mask, as per Chipmunk's layers definition, for game object's current mask
		uint mask = 0;
		
		//what is the max valid layer slot you defined on Unity's Layers window
		int unityLayersCount = CountLayers.countUnitysLayers();
		
		/// For every layer this game object belongs: ask what other layer/s it collides with.
		/// When it collides then set the correct bit mask
		for (int i=0; i < unityLayersCount; ++i) {
			// if bit i isn't 1 then continue
			if (((currentMask >> i) & 1) != 1)
				continue;
			for (int j=0; j < unityLayersCount; ++j) {
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

/// <summary>
/// This static class only calculates which is the max valid layer slot you defined on Unity's Layers window.
/// Once the value is calculated then it's cached.
/// </summary>
static class CountLayers {
	private static int unitys_layers = 0;
	
	public static int countUnitysLayers ()
	{
		if (unitys_layers != 0)
			return unitys_layers;
		
		// start testing from last layer
		for (int i=31; i >= 0; --i) {
			string name = LayerMask.LayerToName(i);
			if (!"".Equals(name)) {
				unitys_layers = i + 1;
				break;
			}
		}
		
		return unitys_layers;
	}
}