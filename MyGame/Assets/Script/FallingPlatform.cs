using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPlatform : MonoBehaviour
{

    [SerializeField] float fallSec = 0.5f, destroySec = 2f;  // Platform�� ������������� �ð��� �ı��Ǳ������ �ð�
    Rigidbody2D rb;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name.Equals("Player"))
        {
            Invoke("FallPlatform", fallSec); // ���� �ð� �� FallPlatform �޼��� ȣ��
            Destroy(gameObject, destroySec); // ���� �ð� �� ���� ���� ������Ʈ �ı�
        }
    }
    void FallPlatform()
    {
        rb.isKinematic = false; //������ ���������� ����(������Ģ�� ������ ��)
    }
}
