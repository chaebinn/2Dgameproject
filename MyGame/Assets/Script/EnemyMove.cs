using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    Rigidbody2D rigid;
    Animator anim;
    SpriteRenderer spriteRenderer;
    CapsuleCollider2D capsuleCollider;

    public int nextMove;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        Invoke("Think", 5); //5초가 지난 뒤, 지정된 함수인 Thick()를 실행함. 즉, 5초간격으로 몬스터의 행동 바뀜
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rigid.velocity = new Vector2(nextMove, rigid.velocity.y); //속력 지정

        //지형 체크(떨어짐 방지)
        Vector2 frontVec = new Vector2(rigid.position.x + nextMove * 0.2f, rigid.position.y);
        Debug.DrawRay(frontVec, Vector3.down, new Color(0, 1, 0)); //에디터창에 ray를 그려줌
        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 1, LayerMask.GetMask("Platform")); // 아래 방향으로 Platform 레이어를 검사
        if (rayHit.collider == null)
        {
            Turn();
        }
    }
    void Think() //행동지표를 바꿔줄 함수 생성,자신 스스로를 호출하는 재귀함수
    {
        nextMove = Random.Range(-1, 2); //랜덤 클래스 사용. -1~1 범위의 랜덤 수 생성(2는 포함X)

        anim.SetInteger("WalkSpeed", nextMove); //애니메이션 설정

        //방향 조절
        if (nextMove != 0)
            spriteRenderer.flipX = nextMove == 1; // 오른쪽으로 이동하면 flipX를 false로 설정

        //재귀함수
        float nextThinkTime = Random.Range(2f, 5f); //생각하는 시간을 2~5초로 랜덤지정
        Invoke("Think", nextThinkTime); // 다음 Think 메서드 호출 예약
    }

    void Turn()
    {
        nextMove *= -1; // 움직임 반전
        spriteRenderer.flipX = nextMove == 1; // 이동 방향에 따라 flipX 설정
        CancelInvoke(); // 현재 예약된 모든 Invoke 취소
        Invoke("Think", 2);  // 2초 후에 Think 메서드 호출 예약
    }

    public void OnDamaged() //몬스터가 죽었을때 취할 액션 구현
    {
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);//투명해지도록 표현
        spriteRenderer.flipY = true;//뒤집혀지도록
        capsuleCollider.enabled = false;//콜라이더 비활성화
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);//점프했다가 추락
        Invoke("DeActive", 5); //비활성화 로직은 5초 뒤에 실행
    }

    void DeActive()
    {
        gameObject.SetActive(false); //게임 오브젝트 비활성화
    }
}
