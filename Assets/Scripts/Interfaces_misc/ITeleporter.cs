using UnityEngine;

/**
 * Defines the behavior a teleporter game object (trigger, collider, etc) must exposes.
 */
public interface ITeleporter
{
	Vector3 getDirEntrance ();
}

