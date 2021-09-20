using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed;     //�ӵ�
    public float health;    //ü��(�� �� �¾��� �� �״���
    public Sprite[] sprites;    //spriteRender�� ����� ��


    SpriteRenderer spriteRenderer;  //�ǰ� ������ �� �������� �̹����� �ٲٱ�
    Rigidbody2D rigid;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigid = GetComponent<Rigidbody2D>();
        rigid.velocity = Vector2.down * speed; //������
    }

    void OnHit(int dmg) //�÷��̾ �� �Ѿ��� �¾��� �� ������
    {
        health -= dmg;
        spriteRenderer.sprite = sprites[1]; //��� ��������Ʈ�� 0, �ǰݽ� ��������Ʈ�� 1
        Invoke("ReturnSprite",0.1f);

        if (health <= 0) // ü���� 0 ���� �϶�
        {
            Destroy(gameObject); //�ڽ��� �ı�
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
        else if (collision.gameObject.tag == "PlayerBullet") //�÷��̾��� �Ѿ˿� �¾��� �� 
        {

            Bullet bullet = collision.gameObject.GetComponent<Bullet>();
            OnHit(bullet.dmg); //�Ѿ��� ������ ��ŭ ü���� ����

            Destroy(collision.gameObject);  //�ǰݽ� �÷��̾��� �Ѿ˵� ����
        }
       
    }
}
