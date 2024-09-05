using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyScript : MonoBehaviour {

	//objects this object needs to keep track of
	public GameObject enemy;

	//sprites
	public Sprite normalSprite;
	public Sprite punchingSprite;
	public Sprite kickingSprite;
	public Sprite walk1;
	public Sprite walk2;
	public Sprite crouchNormal;
	public Sprite crouchPunch;
	public Sprite crouchKick;

	//health
	public static float health = 100;

	//speed and jumping
	public float speed;
	public float max_x_speed;
	public float max_y_speed;
	public float jumpPower;
	public float strideDuration;

	//punching and kicking
	public float punchPower;
	public float punchReach;
	public float punchDuration;
	public float punchDamage;
	public float kickPower;
	public float kickReach;
	public float kickDuration;
	public float kickDamage;
	public float coolDownPeriod;

	//used for crouching
	public float crouchSize;

	//private objects
	private SpriteRenderer SprRen;
	private Rigidbody2D rb;
	private Rigidbody2D enemyrb;

	//status variables
	private float normalHeight;
	private bool grounded;
	private bool upright;
	private bool crouching;
	private bool facingRight;
	private bool gettingUp;
	private bool punching;
	private bool recumbent;
	private bool kicking;
	private bool readyToAttack;
	private float punchTimer;
	private float kickTimer;
	private float walkTimer;
	private float coolDownTimer;
	private bool readyToWalk;
	private float crouchTimer;

	//the level we are on
	private string level;

    public AudioSource source;


    // Use this for initialization
    void Start () {
        source = GetComponent<AudioSource>();

		//getting all of the components we need
		rb = GetComponent<Rigidbody2D>();
		SprRen = GetComponent<SpriteRenderer> ();
		enemyrb = enemy.GetComponent<Rigidbody2D>();
		normalHeight = transform.localScale.y; 

		//initial values for bools
		facingRight = true;
		readyToAttack = true;
		punching = false;
		kicking = false;

		//timers
		punchTimer = 0;
		kickTimer = 0;
		walkTimer = 0;
		coolDownTimer = 0;
		crouchTimer = 0;

		level = SceneManager.GetActiveScene().name;
	}

	// Update is called once per frame
	void FixedUpdate () {

		updateStatus ();
		showGraphic ();

        if (roundController.roundStarted)
        {
            ////////////////////////////////////////////////////////////////////////
			//AI for level 1 enemy
			if (level == "Level1") {

				if (Mathf.Abs (enemy.transform.position.x - transform.position.x) < punchReach)
					punch ();
				else if (Mathf.Abs (enemy.transform.position.x - transform.position.x) < punchReach)
					kick ();
				else if (enemy.transform.position.x < transform.position.x)
					goLeft ();
				else if (enemy.transform.position.x > transform.position.x)
					goRight ();

				if (enemy.transform.position.y > transform.position.y)
					jump ();

				if (recumbent)
					jump ();

				if (crouchTimer == 0) {
					if (enemy.transform.localScale.y < transform.localScale.y && Mathf.Abs (enemy.transform.position.x - transform.position.x) < kickReach) {
						crouch ();
						crouchTimer += Time.deltaTime;
					} 
				} else if (crouchTimer > 1f) {
					crouchTimer = 0;
					unCrouch ();
				} else
					crouchTimer += Time.deltaTime;

			}
			////////////////////////////////////////////////////////////////////////
			//AI for level 2 enemy
			else if (level == "Level2") {

				if (health > 50) {
					if (Mathf.Abs (enemy.transform.position.x - transform.position.x) < punchReach)
						punch ();
					else if (Mathf.Abs (enemy.transform.position.x - transform.position.x) < punchReach)
						kick ();
					else if (enemy.transform.position.x < transform.position.x)
						goLeft ();
					else if (enemy.transform.position.x > transform.position.x)
						goRight ();

					if (enemy.transform.position.y > transform.position.y)
						jump ();

					if (recumbent)
						jump ();

					if (crouchTimer == 0) {
						if (enemy.transform.localScale.y < transform.localScale.y && Mathf.Abs (enemy.transform.position.x - transform.position.x) < kickReach) {
							crouch ();
							crouchTimer += Time.deltaTime;
						} 
					} else if (crouchTimer > 1f) {
						crouchTimer = 0;
						unCrouch ();
					} else
						crouchTimer += Time.deltaTime;

				} 
				//low on health desperation move
				else {

					if (grounded) {

						if (!facingRight)
							flip ();

						transform.position = new Vector3 (enemy.transform.position.x, 50, 0);
						transform.up = Vector3.right;
						unCrouch ();

						rb.angularVelocity = 0;
						rb.velocity = Vector3.zero;

						grounded = false;
					}

					if (transform.position.y > 48) {
						transform.up = Vector3.right;
						rb.angularVelocity = 0;
						rb.velocity = new Vector3 (0, rb.velocity.y, 0);
					}

					punch ();

				}


			}
			////////////////////////////////////////////////////////////////////////
			//AI for level 3 enemy
			else if (level == "Level3") {

				//distance to player
				float distance = Mathf.Abs (enemy.transform.position.x - transform.position.x);

				if (recumbent) {
					jump ();
					punch ();
				}

				if (!grounded)
					punch ();

				//can't crouch your way through this one
				if ((Mathf.Abs (rb.velocity.x) > max_x_speed - 1) && 
					(enemy.transform.localScale.y < transform.localScale.y) &&
					(distance < 20)) {

					jump ();

					if(facingRight)
						rb.AddTorque (-1, ForceMode2D.Impulse);
					else
						rb.AddTorque (1, ForceMode2D.Impulse);

					if(Mathf.Abs (enemy.transform.position.x - transform.position.x) < 0.1)
						rb.velocity = new Vector3(0, rb.velocity.y, 0); 
				}

				if (distance < punchReach)
					punch ();
				else if (distance < kickReach)
					kick ();

				if (enemy.transform.position.x < transform.position.x)
					goLeft ();
				else if (enemy.transform.position.x > transform.position.x)
					goRight ();
				
			}
			////////////////////////////////////////////////////////////////////////

            enforceMaxSpeed();
        }

	}

	//updates variables about the character
	void updateStatus () {

		//death check
		if (health <= 0)
			Destroy (this);

		//if you can attack or not
		if (punching || kicking || coolDownTimer != 0)
			readyToAttack = false;
		else
			readyToAttack = true;

		//checking if upright
		if (transform.up.x > -0.1 && transform.up.x < 0.1 && transform.up.y > 0)
			upright = true;
		else
			upright = false;

		//check if character is recumbent
		if (grounded && (transform.up.x < -0.9 || transform.up.x > 0.9 || transform.up.y < 0))
			recumbent = true;
		else
			recumbent = false;

		//if you can walk or not
		if (recumbent || punching || kicking || !grounded || crouching)
			readyToWalk = false;
		else
			readyToWalk = true;

		//make sure character is facing right direction
		if (grounded && upright) {
			if (enemy.transform.position.x - transform.position.x > 0 && !facingRight)
				flip ();
			if (enemy.transform.position.x - transform.position.x < 0 && facingRight)
				flip ();
		}

		//sometimes Jo isn't grounded when he should be
		if (transform.position.y < 0.1)
			grounded = true;

	}

	void showGraphic() {

		//showng the right graphic based off of current status
		if (punching && !crouching)
			SprRen.sprite = punchingSprite;

		if (kicking && !crouching)
			SprRen.sprite = kickingSprite;

		if (punching && crouching)
			SprRen.sprite = crouchPunch;

		if (kicking && crouching)
			SprRen.sprite = crouchKick;

		if (crouching && !kicking && !punching)
			SprRen.sprite = crouchNormal;

		//managing punches and kicks
		if (punching)
			punchTimer += Time.deltaTime;
		if (kicking)
			kickTimer += Time.deltaTime;

		if (punchTimer > punchDuration) {
			punchTimer = 0;
			punching = false;
			coolDownTimer += Time.deltaTime;
		}

		if (kickTimer > kickDuration) {
			kickTimer = 0;
			kicking = false;
			coolDownTimer += Time.deltaTime;
		}

		if (coolDownTimer > 0)
			coolDownTimer += Time.deltaTime;

		if (coolDownTimer > coolDownPeriod)
			coolDownTimer = 0;

		//not attacking
		if (!punching && !kicking) {

			//standing still
			if (rb.velocity.x == 0)
				SprRen.sprite = normalSprite;

			//should cycle between the stepping sprites
			else {

				//half second strided
				if (walkTimer < 0.5)
					SprRen.sprite = walk1;
				else if (walkTimer > 0.5)
					SprRen.sprite = walk2;

				if (walkTimer > 1.0)
					walkTimer = 0;

				walkTimer += Time.deltaTime;
			}

		}
	}

	void OnCollisionEnter2D(Collision2D other) { 

		//ground check
		if (other.collider.CompareTag("Ground"))
			grounded = true;

	} 

	void OnCollisionExit2D(Collision2D other){
		if (other.collider.CompareTag ("Ground"))
			grounded = false;
	}

	void jump() {

		//checks if you can jump and then jumps
		if (grounded && upright && !crouching && !punching && !kicking) {

			rb.AddForce (transform.up * jumpPower, ForceMode2D.Impulse);

		}

		//special jump for when you are recumbent
		if (grounded && recumbent) {

			rb.AddForce (Vector3.up * jumpPower, ForceMode2D.Impulse);
			transform.up = Vector3.up;
			rb.angularVelocity = 0;

		}


	}

	void goLeft() {

		if (readyToWalk)
			rb.AddForce (transform.right * speed * -1, ForceMode2D.Impulse);

	}

	void goRight() {

		if(readyToWalk)
			rb.AddForce (transform.right * speed, ForceMode2D.Impulse);

	}

	void crouch() {
		if (grounded && upright && !crouching && !punching && !kicking) {

			crouching = true;
			transform.localScale = new Vector3 (transform.localScale.x, crouchSize, 0);
		}
	}

	void unCrouch() {

		if (crouching) {

			transform.localScale = new Vector3 (transform.localScale.x, normalHeight, 0);
			crouching = false;
		}
	}

	void flip() {
		// Switch the way the player is labelled as facing
		facingRight = !facingRight;

		// Multiply the player's x local scale by -1
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

	void punch() {

		if(readyToAttack) {

			punching = true;

            Vector3 start = transform.position + transform.up;
			Vector3 end;

			if(facingRight)
				end = start + punchReach * transform.right;
			else
				end = start + punchReach * -transform.right;

			Vector2 punchDir = end - start;

			if (Physics2D.Linecast (start, end)) {
				enemyrb.AddForceAtPosition(punchDir.normalized * punchPower, start);
				source.Play();
				JoScript.takeDamage(punchDamage);
			}

		}

	}

	void kick() {

		if(readyToAttack) {

			kicking = true;

            Vector3 start = transform.position + transform.up;
			Vector3 end;

			//special crouching kick
			if (crouching) {

				if (facingRight)
					end = start + kickReach * transform.right;
				else
					end = start + kickReach * -transform.right;

				Vector2 kickDir = end - start;

				if (Physics2D.Linecast (start, end)) {
					enemyrb.AddForceAtPosition (kickDir.normalized * kickPower, start);
					source.Play();
					JoScript.takeDamage(kickDamage);
				}

			} 
			//normal standing upright kick
			else {

				if (facingRight)
					end = start + kickReach * (transform.right + transform.up);
				else
					end = start + kickReach * (-transform.right + transform.up);

				Vector2 kickDir = end - start;

				if (Physics2D.Linecast (start, end)) {
					enemyrb.AddForceAtPosition (kickDir.normalized * kickPower, start);
					source.Play();
					JoScript.takeDamage(kickDamage);
				}
			}

		}
	}

	void enforceMaxSpeed() {

		//reduces horizontal speed
		if (rb.velocity.x > max_x_speed || rb.velocity.x < -max_x_speed)
			rb.velocity = new Vector2 (rb.velocity.x.CompareTo(0) * max_x_speed, rb.velocity.y);

		//reduces vertical speed
		if (rb.velocity.y > max_y_speed)
			rb.velocity = new Vector2 (rb.velocity.x, max_y_speed);
	}

	public static void takeDamage(float damage) {

		health -= damage;

	}

}
