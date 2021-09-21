using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public string enemyname;    //적군 크기에 따른 이름

    public float speed;     //속도
    public float health;    //체력(몇 발 맞았을 때 죽는지
    public Sprite[] sprites;    //spriteRender를 사용할 때

    public float maxShotDealy;  //최대 딜레이
    public float curShotDealy;  //현재 딜레이

    public GameObject bulletObjA; //총알 오브젝트 prefabs
    public GameObject bulletObjB;

    public GameObject player;       //GameManager 한테서 받아옴

    SpriteRenderer spriteRenderer;  //피격 당했을 때 반투명한 이미지로 바꾸기


    void Update()
    {
        Fire();
        Reload();
    }
    void Fire()
    {   
        if (curShotDealy < maxShotDealy) //아직 장전이 되지 않았을 때 리턴 (장전 시간이 충족 되지 않았다)
            return;

        if(enemyname == "S") //enemyname이라는 변수로 적군의 공격방식 정하기
        {
            GameObject bullet = Instantiate(bulletObjA, transform.position, transform.rotation);
            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
            
            //목표물로 방향 = 목표물 위치 - 자신의 위치
            Vector3 dirVec = player.transform.position - transform.position;
            rigid.AddForce(dirVec.normalized*3, ForceMode2D.Impulse);
        }
        else if(enemyname == "L")
        {
            GameObject bulletR = Instantiate(bulletObjB, transform.position + Vector3.right * 0.3f, transform.rotation);
            GameObject bulletL = Instantiate(bulletObjB, transform.position + Vector3.left * 0.3f, transform.rotation);

            Rigidbody2D rigidR = bulletL.GetComponent<Rigidbody2D>();
            Rigidbody2D rigidL = bulletR.GetComponent<Rigidbody2D>();

            //목표물로 방향 = 목표물 위치 - 자신의 위치
            Vector3 dirVecR = player.transform.position - (transform.position + Vector3.right * 0.3f);
            Vector3 dirVecL = player.transform.position - (transform.position + Vector3.left * 0.3f);

            rigidR.AddForce(dirVecR.normalized * 4, ForceMode2D.Impulse);
            rigidL.AddForce(dirVecL.normalized * 4, ForceMode2D.Impulse);
        }

        curShotDealy = 0; //딜레이변수 초기화
    }

    void Reload()
    {
        curShotDealy += Time.deltaTime;
    }

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        
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
