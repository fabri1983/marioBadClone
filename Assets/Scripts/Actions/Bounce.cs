using UnityEngine;

[ExecuteInEditMode]
public class Bounce : MonoBehaviour {
	
	public bool canBounce   = false;
    public float vel        = 0.0f;     // vertical velocity
    public float spring     = 600.0f;   // spring rate of the object
    public float py         = 1.0f;     // vertical position
    public float mass       = 2.0f;     // objects mass
    public float timescale  = 1.0f;     // time multiplier
	public bool firstCollision = false;

    void Update () {
		updatePosition();
    }
	
	private void updatePosition() {
		
		if (!firstCollision)
			return;
		
		// simulate physic time with desire time scale
        float t = Time.fixedDeltaTime * timescale;
		// gravity force
        float fy = -9.81f * mass;
		// convert acceleration to velocity
        vel += fy * t;
		// convert velocity to position
        py += vel * t;
		
		// apply the vertical position to the transform
        Vector3 pos = transform.position;
        pos.y = py;
        transform.position = pos;
		
	}
	
	public void collision (Collision collision) {
		
		if (canBounce && !collision.transform.tag.Equals("Mario")) {
			firstCollision = true;
			// set spring force only if collision's normal is vector up (floor)
			if (collision.contacts[0].normal.y > 0.8f)
				vel = spring * Time.fixedDeltaTime * timescale;
			// damp velocity if collision's normal is down vector (ceiling)
			else if (collision.contacts[0].normal.y > -1f && collision.contacts[0].normal.y < -0.8f)
				vel = 0f;
			// restart bounce
			py = transform.position.y;
		}
	}
}
