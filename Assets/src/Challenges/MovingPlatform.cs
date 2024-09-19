using UnityEngine;

public class MovingPlatform : MonoBehaviour {
	const float wallCheckRadius = 0.1f;
	Vector2 wallCheckSize = new Vector2(0.01f, 0.3f);

	public LayerMask whatIsWall;
	public Transform wallCheck;
	public float moveSpeed = -1.0f;

	Rigidbody2D platformRB;
	SpriteRenderer platformSpriteRenderer;

	void Awake() {
		platformRB = GetComponent<Rigidbody2D>();
		platformSpriteRenderer = GetComponent<SpriteRenderer>();
	}

	void Update() {
		if (Physics2D.OverlapBox(wallCheck.position, wallCheckSize, 0.0f, whatIsWall)) {
			moveSpeed *= -1.0f;
			transform.localScale = new Vector2(transform.localScale.x * -1.0f, transform.localScale.y);
			platformSpriteRenderer.flipX = !platformSpriteRenderer.flipX;
		}
	}

	void FixedUpdate() {
		platformRB.linearVelocityX = moveSpeed;
	}

	void OnDrawGizmos() {
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireCube(wallCheck.position, wallCheckSize);
	}
}
