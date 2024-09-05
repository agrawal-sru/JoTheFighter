using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camera : MonoBehaviour {

	public GameObject player;
	public GameObject enemy;
	public GameObject leftWall;
	public GameObject rightWall;

	private float halfWidth;

	// Use this for initialization
	void Start () {

		halfWidth = Camera.main.orthographicSize * Screen.width / Screen.height;

	}
	
	// Update is called once per frame
	void Update () {

		halfWidth = Camera.main.orthographicSize * Screen.width / Screen.height;

		float newPos = (player.transform.position.x + enemy.transform.position.x) / 2f;

		//makes sure player is always visible
		if (newPos > player.transform.position.x + (halfWidth * 2f))
			newPos = player.transform.position.x + (halfWidth / 2f);
		if (newPos < player.transform.position.x - (halfWidth / 2f))
				newPos = player.transform.position.x - (halfWidth / 2f);

		//makes sure camera does not show end of world
		if (newPos < leftWall.transform.position.x + halfWidth)
			newPos = leftWall.transform.position.x + halfWidth;
		if (newPos > rightWall.transform.position.x - halfWidth)
			newPos = rightWall.transform.position.x - halfWidth;

		transform.position = new Vector3(newPos, transform.position.y, transform.position.z);

	}
}
