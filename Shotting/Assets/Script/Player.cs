using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
  

    public bool isTouchTop; //��輱�� �浹���� �Ǵ��ϴ� ����
    public bool isTouchBottom;
    public bool isTouchRight;
    public bool isTouchLeft;
   

    public int score;   //����
    public int life;    //���
    public float speed;     //�÷��̾��� ���ǵ�

    public int power;     //�Ŀ� (�Ŀ������� �Ծ��� �� ������)
    public int maxpower;    

    public int boom;    //��ź�� �Ŀ�ó�� �ִ�ũ��� ���� ũ��� ����
    public int maxboom;

    public float maxShotDealy;  //�ִ� ������
    public float curShotDealy;  //���� ������


    public GameObject bulletObjA; //�Ѿ� ������Ʈ prefabs
    public GameObject bulletObjB;

    public GameManager gameManager;
    public GameObject boomEffect;   //��ź ������ �� effect

    public bool isHit;      //������ ���� ���¿��� �� ���� ���ϰ� �ϱ�
    public bool isBoomTime; //��ź�� ������ �ִ� ��Ȳ

    Animator anim;
 

    void Awake()
    {
        anim = GetComponent<Animator>();
    }
    void Update()
    {
        Move();
        Fire();
        Boom();
        Reload();
    }

    void Move()
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

        if (Input.GetButtonDown("Horizontal") || //�ִϸ��̼� Ʈ���� ����
            Input.GetButtonUp("Horizontal"))
        {
            anim.SetInteger("Input", (int)h);
        }
    }

    void Fire()
    {
        
        if (!Input.GetButton("Fire1")) //��ư üũ
            return;

        if (curShotDealy < maxShotDealy) //���� ������ ���� �ʾ��� �� ���� (���� �ð��� ���� ���� �ʾҴ�)
            return;

        switch (power)
        {
            case 1:
                GameObject bullet = Instantiate(bulletObjA, transform.position, transform.rotation);
                Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
                rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse); 
                break;
            case 2:
                //+ transform�� ������ Vector3
                //���� ��ġ���� ���� ���� ���ϱ�
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
        

        
        curShotDealy = 0; //�����̺��� �ʱ�ȭ
    }

    void Boom()
    {
        if (!Input.GetButton("Fire2")) //��ư üũ
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
        //��ź ����Ʈ�� Invoke()�� �ð��� �� Ȱ��ȭ
        Invoke("OffEffect", 5f);
        //#2. Remove Enemy
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        for (int i = 0; i < enemies.Length; i++)
        {
            Enemy enemyLogic = enemies[i].GetComponent<Enemy>();
            enemyLogic.OnHit(100);
        }

        //#3.Remove Enemy Bullet
        GameObject[] bullets = GameObject.FindGameObjectsWithTag("EnemyBullet");
        for (int i = 0; i < bullets.Length; i++)
        {
            Destroy(bullets[i]);
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
        else if(collision.gameObject.tag == "Enemy"|| collision.gameObject.tag == "EnemyBullet") //����
        {
            if (isHit)
                return;
            isHit = true;
            life--; //��� �ϳ� ����
            gameManager.UpdateLifeIcon(life);   //���� �̹��� ����

            if(life == 0) //�׾�����
            {
                gameManager.GameOver();
            }
            else
            {
                //�÷��̾ ���ͽ�Ű�� ������ GameManager���� ����
                gameManager.RespawnPlayer();
            }
            
            //�����̳� ���� �Ѿ˿� �ε����� ��
            gameObject.SetActive(false);
            Destroy(collision.gameObject);
        }
        else if(collision.gameObject.tag == "Item") //�浹�� ������Ʈ�� Item�϶�
        {
            Item item = collision.gameObject.GetComponent<Item>();  //Item ���� ��������
            switch (item.type) { //type ���� �̸����� ����

                case "Coin":
                    score += 1000;  //���� �߰�
                    break;
                
                case "Power":
                    if (power == maxpower)
                        score += 500;
                    else
                        power++;    
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
            Destroy(collision.gameObject);
        }
    }

    void OffEffect()
    {
        boomEffect.SetActive(false);
        isBoomTime = false;
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
