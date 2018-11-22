using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour {

	void Start () {
		
	}
	
	void Update () {
		
	}

     public void SwitchScene(int numberScene)
    {
        switch (numberScene)
        {
            case 0:
                SceneManager.LoadScene("level-1");
                break;
            case 1:
                SceneManager.LoadScene("gameover");
                break;
            case 2:
                SceneManager.LoadScene("winner");
                break;
            case 3:
                SceneManager.LoadScene("start");
                break;
        }
        
    }

     public void Exit()
    {
        Application.Quit();
    }
}
