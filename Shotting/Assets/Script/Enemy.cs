using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public string enemyname;    //적군 크기에 따른 이름
    public int EnemyScore;      //적군이 각자 가질 수 있는 Score변수
    public float speed;     //속도
    public float health;    //체력(몇 발 맞았을 때 죽는지
    public Sprite[] sprites;    //spriteRender를 사용할 때

    public float maxShotDealy;  //최대 딜레이
    public float curShotDealy;  //현재 딜레이

    public GameObject bulletObjA; //총알 오브젝트 prefabs
    public GameObject bulletObjB;

    public GameObject ItemCoin;     //아이템들
    public GameObject ItemPower;
    public GameObject ItemBoom;

    public GameObject player;       //GameManager 한테서 받아옴

    //오브젝트 풀링 하기 위해 사용하는 로직
    public ObjectManager objectManager; 

    SpriteRenderer spriteRenderer;  //피격 당했을 때 반투명한 이미지로 바꾸기

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

    }

    void OnEnable()
    {
        switch (enemyname)
        {
            case "L":
                health = 40;
                break;
            case "M":
                health = 10;
                break;
            case "S":
                health = 3;
                break;
        }
    }

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
            GameObject bullet = objectManager.MakeObj("BulletEnemyA");
            bullet.transform.position = transform.position;
            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
            
            //목표물로 방향 = 목표물 위치 - 자신의 위치
            Vector3 dirVec = player.transform.position - transform.position;
            rigid.AddForce(dirVec.normalized*3, ForceMode2D.Impulse);
        }
        else if(enemyname == "L")
        {
            GameObject bulletR = objectManager.MakeObj("BulletEnemyB");
            bulletR.transform.position = transform.position + Vector3.right * 0.3f;
            GameObject bulletL = objectManager.MakeObj("BulletEnemyB");
            bulletL.transform.position = transform.position + Vector3.left * 0.3f;

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

   

    public void OnHit(int dmg) //플레이어가 쏜 총알을 맞았을 때 데미지
    {
        //아이템이 다중으로 만들어지지 않도록 예외 처리
        if (health <= 0)
            return;
        health -= dmg;
        spriteRenderer.sprite = sprites[1]; //평소 스프라이트는 0, 피격시 스프라이트는 1
        Invoke("ReturnSprite",0.1f);

        if (health <= 0) // 체력이 0 이하 일때
        {
            Player playerlogic = player.GetComponent<Player>();
            playerlogic.score += EnemyScore;    //플레이어의 점수에 자신의 점수를 넣어줌

            //#. Random Ratio Item Drop
            int ran = Random.Range(0, 10);
            //랜덤 숫자를 이용하여 Item 로직 작성
            if (ran < 3)    //Not Item 30%
            {
                Debug.Log("Not Item");
            }
            else if (ran < 6)   //Coin 30%
            {
                GameObject itemCoin = objectManager.MakeObj("ItemCoin");
                itemCoin.transform.position = transform.position;
            }
            else if (ran < 8)   //Power 20%
            {
                GameObject itemPower = objectManager.MakeObj("ItemPower");
                itemPower.transform.position = transform.position;
            }
            else if (ran < 10)   //Boom 20%
            {
                GameObject itemBoom = objectManager.MakeObj("ItemBoom");
                itemBoom.transform.position = transform.position;

            }

            //Destroy()는 false로 교체
            gameObject.SetActive(false);

            //Quaternion.identity : 기본 회전 값 0
            transform.rotation = Quaternion.identity;
        }
          
    }

    void ReturnSprite()
    {
        spriteRenderer.sprite = sprites[0];
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "BorderBullet")
        {
            gameObject.SetActive(false);

            //Quaternion.identity : 기본 회전 값 0
            transform.rotation = Quaternion.identity;
        }
        else if (collision.gameObject.tag == "PlayerBullet") //플레이어의 총알에 맞았을 때 
        {

            Bullet bullet = collision.gameObject.GetComponent<Bullet>();
            OnHit(bullet.dmg); //총알의 데미지 만큼 체력을 감소

            collision.gameObject.SetActive(false);  //피격시 플레이어의 총알도 비활성화
        }
       
    }
}
