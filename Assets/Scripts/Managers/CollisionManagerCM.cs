using UnityEngine;
using System.Collections;

public class CollisionManagerCM : ChipmunkCollisionManager {

	protected void Start(){
		// Turning down the timestep can smooth things out significantly.
		// Chipmunk is also pretty fast so you don't need to worry about the performance so much.
		// Not really necessary, but helps in several subtle ways.
		// NOTE: doing this from Wake() will ignore base.Awake() method which is the one who adds this manager instance to chipmunk api
		Time.fixedDeltaTime = 1f/180f;
		Chipmunk.gravity = new Vector2(0f, -100f);
		
		Chipmunk.solverIterationCount = 3;
	}
	
	bool ChipmunkBegin_Goomba_Scenery (ChipmunkArbiter arbiter){
		return Patrol.beginCollision(arbiter);
	}
	
	bool ChipmunkBegin_Goomba_Goomba (ChipmunkArbiter arbiter){
		return Patrol.beginCollision(arbiter);
	}
	
	bool ChipmunkBegin_Goomba_PowerUp (ChipmunkArbiter arbiter){
		return Goomba.beginCollisionWithPowerUp(arbiter);
	}
	
	bool ChipmunkBegin_Goomba_Player (ChipmunkArbiter arbiter){
		return Goomba.beginCollisionWithPlayer(arbiter);
	}
	
	bool ChipmunkBegin_Player_Scenery (ChipmunkArbiter arbiter){
		if (!Player.beginCollisionWithScenery(arbiter))
			return false;
		return Jump.beginCollisionWithScenery(arbiter);
	}
	
	void ChipmunkSeparate_Player_Scenery (ChipmunkArbiter arbiter){
		Player.endCollisionWithScenery(arbiter);
		Jump.endCollision(arbiter);
	}
}
