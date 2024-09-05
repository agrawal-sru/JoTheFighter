using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HealthBarScript : MonoBehaviour {

	public Image healthbar_player;
	public Image healthbar_enemy;


	// Use this for initialization
	void Start () {

		JoScript.health = 100;
		EnemyScript.health = 100;


	}

	void Update()
	{
		checkHealth ();
	}

	void checkHealth()
	{
		if (JoScript.health > 0) {
			healthbar_player.rectTransform.localScale = new Vector3 ((JoScript.health / 100f) * 3.5f, healthbar_player.rectTransform.localScale.y, healthbar_player.rectTransform.localScale.z);
			healthbar_player.color = Color.Lerp (Color.red, Color.green, JoScript.health / 100f);
		}
		if (EnemyScript.health > 0) {
			healthbar_enemy.rectTransform.localScale = new Vector3 ((EnemyScript.health / 100f) * 3.5f, healthbar_enemy.rectTransform.localScale.y, healthbar_enemy.rectTransform.localScale.z);
			healthbar_enemy.color = Color.Lerp (Color.red, Color.green, EnemyScript.health / 100f);
		}

	}
		

}
