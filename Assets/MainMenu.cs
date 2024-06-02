using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{ 
    public void PlayGame()
    {
        Debug.Log("Game will start");
        SceneManager.LoadSceneAsync("RunnerScene");
        
    }

    public void QuitGame()
    {
        Debug.Log("Exit game");
        Application.Quit();
    }
}
