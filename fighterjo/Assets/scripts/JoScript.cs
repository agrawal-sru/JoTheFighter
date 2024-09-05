using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoScript : MonoBehaviour {

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
    }

	// Update is called once per frame
	void FixedUpdate () {

		updateStatus ();
		showGraphic ();
        if (roundController.roundStarted)
        {
            if (Input.GetKey(KeyCode.UpArrow))
                jump();

            if (Input.GetKey(KeyCode.LeftArrow))
                goLeft();

            if (Input.GetKey(KeyCode.RightArrow))
                goRight();

            if (Input.GetKey(KeyCode.DownArrow))
                crouch();
            else
                unCrouch();

            if (Input.GetKey(KeyCode.Z))
                punch();

            if (Input.GetKey(KeyCode.X))
                kick();

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
		if (other.collider.CompareTag ("Ground"))
			grounded = true;

	}

	void OnCollisionExit2D(Collision2D other){
		if (other.collider.CompareTag ("Ground"))
			grounded = false;
	}

	void OnTriggerEnter2D(Collider2D other) { 

		//begin fire damage
		InvokeRepeating("getBurned", 0, 1);

	}

	void OnTriggerExit2D() { 

		//end fire damage
		CancelInvoke("getBurned");

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
				EnemyScript.takeDamage(punchDamage);
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
					EnemyScript.takeDamage(kickDamage);
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
					EnemyScript.takeDamage(kickDamage);
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

	public void getBurned() {

		health -= 30;

	}

    public static void takeDamage(float damage)
    {
        health -= damage;
        
    }
}
