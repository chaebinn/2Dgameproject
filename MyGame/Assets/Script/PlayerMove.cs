using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public GameManager gameManager; //���ӸŴ������� ����
    public AudioClip audioJump; //���� ����� Ŭ��
    public AudioClip audioAttack; //���� ����� Ŭ��
    public AudioClip audioDamaged; //�������޾����� ����� Ŭ��
    public AudioClip audioItem; //������ ȹ�� ����� Ŭ��
    public AudioClip audioDie; //�÷��̾� ��� ����� Ŭ��
    public AudioClip audioFinish; //�������� �Ǵ� ���� Ŭ���� ����� Ŭ��

    public float maxSpeed; //�ִ� �̵��ӵ�
    public float jumpPower; //���� ����
    public int jumpCount; //���� Ƚ�� ī��� ����
    private bool isDoubleJumping = false; // ���� ���� ���� ���θ� ��Ÿ���� ����
    [SerializeField] float fallSec = 4f, destroySec = 500f; // �÷����� ������������� �ð��� �ı��Ǳ������ �ð� ����
    float moveX; //�̵��� 

    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Animator anim;
    CapsuleCollider2D capsuleCollider;
    AudioSource audioSource;
    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
    } 

    void Update() //�ܹ����� �Է��� ������Ʈ���� 
    {

        //����
        if (Input.GetButtonDown("Jump") && (jumpCount < 2 || !isDoubleJumping))
        {
            rigid.velocity = Vector2.zero;
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            jumpCount++;
            PlaySound("JUMP"); //��������
            if (jumpCount == 2) // ���� ������ ��
            {
                isDoubleJumping = true;
                anim.SetBool("isDoubleJumping", true); // �ִϸ����Ϳ� ���� ���� ���¸� ����
            }
            anim.SetBool("isJumping", true); // �Ϲ� ���� ���¸� ����
        }

        if (Input.GetButtonUp("Horizontal") && jumpCount == 0) //�÷��̾ ���� ���� �ƴ� ���
        {
            rigid.velocity = Vector2.zero; // �÷��̾��� �ӵ��� 0���� ����
            anim.SetBool("isJumping", false);// isJumping �÷��׸� false�� �����Ͽ� ���� �ִϸ��̼��� ������Ŵ
            anim.SetBool("isDoubleJumping", false); // isDoubleJumping �÷��׸� false�� �����Ͽ� ���� ���� �ִϸ��̼��� ������Ŵ

        }


        //����Ű
        if (Input.GetButton("Horizontal"))  // ����Ű �Է¿� ���� ��������Ʈ ������ ��ȯ
            spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1; // ���� �Է� ���� -1�̸� ��������Ʈ�� �¿� ����
        //�ִϸ��̼�
        if (Mathf.Abs(rigid.velocity.x) < 0.3)
            anim.SetBool("isRunning", false); // �÷��̾��� �ӵ��� 0.3���� ������ �ٴ� �ִϸ��̼� ����
        else
            anim.SetBool("isRunning", true); // �÷��̾��� �ӵ��� 0.3 �̻��̸� �ٴ� �ִϸ��̼� ����

    }
    void FixedUpdate()
    {
        float h = Input.GetAxisRaw("Horizontal"); // ���� �Է� �� �ޱ�
        rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse); // ���� �Է¿� ���� ���� ���� �̵�

        if (rigid.velocity.x > maxSpeed) //������ �ִ� �ӵ� ����
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
        else if (rigid.velocity.x < maxSpeed*(-1)) //���� �ִ� �ӵ� ����
            rigid.velocity = new Vector2(maxSpeed*(-1), rigid.velocity.y);

        //���� üũ
        if (rigid.velocity.y < 0){ //�ϰ����϶�
            Debug.DrawRay(rigid.position, Vector3.down, new Color(0, 1, 0)); //������â�� ray�� �׷���
            RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Platform"));
            if (rayHit.collider != null) //�浹ü�� ������
            {
                if (rayHit.distance < 0.5f) //�Ÿ��� ������
                    anim.SetBool("isJumping", false); //���� �ִϸ��̼� ����
            }
        }

    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy") // �浹�� ��ü�� �±װ� "Enemy"�� ��
        { //enemy���� ���� ����+�Ʒ��� ������=����
            if (rigid.velocity.y < 0 && transform.position.y > collision.transform.position.y)
            {
                OnAttack(collision.transform); //���� ����
            }

            else //����������
                OnDamaged(collision.transform.position); //�÷��̾� ������ ����
        }


        if (collision.contacts[0].normal.y > 0.6f) //�浹���� �ٴ��϶�
        {
            if (jumpCount > 0) 
                rigid.velocity = Vector2.zero; //�ӵ��� 0���� �Ͽ� ����

            jumpCount = 0; //���� Ƚ���� 0���� �ʱ�ȭ
            isDoubleJumping = false; // �ٴڿ� ������ ���� ���� ���� ����
            anim.SetBool("isJumping", false); //���� �ִϸ��̼� ����
            anim.SetBool("isDoubleJumping", false); //���� ���� �ִϸ��̼� ����

        }
        else
        {
            rigid.velocity = Vector2.zero; //�ӵ��� 0�����Ͽ� ����
        }

    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Item") //�±װ� Item�� ��ü�� �浹��
        {
            bool isCherry = collision.gameObject.name.Contains("Cherry"); //�� ������ �����ϱ� ����
            bool isStrawberry = collision.gameObject.name.Contains("Strawberry");
            bool iskiwi = collision.gameObject.name.Contains("Kiwi");
            bool isPineapple = collision.gameObject.name.Contains("Pineapple");
            bool isBanana = collision.gameObject.name.Contains("Banana");
            PlaySound("ITEM"); //��������
            if (isCherry||isPineapple)
                gameManager.stageScore += 5; //ũ�Ⱑ ���� ���� ȹ��� 5�� ����
            else if (isStrawberry|| iskiwi|| isBanana)
                gameManager.stageScore += 10; //ũ�Ⱑ ū ���� ȹ��� 10�� ����

            collision.gameObject.SetActive(false);
        }
        else if (collision.gameObject.tag == "Finish") //�±װ� Finish�� ��ü�� �浹��
        {
            gameManager.NextStage();  //���� ��������
            PlaySound("FINISH"); //��������
        }
    }

    void OnAttack(Transform enemy)
    { //������ ���� ���� �Լ� ȣ��
        gameManager.stageScore += 20; //���� ���̸� 200�� ����

        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
        EnemyMove enemyMove = enemy.GetComponent<EnemyMove>();
        enemyMove.OnDamaged();
        PlaySound("ATTACK"); //���� ����

    }

    void OnDamaged(Vector2 targetPos)
    {
        gameManager.HpDown(); //hp ����
        gameObject.layer = 9; //���̾ ����
        spriteRenderer.color = new Color(1, 1, 1, 0.4f); //���� �����ϰ� �Ͽ� ������������ �ð������� ǥ��
        int dirc = transform.position.x - targetPos.x > 0 ? 1 : -1;
        //(�÷��̾��� x��-���� ���� x��)�� ���� ����� 1, �ƴϸ� -1
        rigid.AddForce(new Vector2(dirc, 1)*7, ForceMode2D.Impulse);

        anim.SetTrigger("doDamaged"); //�ִϸ��̼� ����
        PlaySound("JUMP"); //��������


        Invoke("OffDamaged", 2);//�������¸� 2�ʰ� �����ϵ���
    }

    void OffDamaged()
    {
        gameObject.layer = 10; //���̾ ����
        spriteRenderer.color = new Color(1, 1, 1, 1); // ��������Ʈ�� ������ ������� (������ 1�� �����Ͽ� ������ ���̰�)

}

    public void OnDie()
    {
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);//������������ ǥ��
        spriteRenderer.flipY = true;//������������
        capsuleCollider.enabled = false;//�ݶ��̴� ��Ȱ��ȭ
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);//�����ߴٰ� �߶�
        PlaySound("DIE"); //��������


    }

    public void VelocityZero()
    {
        rigid.velocity= Vector2.zero; //�ӵ��� 0���� (����)
    }

 
    void PlaySound(string action)
    {
        switch (action) //�׼Ǻ� ���� ���
        {
            case "JUMP": //���� ����
                audioSource.clip = audioJump;
                audioSource.Play();
                break;
            case "ATTACK": //���� ����
                audioSource.clip = audioAttack;
                audioSource.Play();
                break;
            case "DAMAGED": //�ǰ� ����
                audioSource.clip = audioDamaged;
                audioSource.Play();
                break;
            case "ITEM": //������ ȹ�� ����
                audioSource.clip = audioItem;
                audioSource.Play();
                break;
            case "DIE": //�÷��̾ ������ ����
                audioSource.clip = audioDie;
                audioSource.Play();
                break;
            case "FINISH": //��߿� ��� ���������� ������ ����
                audioSource.clip = audioFinish;
                audioSource.Play();
                break;
        }
    }
}
