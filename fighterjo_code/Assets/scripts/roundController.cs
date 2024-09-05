using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class roundController : MonoBehaviour {

    public static int roundNum = 1;
    public static int playerWins = 0;
    public static int enemyWins = 0;
    //see how many rounds won in editor(temp)
    public int rounds;
    public int prounds;
    public int erounds;
    //for coundown till round start
    public bool waited = false;
    public static bool roundStarted = false;
    public bool started;
    public int x;
    //countdown sprites
    public Image countdown;
    public Sprite[] counterSprites = new Sprite[4];
    //round number sprites
    public Image roundImage;
    public Sprite[] roundSprites = new Sprite[5];

    //wins and loses images
    public Image winImage;
    public Image lossImage;
    public Sprite[] wins_losses = new Sprite[2];
    
    //audio clips and source
    public AudioSource source;
    public AudioClip[] timerSound = new AudioClip[4];
    public AudioClip[] roundSound = new AudioClip[5];
    public AudioClip knockout;
    

	// Use this for initialization
	void Start () {
        source = GetComponent<AudioSource>();
        
		rounds = roundNum;
		prounds = playerWins;
		erounds = enemyWins;
        roundStarted = false;
		InvokeRepeating ("waitThreeSecs", 1, 1);

        roundImage.sprite = roundSprites[roundNum-1];
        source.clip = roundSound[roundNum - 1];
        source.Play();
        if (playerWins > 0)
        {
            winImage.sprite = wins_losses[playerWins - 1];
            winImage.color = new Color(255, 255, 255, 255);
        }
        if (enemyWins > 0)
        {
            lossImage.sprite = wins_losses[enemyWins - 1];
            lossImage.color = new Color(255, 255, 255, 255);
        }
        winImage.SetNativeSize();
        lossImage.SetNativeSize();
	}

	
	// Update is called once per frame
	void Update () 
	{
        if (JoScript.health <= 0 || EnemyScript.health <= 0)
        {
            playKnockout();
        }
        if (waited)
            roundStarted = true;
		if (roundNum > rounds)
			detNextRound ();
        started = roundStarted;

	}

    void playKnockout()
    {
        source.clip = knockout;
        source.Play();
    }

	void detNextRound()
	{
		prounds = playerWins;
		erounds = enemyWins;
        if (playerWins < 3 && enemyWins < 3)
        {
            
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else
        {

            if (SceneManager.GetActiveScene().buildIndex == 3 || enemyWins >= 3)
                SceneManager.LoadScene("Menu");
            else
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            roundNum = 1;
            playerWins = 0;
            enemyWins = 0;
        }
	}

	void waitThreeSecs()
	{
		if (x < 4)
        {
            while (source.isPlaying);
            
            source.clip = timerSound[x];
            source.Play();
            
            countdown.sprite = counterSprites[x];
			x++;
           
		}
        else {
			waited = true;
            Destroy(countdown);
			CancelInvoke ("waitThreeSecs");
		}
	}

    
}
