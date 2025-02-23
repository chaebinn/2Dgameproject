using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public int totalScore; //총 점수
    public int stageScore; //스테이지 점수
    public int stageIndex; //스테이지 배열 인덱스로 사용
    public int hp; //목숨
    public PlayerMove player; //플레이어를 참조하기 위한 변수
    public GameObject[] Stages; //스테이지 전환을 위한 스테이지 인덱스 선언

    public Image[] UIHp; //UI에 표시되는 플레이어 체력 이미지 배열 생성
    public Text UIScore; //UI에 표시되는 점수 텍스트
    public Text UIStage; //UI에 표시되는 스테이지 텍스트
    public Text UIEndText; //게임종료시 텍스트
    public Text UIEndScore; //게임종료시 최종 점수 텍스트
    public GameObject UIRetryBtn; //재시작 버튼
    public GameObject UIHomeBtn; //홈 버튼


    private void Start()
    {
        if (UIEndText != null && UIEndScore != null)
        {
            UIEndText.enabled = false; // 게임 시작 시 UIEndText 비활성화
            UIEndScore.enabled = false; // 게임 시작 시 UIScoreText 비활성화
        }
    }

    private void Update()
    {
        UIScore.text = (totalScore + stageScore).ToString(); // 총 점수와 현재 스테이지 점수를 더해 텍스트로 변환
    }
    public void NextStage()
    {
        //스테이지 갯수를 확인해 다음 스테이지로 이동/게임종료 구현
        if (stageIndex < Stages.Length-1) //스테이지1,2일때는 다음스테이지로 이동
        {
            Stages[stageIndex].SetActive(false); // 현재 스테이지를 비활성화
            stageIndex++; //다음 스테이지로 이동하기 위해 스테이지 인덱스 증가
            Stages[stageIndex].SetActive(true); // 증가된 스테이지 인덱스에 해당하는 스테이지를 활성화
            PlayerReposition(); //플레이어 원위치

            UIStage.text = "STAGE" + (stageIndex + 1); // UI 텍스트를 현재 스테이지로 표시
        }
        else //마지막 스테이지일때는 게임종료 
        {
            UIEndText.text = "GAME CLEAR!"; // 텍스트 설정
            UIEndText.enabled = true; // 텍스트 컴포넌트 활성화
            UIEndScore.text = "SCORE:"+ UIScore.text; // 텍스트 설정
            UIEndScore.enabled = true; // 텍스트 컴포넌트 활성화
            UIRetryBtn.SetActive(true); // UIRetry 버튼 활성화
            UIHomeBtn.SetActive(true); // UIHometry 버튼 활성화
        }
        totalScore += stageScore; //점수 계산
        stageScore = 0;

    }

    public void HpDown()//낭떠러지 떨어졌을때의 hp처리
    {
        if (hp > 1) {
            hp--;
            UIHp[hp].color = new Color(1, 1, 1, 0);
        }
        else
        {
            //모든 hp UI 없애기
            UIHp[0].color = new Color(1, 1, 1, 0);

            player.OnDie();

            UIEndText.text = "GAME OVER"; // 텍스트 설정
            UIEndText.enabled = true; // 텍스트 컴포넌트 활성화
            UIEndScore.text = "SCORE:" + UIScore.text; // 텍스트 설정
            UIEndScore.enabled = true; // 텍스트 컴포넌트 활성화

            UIRetryBtn.SetActive(true); // UIRetry 버튼 활성화
            UIHomeBtn.SetActive(true); // UIHometry 버튼 활성화
        }

    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (hp > 1)
                PlayerReposition(); //시작했던 위치로 리스폰

            HpDown();//hp감소
        }

    }
    void PlayerReposition()
    {
        player.transform.position = new Vector3(-4.09f, 0.43f, -1); //시작했을때 플레이어의 위치
        player.VelocityZero(); //플레이어 정지시킴

    }

    public void Retry()
    {
        SceneManager.LoadScene("0");//첫 스테이지로 가서 재시작
    }

    public void Home()
    {
        SceneManager.LoadScene("StartGame"); //홈화면으로 가서 재시작
    }

}
