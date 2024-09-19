using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour {
	public float moveSpeed = 0.3f;
	public float jumpSpeed = 7.0f;
	public float attackHopSpeed = 5.5f;
	public float dashSpeed = 11.0f;
	public float playerAntiFriction = 0.98f;
	const float attackCheckCircleRadius = 0.1f;
	const float damageCoolDown = 0.5f; // Immunity length
	Vector2 groundBoxSize = new Vector2(0.8f, 0.2f);
	//Vector2 wallBoxSize   = new Vector2(0.2f, 0.8f);
	Vector2 attackCheckSize = new Vector2(0.6f, 0.15f);
	const string deathTag       = "Death";
	const string enemyTag       = "Enemy";
	const string winTag         = "Win";
	const string interactionTag = "Interaction";
	public float dieWhenBelowYLevel = -10.0f;
	Color hasDashColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
	Color noDashColor  = new Color(0.0f, 1.0f, 1.0f, 1.0f);

	int playerHealth = 1;

	//public Transform wallCheck;
	public Transform groundCheck;
	public Canvas winScreen;

	public Sprite airPlayerSprite;
	public Sprite groundPlayerSprite;

	InputActions input;

	Rigidbody2D playerRB;
	SpriteRenderer playerSpriteRenderer;
	LayerMask whatIsGround;
	LayerMask whatIsEnemy;
	float damageCoolDownTimer = 0.0f;
	bool playerIsGrounded = false;
	bool hasDoubleJump = false;
	bool hasDash = false;
	bool shouldDash = false;

	void Awake() {
		input                = GetComponent<InputActions>();
		playerRB             = GetComponent<Rigidbody2D>();
		playerSpriteRenderer = GetComponent<SpriteRenderer>();

		whatIsGround = LayerMask.GetMask("Ground");
		whatIsEnemy  = LayerMask.GetMask("Enemy");

		winScreen.enabled = false;
	}

	void Update() {
		playerIsGrounded = Physics2D.OverlapBox(groundCheck.position, groundBoxSize, 0.0f, whatIsGround);
		//playerIsUpAgainstWall = Physics2D.OverlapBox(wallCheck.position,   wallBoxSize,   0.0f, whatIsGround);

		if (input.jump) {
			if (playerIsGrounded) {
				playerRB.linearVelocityY = jumpSpeed;
			} else if (hasDoubleJump) {
				playerRB.linearVelocityY = jumpSpeed;
				hasDoubleJump = false;
			}
		}

		if (playerIsGrounded) {
			hasDash = true;
			hasDoubleJump = true;

			playerSpriteRenderer.color = hasDashColor;

			playerSpriteRenderer.sprite = groundPlayerSprite;
		} else {
			playerSpriteRenderer.sprite = airPlayerSprite;
		}

		if (input.dash && hasDash) {
			shouldDash = true;
		}

		Attack();

		if (transform.position.y < dieWhenBelowYLevel) {
			Die();
		}

		// TODO: make the player look at the camera when standing still
#if false
		if (playerRB.linearVelocityX == 0.0f) {
		}
#endif

		if (playerRB.linearVelocityX > 0.0f) {
			playerSpriteRenderer.flipX = false;
		} else if (playerRB.linearVelocityX < 0.0f) {
			playerSpriteRenderer.flipX = true;
		}
	}

	void Dash() {
		// TODO: Controllers give input.movements that are not 0 or 1. This
		// means only controller can do a partial dash.
		playerRB.linearVelocityX += input.movement.x * dashSpeed;
		playerRB.linearVelocityY += input.movement.y * dashSpeed;

		playerSpriteRenderer.color = noDashColor;

		hasDash = false;
	}

	void FixedUpdate() {
		playerRB.linearVelocityX += input.movement.x * moveSpeed;
		playerRB.linearVelocityX *= playerAntiFriction;

		if (shouldDash) {
			shouldDash = false;
			Dash();
		}

		print(playerRB.linearVelocity);
	}

	void Attack() {
		//if (!Physics2D.OverlapCircle(groundCheck.position, attackCheckCircleRadius, whatIsEnemy)) {
		if (!Physics2D.OverlapBox(groundCheck.position, attackCheckSize, 0.0f, whatIsEnemy)) {
			return;
		}

		//var enemyColliders = Physics2D.OverlapCircleAll(groundCheck.position, attackCheckCircleRadius, whatIsEnemy);
		var enemyColliders = Physics2D.OverlapBoxAll(groundCheck.position, attackCheckSize, 0.0f, whatIsEnemy);

		foreach (var collider in enemyColliders) {
			Destroy(collider.gameObject);
		}

		if (enemyColliders.Length > 0) {
			hasDoubleJump = true;
			hasDash = true;
		}

		playerRB.linearVelocityY = attackHopSpeed;
	}

	void Die() {
		RestartScene();
	}

	void Win() {
		winScreen.enabled = true;
		Time.timeScale = 0.0f;
	}

	// 'opacity' should be a value between 0.0f and 1.0f.
	void SetSpriteRendererOpacity(SpriteRenderer renderer, float opacity) {
		var color = renderer.color;
		color.a = opacity;
		renderer.color = color;
	}

	void TakeDamage() {
		if (Time.time > damageCoolDownTimer) {
			playerHealth -= 1;
			damageCoolDownTimer = Time.time + damageCoolDown;
		}

		if (playerHealth <= 0) {
			Die();
		}
	}

	void RestartScene() {
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}

	void OnCollisionEnter2D(Collision2D other) {
		if (other.gameObject.CompareTag(deathTag)) {
			Die();
		}
	}

	void OnTriggerEnter2D(Collider2D other) {
		// For moving death wall.
		if (other.gameObject.CompareTag(deathTag)) {
			Die();
		} else if (other.gameObject.CompareTag(winTag)) {
			Win();
		}
	}

	void OnCollisionStay2D(Collision2D other) {
		if (other.gameObject.CompareTag(enemyTag)) {
			TakeDamage();
		} /* else if (other.gameObject.CompareTag(interactionTag)) {
			// TODO: Make player follow platform.
			playerRB.linearVelocityX += other.rigidbody.linearVelocityX;
			playerRB.linearVelocityY += other.rigidbody.linearVelocityY;
		} */
	}

	void OnDrawGizmos() {
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireCube(groundCheck.position, groundBoxSize);

		Gizmos.color = Color.red;
		//Gizmos.DrawWireSphere(groundCheck.position, attackCheckCircleRadius);
		Gizmos.DrawWireCube(groundCheck.position, attackCheckSize);
	}
}
