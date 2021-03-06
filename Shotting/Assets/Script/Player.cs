using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
  

    public bool isTouchTop; //경계선과 충돌여부 판단하는 변수
    public bool isTouchBottom;
    public bool isTouchRight;
    public bool isTouchLeft;
   

    public int score;   //점수
    public int life;    //목숨
    public float speed;     //플레이어의 스피드

    public int power;     //파워 (파워아이템 먹었을 때 증가함)
    public int maxpower;    

    public int boom;    //폭탄도 파워처럼 최대크기와 현재 크기로 증가
    public int maxboom;

    public float maxShotDealy;  //최대 딜레이
    public float curShotDealy;  //현재 딜레이


    public GameObject bulletObjA; //총알 오브젝트 prefabs
    public GameObject bulletObjB;

    public GameManager gameManager;
    public ObjectManager objectManager; //오브젝트 풀링 사용을 위한 로직

    public GameObject boomEffect;   //폭탄 터졌을 때 effect

    public bool isHit;      //공격을 받은 상태에서 또 받지 못하게 하기
    public bool isBoomTime; //폭탄이 터지고 있는 상황
    public bool isRespawnTime;  //무적 타임 플래그 bool 변수 생성

    public GameObject[] followers;      //보조무기배열
   
    Animator anim;
    public SpriteRenderer spriteRenderer;


    void Awake()
    {
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void OnEnable()
    {
        
        Upbeatable();
        Invoke("Upbeatable", 3);

    }
    void Update()
    {
        Move();
        Fire();
        Boom();
        Reload();
    }

    void Upbeatable()
    {
        isRespawnTime = !isRespawnTime;
        if (isRespawnTime) //무적 타임 이팩트 (투명)
        {
            spriteRenderer.color = new Color(1, 1, 1, 0.5f);

            for (int i = 0; i < followers.Length; i++)
            {
                followers[i].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
            }
        }
        else
        {   //무적타임 종료 (원래대로)
            spriteRenderer.color = new Color(1, 1, 1, 1);
            for (int i = 0; i < followers.Length; i++)
            {
                followers[i].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
            }
        }
    }
    void Move()
    {
        //이동 키 입력
        float h = Input.GetAxisRaw("Horizontal");
        if ((isTouchRight && h == 1) || (isTouchLeft && h == -1)) //충돌했을때 키 움직임을 받았다면 0으로 초기화
            h = 0;
        float v = Input.GetAxisRaw("Vertical");
        if ((isTouchTop && v == 1) || (isTouchBottom && v == -1)) //충돌했을때 키 움직임을 받았다면 0으로 초기화
            v = 0;

        //플레이어의 현재 포지션 가져오기
        Vector3 curPos = transform.position;
        //다음에 이동해야할 위치
        Vector3 nextPos = new Vector3(h, v, 0) * speed * Time.deltaTime;
        //transform 이동엔 Time.deltaTime을 꼭 붙여줘야한다.

        transform.position = curPos + nextPos;

        if (Input.GetButtonDown("Horizontal") || //애니메이션 트리거 세팅
            Input.GetButtonUp("Horizontal"))
        {
            anim.SetInteger("Input", (int)h);
        }
    }

    void Fire()
    {
        
        if (!Input.GetButton("Fire1")) //버튼 체크
            return;

        if (curShotDealy < maxShotDealy) //아직 장전이 되지 않았을 때 리턴 (장전 시간이 충족 되지 않았다)
            return;

        switch (power)
        {
            case 1:
                GameObject bullet = objectManager.MakeObj("BulletPlayerA");
                bullet.transform.position = transform.position;
                Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
                rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse); 
                break;
            case 2:
                //+ transform은 무조건 Vector3
                //생성 위치에서 방향 백터 더하기
                GameObject bulletR = objectManager.MakeObj("BulletPlayerA");
                bulletR.transform.position = transform.position + Vector3.right * 0.1f;

                GameObject bulletL = objectManager.MakeObj("BulletPlayerA");
                bulletL.transform.position = transform.position + Vector3.left * 0.1f;

                Rigidbody2D rigidR = bulletR.GetComponent<Rigidbody2D>();
                Rigidbody2D rigidL = bulletL.GetComponent<Rigidbody2D>();

                rigidR.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                rigidL.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                break;

            default:
                GameObject bulletRR = objectManager.MakeObj("BulletPlayerA");
                bulletRR.transform.position = transform.position + Vector3.right * 0.35f;

                GameObject bulletCC = objectManager.MakeObj("BulletPlayerB");
                bulletCC.transform.position = transform.position;

                GameObject bulletLL = objectManager.MakeObj("BulletPlayerA");
                bulletLL.transform.position = transform.position + Vector3.left * 0.35f;

                Rigidbody2D rigidRR = bulletRR.GetComponent<Rigidbody2D>();
                Rigidbody2D rigidCC = bulletCC.GetComponent<Rigidbody2D>();
                Rigidbody2D rigidLL = bulletLL.GetComponent<Rigidbody2D>();

                rigidRR.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                rigidCC.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                rigidLL.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                break;


        }
        curShotDealy = 0; //딜레이변수 초기화
    }

    void Boom()
    {
        if (!Input.GetButton("Fire2")) //버튼 체크
            return;
        if (isBoomTime)
            return;
        if (boom == 0)
            return;

        boom--;
        gameManager.UpdateBoomIcon(boom);   
        isBoomTime = true;      

        //#1. Effect Visible
        boomEffect.SetActive(true);
        //폭탄 이팩트는 Invoke()로 시간차 비 활성화
        Invoke("OffEffect", 5f);
        //#2. Remove enemy
        GameObject[] enemiesS = objectManager.GetPool("EnemyS");
        GameObject[] enemiesM = objectManager.GetPool("EnemyM");
        GameObject[] enemiesL = objectManager.GetPool("EnemyL");
      
        for (int i = 0; i < enemiesM.Length; i++)
        {
            if (enemiesM[i].activeSelf)
            {
                Enemy enemyLogic = enemiesM[i].GetComponent<Enemy>();
                enemyLogic.OnHit(1000);
            }
            
        }
        for (int i = 0; i < enemiesL.Length; i++)
        {
            if (enemiesL[i].activeSelf)
            {
                Enemy enemyLogic = enemiesL[i].GetComponent<Enemy>();
                enemyLogic.OnHit(1000); 
            }
            
        }

        //#3.Remove Enemy Bullet
        GameObject[] bulletA = objectManager.GetPool("EnemyBulletA");
        GameObject[] bulletB = objectManager.GetPool("EnemyBulletB");
        for (int i = 0; i < bulletA.Length; i++)
        {
            bulletA[i].SetActive(false);
        }
        for (int i = 0; i < bulletB.Length; i++)
        {
            bulletB[i].SetActive(false);
        }

    }
    void Reload()
    {
        curShotDealy += Time.deltaTime;
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Border")
        {
            switch (collision.gameObject.name) //경계와 충돌 여부 판단
            {
                case "Top":
                    isTouchTop = true;
                    break;
                case "Bottom":
                    isTouchBottom = true;
                    break;
                   case "Right":
                    isTouchRight = true;
                    break;
                case "Left":
                    isTouchLeft = true;
                    break;
                default:
                    break;
            }
        }
        else if(collision.gameObject.tag == "Enemy"|| collision.gameObject.tag == "EnemyBullet") //죽음
        {
            if (isRespawnTime)  //변수를 활용하여 충돌 이벤트를 제한
                return;
            if (isHit)
                return;
            isHit = true;
            life--; //목숨 하나 죽음
            gameManager.UpdateLifeIcon(life);   //생명 이미지 관리

            if(life == 0) //죽었을때
            {
                gameManager.GameOver();
            }
            else
            {
                //플레이어를 복귀시키는 로직을 GameManager에서 관리
                gameManager.RespawnPlayer();
            }
            
            //적군이나 적군 총알에 부딪혔을 때
            gameObject.SetActive(false);
            collision.gameObject.SetActive(false);
        }
        else if(collision.gameObject.tag == "Item") //충돌한 오브젝트가 Item일때
        {
            Item item = collision.gameObject.GetComponent<Item>();  //Item 로직 가져오기
            switch (item.type) { //type 변수 이름으로 구분

                case "Coin":
                    score += 1000;  //점수 추가
                    break;
                
                case "Power":
                    if (power == maxpower)
                        score += 500;
                    else
                    {
                        power++;
                        AddFollower();
                    }
                    break;

                case "Boom":
                    if (boom == maxboom)
                        score += 50;
                    else
                    {
                        boom++;
                        gameManager.UpdateBoomIcon(boom);
                    }
                        
                    
                    break;
            }
            collision.gameObject.SetActive(false);
        }
    }

    void AddFollower()
    {
        if (power == 4)
            followers[0].SetActive(true);
        else if (power == 5)
            followers[1].SetActive(true);
        else if (power == 6)
            followers[2].SetActive(true);

    }

    void OffEffect()
    {
        boomEffect.SetActive(false);
        isBoomTime = false;
    }

    
    void OnTriggerExit2D(Collider2D collision) //플래그 지우기
    {
        if (collision.gameObject.tag == "Border")
        {
            switch (collision.gameObject.name) //경계와 충돌 여부 판단
            {
                case "Top":
                    isTouchTop = false;
                    break;
                case "Bottom":
                    isTouchBottom = false;
                    break;
                case "Right":
                    isTouchRight = false;
                    break;
                case "Left":
                    isTouchLeft = false;
                    break;


                default:
                    break;
            }
        }
    }
}
