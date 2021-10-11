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
    public ObjectManager objectManager; //������Ʈ Ǯ�� ����� ���� ����

    public GameObject boomEffect;   //��ź ������ �� effect

    public bool isHit;      //������ ���� ���¿��� �� ���� ���ϰ� �ϱ�
    public bool isBoomTime; //��ź�� ������ �ִ� ��Ȳ

    public GameObject[] followers;      //��������迭

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
                GameObject bullet = objectManager.MakeObj("BulletPlayerA");
                bullet.transform.position = transform.position;
                Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
                rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse); 
                break;
            case 2:
                //+ transform�� ������ Vector3
                //���� ��ġ���� ���� ���� ���ϱ�
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
            collision.gameObject.SetActive(false);
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
