// Copyright 2013 Howling Moon Software. All rights reserved.
// See http://chipmunk2d.net/legal.php for more information.

using UnityEngine;
using System.Collections;

/// Internal Chipmunk class used to implement the physic step logic.
public class ChipmunkManager : MonoBehaviour {
	public ChipmunkSpace _space;
	
	protected void OnDestroy(){
		Chipmunk._manager = null;
	}
	
	protected void FixedUpdate(){
		_space._Step(Time.fixedDeltaTime);	
		
		for (int i=0,c=_space.bodies.Count; i<c; ++i){
			ChipmunkBody b = _space.bodies[i];
			
			//b.transform.position = (Vector3) b.position + (Vector3.forward * b._savedZ);
			// Next lines do the same than above line
			Vector3 thePos = b.transform.position;
			thePos.x = b.position.x;
			thePos.y = b.position.y;
			thePos.z = b._savedZ;
			b.transform.position = thePos;
			
			// next rotation operation seems to be the fastest, since it immediately executes internal call to engine api
			b.transform.rotation = Quaternion.AngleAxis(b.angle*Mathf.Rad2Deg, Vector3.forward);
		}
	}
}
