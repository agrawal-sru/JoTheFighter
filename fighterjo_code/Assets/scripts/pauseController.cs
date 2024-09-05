using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pauseController : MonoBehaviour {

	public bool paused;
	public GameObject pauseScreen;

	void Start()
	{
		paused = false;
		pauseScreen.SetActive (false);
	}

	void Update()
	{
		if (Input.GetKeyDown (KeyCode.Escape)) {
			paused = !paused;
			changeTime ();
		}
	}
	void changeTime()
	{
		if (paused) {
			Time.timeScale = 0;
			pauseScreen.SetActive (true);
		} else {
			Time.timeScale = 1;
			pauseScreen.SetActive (false);
		}
	}


}
