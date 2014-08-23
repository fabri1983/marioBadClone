using UnityEngine;
using System.Collections;

public class CollisionManagerCP : ChipmunkCollisionManager {

	void Start(){
		// NOTE: applying changes from Wake() will ignore base.Awake() method which is the 
		// one who adds this manager instance to chipmunk API. So apply changes on Start().
		
		// Turning down the timestep can smooth things out significantly.
		// Chipmunk is also pretty fast so you don't need to worry about the performance so much.
		// Not really necessary, but helps in several subtle ways.
		// NOTE: do this in the Time Manager window
		//Time.fixedDeltaTime = 0.01f; // 0.0033 (1f/300f), 0.0055 (1f/180f), and 0.0083 (1f/120f) is OK. However lower it and see what happens
		
		Chipmunk.gravity = new Vector2(0f, -100f);	
		Chipmunk.solverIterationCount = 3; // Unity's Physic default is 6
	}
	
	//##################### Goomba #################
	bool ChipmunkBegin_Scenery_Goomba (ChipmunkArbiter arbiter){
		return Patrol.beginCollision(arbiter);
	}
	
	bool ChipmunkBegin_Goomba_Goomba (ChipmunkArbiter arbiter){
		return Patrol.beginCollision(arbiter);
	}
	
	bool ChipmunkBegin_Goomba_Ghost (ChipmunkArbiter arbiter){
		return Patrol.beginCollision(arbiter);
	}
	
	bool ChipmunkBegin_Goomba_KoopaTroopa (ChipmunkArbiter arbiter){
		return Patrol.beginCollision(arbiter);
	}
	
	bool ChipmunkBegin_Goomba_PowerUp (ChipmunkArbiter arbiter){
		return Goomba.beginCollisionWithPowerUp(arbiter);
	}
	
	bool ChipmunkBegin_Goomba_Player (ChipmunkArbiter arbiter){
		return Goomba.beginCollisionWithPlayer(arbiter);
	}
	
	//##################### Ghost #################
	bool ChipmunkBegin_Ghost_PowerUp (ChipmunkArbiter arbiter){
		return Ghost.beginCollisionWithPowerUp(arbiter);
	}
	
	bool ChipmunkBegin_Ghost_Player (ChipmunkArbiter arbiter){
		return Ghost.beginCollisionWithPlayer(arbiter);
	}
	
	//##################### KoopaTroopa #################
	bool ChipmunkBegin_Scenery_KoopaTroopa (ChipmunkArbiter arbiter){
		Chase.beginCollisionWithScenery(arbiter);
		if (!Patrol.beginCollision(arbiter))
			return false;
		return Jump.beginCollisionWithScenery(arbiter);
	}
	
	bool ChipmunkBegin_KoopaTroopa_KoopaTroopa (ChipmunkArbiter arbiter){
		if (!KoopaTroopa.beginCollisionWithKoopaTroopa(arbiter))
			return false;
		return Patrol.beginCollision(arbiter);
	}
	
	bool ChipmunkBegin_KoopaTroopa_PowerUp (ChipmunkArbiter arbiter){
		return KoopaTroopa.beginCollisionWithPowerUp(arbiter);
	}
	
	bool ChipmunkBegin_KoopaTroopa_Player (ChipmunkArbiter arbiter){
		return KoopaTroopa.beginCollisionWithPlayer(arbiter);
	}
	
	//##################### ChaseSensor #################
	bool ChipmunkBegin_ChaseSensor_Player (ChipmunkArbiter arbiter){
		return Chase.beginCollisionWithPlayer(arbiter);
	}
	
	void ChipmunkSeparate_ChaseSensor_Player (ChipmunkArbiter arbiter){
		Chase.endCollisionWithPlayer(arbiter);
	}
	
	//##################### Player #################
	bool ChipmunkBegin_Scenery_Player (ChipmunkArbiter arbiter){
		if (!Player.beginCollisionWithScenery(arbiter))
			return false;
		return Jump.beginCollisionWithScenery(arbiter);
	}
	
	void ChipmunkSeparate_Scenery_Player (ChipmunkArbiter arbiter){
		Player.endCollisionWithScenery(arbiter);
		Jump.endCollisionWithScenery(arbiter);
	}
	
	bool ChipmunkBegin_Player_SpawnPos (ChipmunkArbiter arbiter){
		return SpawnPositionTrigger.beginCollisionWithPlayer(arbiter);
	}
	
	bool ChipmunkBegin_Player_UnlockSensor (ChipmunkArbiter arbiter){
		return Player.beginCollisionWithUnlockSensor(arbiter);
	}
}
