using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed;     //속도
    public float health;    //체력(몇 발 맞았을 때 죽는지
    public Sprite[] sprites;    //spriteRender를 사용할 때


    SpriteRenderer spriteRenderer;  //피격 당했을 때 반투명한 이미지로 바꾸기
    Rigidbody2D rigid;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigid = GetComponent<Rigidbody2D>();
        rigid.velocity = Vector2.down * speed; //움직임
    }

    void OnHit(int dmg) //플레이어가 쏜 총알을 맞았을 때 데미지
    {
        health -= dmg;
        spriteRenderer.sprite = sprites[1]; //평소 스프라이트는 0, 피격시 스프라이트는 1
        Invoke("ReturnSprite",0.1f);

        if (health <= 0) // 체력이 0 이하 일때
        {
            Destroy(gameObject); //자신을 파괴
        }
          
    }

    void ReturnSprite()
    {
        spriteRenderer.sprite = sprites[0];
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "BorderBullet")
            Destroy(gameObject);
        else if (collision.gameObject.tag == "PlayerBullet") //플레이어의 총알에 맞았을 때 
        {

            Bullet bullet = collision.gameObject.GetComponent<Bullet>();
            OnHit(bullet.dmg); //총알의 데미지 만큼 체력을 감소

            Destroy(collision.gameObject);  //피격시 플레이어의 총알도 삭제
        }
       
    }
}
