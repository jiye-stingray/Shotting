using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
  

    public bool isTouchTop; //경계선과 충돌여부 판단하는 변수
    public bool isTouchBottom;
    public bool isTouchRight;
    public bool isTouchLeft;

    public float speed;     //플레이어의 스피드
    public float power;     //파워 (파워아이템 먹었을 때 증가함)
    public float maxShotDealy;  //최대 딜레이
    public float curShotDealy;  //현재 딜레이


    public GameObject bulletObjA; //총알 오브젝트 prefabs
    public GameObject bulletObjB;


    Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }
    void Update()
    {
        Move();
        Fire();
        Reload();
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
                GameObject bullet = Instantiate(bulletObjA, transform.position, transform.rotation);
                Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
                rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse); 
                break;
            case 2:
                //+ transform은 무조건 Vector3
                //생성 위치에서 방향 백터 더하기
                GameObject bulletR = Instantiate(bulletObjA, transform.position + Vector3.right * 0.1f, transform.rotation);
                GameObject bulletL = Instantiate(bulletObjA, transform.position + Vector3.left * 0.1f, transform.rotation);

                Rigidbody2D rigidR = bulletR.GetComponent<Rigidbody2D>();
                Rigidbody2D rigidL = bulletL.GetComponent<Rigidbody2D>();
                rigidR.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                rigidL.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                break;
            case 3:
                GameObject bulletRR = Instantiate(bulletObjA, transform.position + Vector3.right * 0.35f, transform.rotation);
                GameObject bulletCC = Instantiate(bulletObjB, transform.position, transform.rotation);
                GameObject bulletLL = Instantiate(bulletObjA, transform.position + Vector3.left * 0.35f, transform.rotation);
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
