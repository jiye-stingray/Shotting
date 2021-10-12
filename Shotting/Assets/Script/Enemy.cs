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

    Animator anim; //보스

    //보스 공격 패턴 흐름에 필요한 변수
    public int patternIndex;
    public int curPatternCount;
    public int[] maxPatternCount;
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (enemyname == "B")
            anim = GetComponent<Animator>();

    }

    void OnEnable()
    {
        switch (enemyname)
        {
            case "B":
                health = 3000;
                Invoke("Stop",2);
                break;
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
        if (enemyname == "B")
            return;

        Fire();
        Reload();
    }

    //보스를 멈추는 함수
    void Stop()
    {
        //오브젝트 풀링에 의해서 생성되고 한번, 다시 활성화 되었을 때 한번. 총 2번이 호출 되기 때문에 그것을 방지
        if (!gameObject.activeSelf)
            return;
        Rigidbody2D rigid = GetComponent<Rigidbody2D>();
        rigid.velocity = Vector2.zero;  //속도 멈추기

        Invoke("Think", 2);
    }

    //패턴을 생각하는 함수
    void Think()
    { 
        //패턴 4개
        patternIndex = patternIndex == 3 ? 0 : patternIndex + 1;
        curPatternCount = 0;        //패턴 변경시 패턴 실행 횟수 변경 초기화
        switch (patternIndex)
        {
            case 0:
                FireFoward();
                break;
            case 1:
                FireShot();
                break;
            case 2:
                FireArc();
                break;
            case 3:
                FireAround();
                break;
            

            default:
                break;
        }
    }

    //앞으로 4발 발사
    void FireFoward()
    {
        if (health <= 0)
            return;
        //#.Fire 4 Bullet Foward
        GameObject bulletR = objectManager.MakeObj("BulletBossA");
        bulletR.transform.position = transform.position + Vector3.right * 0.3f;
        GameObject bulletRR = objectManager.MakeObj("BulletBossA");
        bulletRR.transform.position = transform.position + Vector3.right * 0.45f;
        GameObject bulletL = objectManager.MakeObj("BulletBossA");
        bulletL.transform.position = transform.position + Vector3.left * 0.3f;
        GameObject bulletLL = objectManager.MakeObj("BulletBossA");
        bulletLL.transform.position = transform.position + Vector3.left * 0.45f;

        Rigidbody2D rigidR = bulletR.GetComponent<Rigidbody2D>();
        Rigidbody2D rigidRR = bulletRR.GetComponent<Rigidbody2D>();
        Rigidbody2D rigidL = bulletL.GetComponent<Rigidbody2D>();
        Rigidbody2D rigidLL = bulletLL.GetComponent<Rigidbody2D>();


        rigidR.AddForce(Vector2.down * 8, ForceMode2D.Impulse);
        rigidRR.AddForce(Vector2.down * 8, ForceMode2D.Impulse);
        rigidL.AddForce(Vector2.down * 8, ForceMode2D.Impulse);
        rigidLL.AddForce(Vector2.down * 8, ForceMode2D.Impulse);

        //#.Pattern Counting
        curPatternCount++;
        //각 패턴별 횟수를 실행하고 다음 패턴으로 넘어가게 구현
        if (curPatternCount < maxPatternCount[patternIndex]) //만약 현재 패턴 횟수가 정해진 최대 패턴 횟수보다 적다면
            Invoke("FireFoward", 2); //다음 패턴으로 넘어가는 것이 아닌 다시 실행
        else
            Invoke("Think", 2);

    }

    //플레이어 방향으로 샷건
    void FireShot()
    {
        if (health <= 0)
            return;
        for (int i = 0; i < 5; i++)
        {
            GameObject bullet = objectManager.MakeObj("BulletEnemyB");
            bullet.transform.position = transform.position;


            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
            //목표물로 방향 = 목표물 위치 - 자신의 위치
            Vector2 dirVec = player.transform.position - transform.position;
            //위치가 겹치지 않게 랜덤 백터를 더하여 구현
            Vector2 ranVec = new Vector2(Random.Range(-0.5f, 0.5f), Random.Range(0f, 2f));
            dirVec += ranVec;
            rigid.AddForce(dirVec.normalized * 3, ForceMode2D.Impulse);
        }
        
        curPatternCount++;
        //각 패턴별 횟수를 실행하고 다음 패턴으로 넘어가게 구현
        if (curPatternCount < maxPatternCount[patternIndex]) //만약 현재 패턴 횟수가 정해진 최대 패턴 횟수보다 적다면
            Invoke("FireShot", 3.5f); //다음 패턴으로 넘어가는 것이 아닌 다시 실행
        else
            Invoke("Think", 2);

    }

    //부채모양으로 발사
    void FireArc()
    {
        if (health <= 0)
            return;
       
        //#.Fire Arc Continue Fire
        GameObject bullet = objectManager.MakeObj("BulletEnemyA");
        bullet.transform.position = transform.position;
        bullet.transform.rotation = Quaternion.identity;


        Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
        //Mathf.Sin() : 삼각함수
        //Mathf.Cos() : 시작 각도만 다를 뿐, 발사 모양은 동일
        //Mathf.PI() : 파이
        //원의 둘레값을 많이 줄 수 록 빠르게 파형을 그림
        Vector2 dirVec = new Vector2(Mathf.Cos(Mathf.PI *10* curPatternCount/ maxPatternCount[patternIndex]),-1);
        rigid.AddForce(dirVec.normalized * 5, ForceMode2D.Impulse);



        curPatternCount++;
        //각 패턴별 횟수를 실행하고 다음 패턴으로 넘어가게 구현
        if (curPatternCount < maxPatternCount[patternIndex]) //만약 현재 패턴 횟수가 정해진 최대 패턴 횟수보다 적다면
            Invoke("FireArc", 0.2f); //다음 패턴으로 넘어가는 것이 아닌 다시 실행
        else
            Invoke("Think", 2);

    }

    //원 형태의 전체 공격
    void FireAround()
    {
        if (health <= 0)
            return;

        //#.Fire Around
        int roundNumA = 40;
        int roundNumB = 30;
        //패턴 횟수에 따라 생성되는 총알 갯수 조절로 난이도 상승
        int roundNum = curPatternCount % 2 == 0? roundNumA : roundNumB;
        for (int i = 0; i < roundNumA; i++)
        {
            GameObject bullet = objectManager.MakeObj("BulletBossB");
            bullet.transform.position = transform.position;
            bullet.transform.rotation = Quaternion.identity;


            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
            //원의 둘레값 PI * 2
            //생성되는 총알의 순번을 활용하여 바꿈
            Vector2 dirVec = new Vector2(Mathf.Cos(Mathf.PI * 2 * i / roundNum)
                                        ,Mathf.Sin(Mathf.PI * 2 * i / roundNum));
            rigid.AddForce(dirVec.normalized * 2, ForceMode2D.Impulse);

            //총알이 가리키는 방향에 따라 회전하게 만들기
            Vector3 rotVec = Vector3.forward * 360 * i / roundNum + Vector3.forward * 90;
            bullet.transform.Rotate(rotVec);
        }
        

        curPatternCount++;
        //각 패턴별 횟수를 실행하고 다음 패턴으로 넘어가게 구현
        if (curPatternCount < maxPatternCount[patternIndex]) //만약 현재 패턴 횟수가 정해진 최대 패턴 횟수보다 적다면
            Invoke("FireAround", 0.7f); //다음 패턴으로 넘어가는 것이 아닌 다시 실행
        else
            Invoke("Think", 2);

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
        if (enemyname == "B")
        {
            anim.SetTrigger("OnHit");
        }
        else
        {
            spriteRenderer.sprite = sprites[1]; //평소 스프라이트는 0, 피격시 스프라이트는 1
            Invoke("ReturnSprite", 0.1f);
        }
            
        

        if (health <= 0) // 체력이 0 이하 일때
        {
            Player playerlogic = player.GetComponent<Player>();
            playerlogic.score += EnemyScore;    //플레이어의 점수에 자신의 점수를 넣어줌

            //#. Random Ratio Item Drop
            int ran = enemyname == "B" ? 0 : Random.Range(0, 10);
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

            CancelInvoke();
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
        if (collision.gameObject.tag == "BorderBullet" && enemyname != "B")
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
