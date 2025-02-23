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
        Invoke("Think", 5); //5�ʰ� ���� ��, ������ �Լ��� Thick()�� ������. ��, 5�ʰ������� ������ �ൿ �ٲ�
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rigid.velocity = new Vector2(nextMove, rigid.velocity.y); //�ӷ� ����

        //���� üũ(������ ����)
        Vector2 frontVec = new Vector2(rigid.position.x + nextMove * 0.2f, rigid.position.y);
        Debug.DrawRay(frontVec, Vector3.down, new Color(0, 1, 0)); //������â�� ray�� �׷���
        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 1, LayerMask.GetMask("Platform")); // �Ʒ� �������� Platform ���̾ �˻�
        if (rayHit.collider == null)
        {
            Turn();
        }
    }
    void Think() //�ൿ��ǥ�� �ٲ��� �Լ� ����,�ڽ� �����θ� ȣ���ϴ� ����Լ�
    {
        nextMove = Random.Range(-1, 2); //���� Ŭ���� ���. -1~1 ������ ���� �� ����(2�� ����X)

        anim.SetInteger("WalkSpeed", nextMove); //�ִϸ��̼� ����

        //���� ����
        if (nextMove != 0)
            spriteRenderer.flipX = nextMove == 1; // ���������� �̵��ϸ� flipX�� false�� ����

        //����Լ�
        float nextThinkTime = Random.Range(2f, 5f); //�����ϴ� �ð��� 2~5�ʷ� ��������
        Invoke("Think", nextThinkTime); // ���� Think �޼��� ȣ�� ����
    }

    void Turn()
    {
        nextMove *= -1; // ������ ����
        spriteRenderer.flipX = nextMove == 1; // �̵� ���⿡ ���� flipX ����
        CancelInvoke(); // ���� ����� ��� Invoke ���
        Invoke("Think", 2);  // 2�� �Ŀ� Think �޼��� ȣ�� ����
    }

    public void OnDamaged() //���Ͱ� �׾����� ���� �׼� ����
    {
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);//������������ ǥ��
        spriteRenderer.flipY = true;//������������
        capsuleCollider.enabled = false;//�ݶ��̴� ��Ȱ��ȭ
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);//�����ߴٰ� �߶�
        Invoke("DeActive", 5); //��Ȱ��ȭ ������ 5�� �ڿ� ����
    }

    void DeActive()
    {
        gameObject.SetActive(false); //���� ������Ʈ ��Ȱ��ȭ
    }
}
