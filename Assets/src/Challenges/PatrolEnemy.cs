using UnityEngine;

public class PatrolEnemy : MonoBehaviour {
	const float fallCheckRadius = 0.1f;
	const float wallCheckRadius = 0.1f;

	public LayerMask whatIsWall;
	public Transform wallCheck;
	public Transform fallCheck;

	float moveSpeed = -1.0f;
	Rigidbody2D enemyRB;

	void Awake() {
		enemyRB = GetComponent<Rigidbody2D>();
	}

	void Update() {
		if (DetectedWallOrFall()) {
			moveSpeed *= -1.0f;
			transform.localScale = new Vector2(transform.localScale.x * -1.0f, 1.0f);
		}
	}

	void FixedUpdate() {
		enemyRB.linearVelocityX = moveSpeed;
	}


	bool DetectedWallOrFall() {
		return !Physics2D.OverlapCircle(fallCheck.position, fallCheckRadius, whatIsWall) ||
		        Physics2D.OverlapCircle(wallCheck.position, wallCheckRadius, whatIsWall);
	}

	void OnDrawGizmos() {
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(fallCheck.position, fallCheckRadius);
		Gizmos.DrawWireSphere(wallCheck.position, wallCheckRadius);
	}
}
