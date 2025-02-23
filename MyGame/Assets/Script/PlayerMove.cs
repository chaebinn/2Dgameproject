using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public GameManager gameManager; //게임매니저변수 생성
    public AudioClip audioJump; //점프 오디오 클립
    public AudioClip audioAttack; //공격 오디오 클립
    public AudioClip audioDamaged; //데미지받았을때 오디오 클립
    public AudioClip audioItem; //아이템 획득 오디오 클립
    public AudioClip audioDie; //플레이어 사망 오디오 클립
    public AudioClip audioFinish; //스테이지 또는 게임 클리어 오디오 클립

    public float maxSpeed; //최대 이동속도
    public float jumpPower; //점프 강도
    public int jumpCount; //점프 횟수 카운드 변수
    private bool isDoubleJumping = false; // 더블 점프 상태 여부를 나타내는 변수
    [SerializeField] float fallSec = 4f, destroySec = 500f; // 플랫폼이 떨어지기까지의 시간과 파괴되기까지의 시간 설정
    float moveX; //이동값 

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

    void Update() //단발적인 입력은 업데이트에서 
    {

        //점프
        if (Input.GetButtonDown("Jump") && (jumpCount < 2 || !isDoubleJumping))
        {
            rigid.velocity = Vector2.zero;
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            jumpCount++;
            PlaySound("JUMP"); //사운드적용
            if (jumpCount == 2) // 더블 점프일 때
            {
                isDoubleJumping = true;
                anim.SetBool("isDoubleJumping", true); // 애니메이터에 더블 점프 상태를 전달
            }
            anim.SetBool("isJumping", true); // 일반 점프 상태를 전달
        }

        if (Input.GetButtonUp("Horizontal") && jumpCount == 0) //플레이어가 점프 중이 아닐 경우
        {
            rigid.velocity = Vector2.zero; // 플레이어의 속도를 0으로 설정
            anim.SetBool("isJumping", false);// isJumping 플래그를 false로 설정하여 점프 애니메이션을 중지시킴
            anim.SetBool("isDoubleJumping", false); // isDoubleJumping 플래그를 false로 설정하여 더블 점프 애니메이션을 중지시킴

        }


        //방향키
        if (Input.GetButton("Horizontal"))  // 방향키 입력에 따라 스프라이트 방향을 전환
            spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1; // 수평 입력 값이 -1이면 스프라이트를 좌우 반전
        //애니메이션
        if (Mathf.Abs(rigid.velocity.x) < 0.3)
            anim.SetBool("isRunning", false); // 플레이어의 속도가 0.3보다 작으면 뛰는 애니메이션 중지
        else
            anim.SetBool("isRunning", true); // 플레이어의 속도가 0.3 이상이면 뛰는 애니메이션 시작

    }
    void FixedUpdate()
    {
        float h = Input.GetAxisRaw("Horizontal"); // 수평 입력 값 받기
        rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse); // 수평 입력에 따라 힘을 가해 이동

        if (rigid.velocity.x > maxSpeed) //오른쪽 최대 속도 제한
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
        else if (rigid.velocity.x < maxSpeed*(-1)) //왼쪽 최대 속도 제한
            rigid.velocity = new Vector2(maxSpeed*(-1), rigid.velocity.y);

        //지형 체크
        if (rigid.velocity.y < 0){ //하강중일때
            Debug.DrawRay(rigid.position, Vector3.down, new Color(0, 1, 0)); //에디터창에 ray를 그려줌
            RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Platform"));
            if (rayHit.collider != null) //충돌체가 있으면
            {
                if (rayHit.distance < 0.5f) //거리가 가까우면
                    anim.SetBool("isJumping", false); //점프 애니메이션 중지
            }
        }

    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy") // 충돌한 객체의 태그가 "Enemy"일 때
        { //enemy보다 위에 있음+아래로 낙하중=밟음
            if (rigid.velocity.y < 0 && transform.position.y > collision.transform.position.y)
            {
                OnAttack(collision.transform); //적을 공격
            }

            else //데미지받음
                OnDamaged(collision.transform.position); //플레이어 데미지 받음
        }


        if (collision.contacts[0].normal.y > 0.6f) //충돌면이 바닥일때
        {
            if (jumpCount > 0) 
                rigid.velocity = Vector2.zero; //속도를 0으로 하여 정지

            jumpCount = 0; //점프 횟수를 0으로 초기화
            isDoubleJumping = false; // 바닥에 닿으면 더블 점프 상태 해제
            anim.SetBool("isJumping", false); //점프 애니메이션 중지
            anim.SetBool("isDoubleJumping", false); //더블 점프 애니메이션 중지

        }
        else
        {
            rigid.velocity = Vector2.zero; //속도를 0으로하여 정지
        }

    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Item") //태그가 Item인 물체와 충돌시
        {
            bool isCherry = collision.gameObject.name.Contains("Cherry"); //각 과일을 구분하기 위함
            bool isStrawberry = collision.gameObject.name.Contains("Strawberry");
            bool iskiwi = collision.gameObject.name.Contains("Kiwi");
            bool isPineapple = collision.gameObject.name.Contains("Pineapple");
            bool isBanana = collision.gameObject.name.Contains("Banana");
            PlaySound("ITEM"); //사운드적용
            if (isCherry||isPineapple)
                gameManager.stageScore += 5; //크기가 작은 과일 획득시 5점 증가
            else if (isStrawberry|| iskiwi|| isBanana)
                gameManager.stageScore += 10; //크기가 큰 과일 획득시 10점 증가

            collision.gameObject.SetActive(false);
        }
        else if (collision.gameObject.tag == "Finish") //태그가 Finish인 물체와 충돌시
        {
            gameManager.NextStage();  //다음 스테이지
            PlaySound("FINISH"); //사운드적용
        }
    }

    void OnAttack(Transform enemy)
    { //몬스터의 죽음 관련 함수 호출
        gameManager.stageScore += 20; //몬스터 죽이면 200점 증가

        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
        EnemyMove enemyMove = enemy.GetComponent<EnemyMove>();
        enemyMove.OnDamaged();
        PlaySound("ATTACK"); //사운드 적용

    }

    void OnDamaged(Vector2 targetPos)
    {
        gameManager.HpDown(); //hp 감소
        gameObject.layer = 9; //레이어를 변경
        spriteRenderer.color = new Color(1, 1, 1, 0.4f); //색을 투명하게 하여 무적상태임을 시각적으로 표현
        int dirc = transform.position.x - targetPos.x > 0 ? 1 : -1;
        //(플레이어의 x축-맞은 적의 x축)의 값이 양수면 1, 아니면 -1
        rigid.AddForce(new Vector2(dirc, 1)*7, ForceMode2D.Impulse);

        anim.SetTrigger("doDamaged"); //애니메이션 적용
        PlaySound("JUMP"); //사운드적용


        Invoke("OffDamaged", 2);//무적상태를 2초간 유지하도록
    }

    void OffDamaged()
    {
        gameObject.layer = 10; //레이어를 변경
        spriteRenderer.color = new Color(1, 1, 1, 1); // 스프라이트의 색상을 원래대로 (투명도를 1로 설정하여 완전히 보이게)

}

    public void OnDie()
    {
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);//투명해지도록 표현
        spriteRenderer.flipY = true;//뒤집혀지도록
        capsuleCollider.enabled = false;//콜라이더 비활성화
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);//점프했다가 추락
        PlaySound("DIE"); //사운드적용


    }

    public void VelocityZero()
    {
        rigid.velocity= Vector2.zero; //속도를 0으로 (정지)
    }

 
    void PlaySound(string action)
    {
        switch (action) //액션별 사운드 출력
        {
            case "JUMP": //점프 사운드
                audioSource.clip = audioJump;
                audioSource.Play();
                break;
            case "ATTACK": //공격 사운드
                audioSource.clip = audioAttack;
                audioSource.Play();
                break;
            case "DAMAGED": //피격 사운드
                audioSource.clip = audioDamaged;
                audioSource.Play();
                break;
            case "ITEM": //아이템 획득 사운드
                audioSource.clip = audioItem;
                audioSource.Play();
                break;
            case "DIE": //플레이어가 죽을때 사운드
                audioSource.clip = audioDie;
                audioSource.Play();
                break;
            case "FINISH": //깃발에 닿아 스테이지가 끝날때 사운드
                audioSource.clip = audioFinish;
                audioSource.Play();
                break;
        }
    }
}
