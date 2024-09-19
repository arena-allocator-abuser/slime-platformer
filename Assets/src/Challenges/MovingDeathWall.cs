using UnityEngine;

public class MovingDeathWall : MonoBehaviour {
	const int   cameraZ   = -10;
	const float wallSpeed = 5.0f;

	Rigidbody2D wallRB;

	void Awake() {
		wallRB = GetComponent<Rigidbody2D>();
	}

	void FixedUpdate() {
		wallRB.linearVelocityX = wallSpeed;
	}
}
