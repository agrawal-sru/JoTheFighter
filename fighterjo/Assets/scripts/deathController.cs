using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class deathController : MonoBehaviour {


	public GameObject KO;
	public bool waited = false;
	public int x;
	public Image healthbar_player;
	public Image healthbar_enemy;
	public bool roundOver = false;



	// Use this for initialization
	void Start () {
		KO.SetActive (false);
		roundOver = false;
		x = 0;
	}
	
	// Update is called once per frame
	void Update () {
		
			if (JoScript.health <= 0) {
                KO.SetActive (true);
            Destroy (healthbar_player, 0f);
                death ();
			}
			if (EnemyScript.health <= 0) {
                KO.SetActive (true);
				Destroy (healthbar_enemy, 0f);
                death ();
			}
		
				
	}

	private void death()
	{

		InvokeRepeating ("waitThreeSecs", 1, 1);
		if(waited && !roundOver)
		{
            roundOver = true;
            
            if (JoScript.health <= 0) 
				{
					roundController.enemyWins++;
				}
				if (EnemyScript.health <= 0) 
				{
					roundController.playerWins++;
				}
			roundController.roundNum++;
		}
	}

	void waitThreeSecs()
	{
		if (x < 3) {
			x++;
		} else {
			waited = true;
			CancelInvoke ("waitThreeSecs");
		}
	}

}
