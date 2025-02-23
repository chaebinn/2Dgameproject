using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPlatform : MonoBehaviour
{

    [SerializeField] float fallSec = 0.5f, destroySec = 2f;  // Platform이 떨어지기까지의 시간과 파괴되기까지의 시간
    Rigidbody2D rb;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name.Equals("Player"))
        {
            Invoke("FallPlatform", fallSec); // 일정 시간 후 FallPlatform 메서드 호출
            Destroy(gameObject, destroySec); // 일정 시간 후 현재 게임 오브젝트 파괴
        }
    }
    void FallPlatform()
    {
        rb.isKinematic = false; //발판이 떨어지도록 설정(물리법칙을 따르게 함)
    }
}
