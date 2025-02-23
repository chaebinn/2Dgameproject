using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class StartGame : MonoBehaviour 
{
    // Start is called before the first frame update
    public void StartBtn()
    {
        SceneManager.LoadScene("game");//버튼 누르면 game씬으로 전환
    }

}
