using UnityEngine;

/// <summary>
/// Register script for ChipmunkCollisionManager callbacks.
/// Given the nature of the callback methods being static, there must be only one instance of this script per scene.
/// This script also adds the posibility to handle "while in collision" situation, a feature that CP does not provide.
/// </summary>
public class CollisionManagerCP : ChipmunkCollisionManager {
	
	void Start(){
		// NOTE: using Awake() will ignore base.Awake() method which is the 
		// one who adds this manager instance to chipmunk API.
		
		// Turning down the timestep can smooth things out significantly.
		// Chipmunk is also pretty fast so you don't need to worry about the performance so much.
		// Not really necessary, but helps in several subtle ways.
		// NOTE: set this in the Time Manager window
		// 0.0033 (1/300), 0.0055 (1/180), and 0.0083 (1/120), and 0.0111 (1/90) is OK. 
		// However lower it and see what happens. Is it OK to at least be 3 times your target
		// Also you need to modified the Maximum Allowed Timestep value in the Time Manager window. For example if you only
		// want to allow a max of 3 steps frame then set it to 1/3 = 0.3333
		//Time.fixedDeltaTime = 0.0111f;
		
		Chipmunk.gravity = new Vector2(0f, -100f);	
		Chipmunk.solverIterationCount = 3; // Unity's Physic default is 6
		
		/// The space.collisionSlop, which is set by Chipmunk.minPenetrationForPenalty, is how much objects are allowed to overlap. 
		/// This helps prevent cached contact values from disappearing because objects stay overlapping instead of 
		/// being pushed apart one frame then coming into contact again the next. 
		/// With a collision slop of 0, shapes quite often stop touching by a very small amount when the solver moves 
		/// overlapping shapes apart. This can cause all sorts of problems from poor solver performance to getting a 
		/// flood of begin/separate collision events. Increasing the collision slop greatly reduces the chances of 
		/// objects getting pushed completely apart.
		/// Default is 0.01f
		//Chipmunk.minPenetrationForPenalty = 0.1f;
		
		/// Rate at which overlapping objects are pushed apart.
		/// Defaults to Mathf.Pow(0.9f, 60f) = 0.001797007f, meaning it will fix 10% of overlap per 1/60th second.
		//Chipmunk.collisionBias = 1f; // a value of 1 seems to cancel the push apart.
	}
	
	//##################### Goomba #################
	bool ChipmunkBegin_Goomba_Scenery (ChipmunkArbiter arbiter) {
		return Patrol.beginCollisionWithAny(arbiter);
	}
	
	bool ChipmunkBegin_Goomba_Goomba (ChipmunkArbiter arbiter) {
		return Patrol.beginCollisionWithAny(arbiter);
	}
	
	bool ChipmunkBegin_Goomba_Ghost (ChipmunkArbiter arbiter) {
		return Patrol.beginCollisionWithAny(arbiter);
	}
	
	bool ChipmunkBegin_Goomba_PowerUp (ChipmunkArbiter arbiter) {
		return Goomba.beginCollisionWithPowerUp(arbiter);
	}
	
	bool ChipmunkBegin_Goomba_Player (ChipmunkArbiter arbiter) {
		return Goomba.beginCollisionWithPlayer(arbiter);
	}
	
	//##################### Ghost #################
	bool ChipmunkBegin_Ghost_PowerUp (ChipmunkArbiter arbiter) {
		return Ghost.beginCollisionWithPowerUp(arbiter);
	}
	
	bool ChipmunkBegin_Ghost_Player (ChipmunkArbiter arbiter) {
		return Ghost.beginCollisionWithPlayer(arbiter);
	}

	bool ChipmunkBegin_Ghost_Oneway (ChipmunkArbiter arbiter) {
		// Ghost does not collide with oneway platform
		return false;
	}

	//##################### KoopaTroopa #################
	bool ChipmunkBegin_KoopaTroopa_Scenery (ChipmunkArbiter arbiter) {
		Chase.beginCollisionWithAny(arbiter);
		if (!Patrol.beginCollisionWithAny(arbiter))
			return false;
		return Jump.beginCollisionWithAny(arbiter);
	}
	
	bool ChipmunkBegin_KoopaTroopa_Oneway (ChipmunkArbiter arbiter) {
		Chase.beginCollisionWithAny(arbiter);
		if (!Patrol.beginCollisionWithAny(arbiter))
			return false;
		return Jump.beginCollisionWithAny(arbiter);
	}
	
	bool ChipmunkBegin_KoopaTroopa_Goomba (ChipmunkArbiter arbiter) {
		Chase.beginCollisionWithAny(arbiter);
		if (!Patrol.beginCollisionWithAny(arbiter))
			return false;
		return Jump.beginCollisionWithAny(arbiter);
	}
	
	bool ChipmunkBegin_KoopaTroopa_KoopaTroopa (ChipmunkArbiter arbiter) {
		if (!KoopaTroopa.beginCollisionWithKoopaTroopa(arbiter))
			return false;
		return Patrol.beginCollisionWithAny(arbiter);
	}
	
	bool ChipmunkBegin_KoopaTroopa_PowerUp (ChipmunkArbiter arbiter) {
		return KoopaTroopa.beginCollisionWithPowerUp(arbiter);
	}
	
	bool ChipmunkBegin_KoopaTroopa_Player (ChipmunkArbiter arbiter) {
		return KoopaTroopa.beginCollisionWithPlayer(arbiter);
	}
	
	//##################### ChaseSensor #################
	bool ChipmunkBegin_ChaseSensor_Player (ChipmunkArbiter arbiter) {
		return Chase.beginCollisionWithPlayer(arbiter);
	}
	
	void ChipmunkSeparate_ChaseSensor_Player (ChipmunkArbiter arbiter) {
		Chase.endCollisionWithPlayer(arbiter);
	}
	
	//##################### Player #################
	bool ChipmunkBegin_Player_Scenery (ChipmunkArbiter arbiter) {
		if (!Player.beginCollisionWithScenery(arbiter))
			return false;
		return Jump.beginCollisionWithAny(arbiter);
	}
	
	void ChipmunkSeparate_Player_Scenery (ChipmunkArbiter arbiter) {
		Player.endCollisionWithScenery(arbiter);
	}
	
	bool ChipmunkBegin_Player_SpawnPos (ChipmunkArbiter arbiter) {
		return SpawnPositionTrigger.beginCollisionWithPlayer(arbiter);
	}

	bool ChipmunkBegin_Player_Oneway (ChipmunkArbiter arbiter) {
		// if condition for oneway platform was not met then proceed as a begin collision with Scenery
		if (!Player.beginCollisionWithOneway(arbiter)) {
			return ChipmunkBegin_Player_Scenery(arbiter);
		}
		return true;
	}

	bool ChipmunkPreSolve_Player_Oneway (ChipmunkArbiter arbiter) {
		return Player.presolveCollisionWithOneway(arbiter);
	}

	void ChipmunkSeparate_Player_Oneway (ChipmunkArbiter arbiter) {
		Player.endCollisionWithOneway(arbiter);
		// proceed as separating from Scenery
		//ChipmunkSeparate_Player_Scenery(arbiter);
	}
	
	bool ChipmunkBegin_Goal_Player (ChipmunkArbiter arbiter) {
		return Goal.beginCollisionWithPlayer(arbiter);
	}
	
	void ChipmunkSeparate_Goal_Player (ChipmunkArbiter arbiter) {
		Goal.endCollisionWithPlayer(arbiter);
	}
}
