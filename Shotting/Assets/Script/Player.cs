using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;     //플레이어의 스피드

    public bool isTouchTop; //경계선과 충돌여부 판단하는 변수
    public bool isTouchBottom;
    public bool isTouchRight;
    public bool isTouchLeft;

    public GameObject bulletObjA;
    public GameObject bulletObjB;


    Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }
    void Update()
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

        if (Input.GetButtonDown("Horizontal")||
            Input.GetButtonUp("Horizontal"))
        {
            anim.SetInteger("Input",(int)h);
        }

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
