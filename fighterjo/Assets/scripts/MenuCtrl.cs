using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuCtrl : MonoBehaviour
{
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
        roundController.roundNum = 1;
        roundController.playerWins = 0;
        roundController.enemyWins = 0;
        Time.timeScale = 1;
    }

    public void exit()
    {
        Application.Quit();
    }

}
