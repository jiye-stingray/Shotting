using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;     //�÷��̾��� ���ǵ�

    public bool isTouchTop; //��輱�� �浹���� �Ǵ��ϴ� ����
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
        //�̵� Ű �Է�
        float h = Input.GetAxisRaw("Horizontal");
        if ((isTouchRight && h == 1) || (isTouchLeft && h == -1)) //�浹������ Ű �������� �޾Ҵٸ� 0���� �ʱ�ȭ
            h = 0;
        float v = Input.GetAxisRaw("Vertical");
        if ((isTouchTop && v == 1) || (isTouchBottom && v == -1)) //�浹������ Ű �������� �޾Ҵٸ� 0���� �ʱ�ȭ
            v = 0;

        //�÷��̾��� ���� ������ ��������
        Vector3 curPos = transform.position;
        //������ �̵��ؾ��� ��ġ
        Vector3 nextPos = new Vector3(h, v, 0) * speed * Time.deltaTime;
        //transform �̵��� Time.deltaTime�� �� �ٿ�����Ѵ�.

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
            switch (collision.gameObject.name) //���� �浹 ���� �Ǵ�
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

    void OnTriggerExit2D(Collider2D collision) //�÷��� �����
    {
        if (collision.gameObject.tag == "Border")
        {
            switch (collision.gameObject.name) //���� �浹 ���� �Ǵ�
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
