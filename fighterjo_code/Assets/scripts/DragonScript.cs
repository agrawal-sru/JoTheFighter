using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonScript : MonoBehaviour {

	//game objects the dragon has at its disposal
	public GameObject fire;
	public GameObject scrub;
	public float fireSpeed;

	//hidden secrets of the dragon
	private Rigidbody2D firerb;
	private bool fighting;
	private AudioSource source; 

	// Use this for initialization
	void Start () {

		firerb = fire.GetComponent<Rigidbody2D> ();
		firerb.angularVelocity = -fireSpeed;
		fighting = false;
		source = GetComponent<AudioSource> ();
		source.Pause ();
		
	}
	
	// Update is called once per frame
	void Update () {

		//player is about to win
		if (roundController.playerWins > 1 && roundController.roundStarted && fighting == false 
			&& JoScript.health == 100 && EnemyScript.health == 100) {
			
			//move the scrub out of the way
			scrub.transform.position = new Vector3 (10, 200, 0);
			scrub.GetComponent<Rigidbody2D> ().simulated = false;

			//moving in
			transform.position = new Vector3 (10, 16, 0);
			fire.transform.position = new Vector3 (10, 26, 0);
			fire.transform.SetPositionAndRotation(
				new Vector3 (10, 26, 0),
				Quaternion.Euler (0, 0, 150) );

			//getting down to business
			fighting = true;

			//plays dragon noises
			source.Play();

		}

		if (fighting) {

			//moves the fire up and down
			if (fire.transform.up.y < -0.8)
				firerb.angularVelocity = -fireSpeed;

			if (fire.transform.up.y > 0.2)
				firerb.angularVelocity = fireSpeed;

			if (EnemyScript.health <= 0) {

				Destroy (fire);
				Destroy (this);

			}
		}
		
	}

}
