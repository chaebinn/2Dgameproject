using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public int totalScore; //�� ����
    public int stageScore; //�������� ����
    public int stageIndex; //�������� �迭 �ε����� ���
    public int hp; //���
    public PlayerMove player; //�÷��̾ �����ϱ� ���� ����
    public GameObject[] Stages; //�������� ��ȯ�� ���� �������� �ε��� ����

    public Image[] UIHp; //UI�� ǥ�õǴ� �÷��̾� ü�� �̹��� �迭 ����
    public Text UIScore; //UI�� ǥ�õǴ� ���� �ؽ�Ʈ
    public Text UIStage; //UI�� ǥ�õǴ� �������� �ؽ�Ʈ
    public Text UIEndText; //��������� �ؽ�Ʈ
    public Text UIEndScore; //��������� ���� ���� �ؽ�Ʈ
    public GameObject UIRetryBtn; //����� ��ư
    public GameObject UIHomeBtn; //Ȩ ��ư


    private void Start()
    {
        if (UIEndText != null && UIEndScore != null)
        {
            UIEndText.enabled = false; // ���� ���� �� UIEndText ��Ȱ��ȭ
            UIEndScore.enabled = false; // ���� ���� �� UIScoreText ��Ȱ��ȭ
        }
    }

    private void Update()
    {
        UIScore.text = (totalScore + stageScore).ToString(); // �� ������ ���� �������� ������ ���� �ؽ�Ʈ�� ��ȯ
    }
    public void NextStage()
    {
        //�������� ������ Ȯ���� ���� ���������� �̵�/�������� ����
        if (stageIndex < Stages.Length-1) //��������1,2�϶��� �������������� �̵�
        {
            Stages[stageIndex].SetActive(false); // ���� ���������� ��Ȱ��ȭ
            stageIndex++; //���� ���������� �̵��ϱ� ���� �������� �ε��� ����
            Stages[stageIndex].SetActive(true); // ������ �������� �ε����� �ش��ϴ� ���������� Ȱ��ȭ
            PlayerReposition(); //�÷��̾� ����ġ

            UIStage.text = "STAGE" + (stageIndex + 1); // UI �ؽ�Ʈ�� ���� ���������� ǥ��
        }
        else //������ ���������϶��� �������� 
        {
            UIEndText.text = "GAME CLEAR!"; // �ؽ�Ʈ ����
            UIEndText.enabled = true; // �ؽ�Ʈ ������Ʈ Ȱ��ȭ
            UIEndScore.text = "SCORE:"+ UIScore.text; // �ؽ�Ʈ ����
            UIEndScore.enabled = true; // �ؽ�Ʈ ������Ʈ Ȱ��ȭ
            UIRetryBtn.SetActive(true); // UIRetry ��ư Ȱ��ȭ
            UIHomeBtn.SetActive(true); // UIHometry ��ư Ȱ��ȭ
        }
        totalScore += stageScore; //���� ���
        stageScore = 0;

    }

    public void HpDown()//�������� ������������ hpó��
    {
        if (hp > 1) {
            hp--;
            UIHp[hp].color = new Color(1, 1, 1, 0);
        }
        else
        {
            //��� hp UI ���ֱ�
            UIHp[0].color = new Color(1, 1, 1, 0);

            player.OnDie();

            UIEndText.text = "GAME OVER"; // �ؽ�Ʈ ����
            UIEndText.enabled = true; // �ؽ�Ʈ ������Ʈ Ȱ��ȭ
            UIEndScore.text = "SCORE:" + UIScore.text; // �ؽ�Ʈ ����
            UIEndScore.enabled = true; // �ؽ�Ʈ ������Ʈ Ȱ��ȭ

            UIRetryBtn.SetActive(true); // UIRetry ��ư Ȱ��ȭ
            UIHomeBtn.SetActive(true); // UIHometry ��ư Ȱ��ȭ
        }

    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (hp > 1)
                PlayerReposition(); //�����ߴ� ��ġ�� ������

            HpDown();//hp����
        }

    }
    void PlayerReposition()
    {
        player.transform.position = new Vector3(-4.09f, 0.43f, -1); //���������� �÷��̾��� ��ġ
        player.VelocityZero(); //�÷��̾� ������Ŵ

    }

    public void Retry()
    {
        SceneManager.LoadScene("0");//ù ���������� ���� �����
    }

    public void Home()
    {
        SceneManager.LoadScene("StartGame"); //Ȩȭ������ ���� �����
    }

}
