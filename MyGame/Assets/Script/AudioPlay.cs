using UnityEngine;

using System.Collections;



public class AudioPlay : MonoBehaviour
{

    void Start()
    {

        DontDestroyOnLoad(transform.gameObject); // 다른 씬으로 넘어가도 오디오가 끊기지 않게

    }


}
