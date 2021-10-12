using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public string enemyname;    //���� ũ�⿡ ���� �̸�
    public int EnemyScore;      //������ ���� ���� �� �ִ� Score����
    public float speed;     //�ӵ�
    public float health;    //ü��(�� �� �¾��� �� �״���
    public Sprite[] sprites;    //spriteRender�� ����� ��

    public float maxShotDealy;  //�ִ� ������
    public float curShotDealy;  //���� ������

    public GameObject bulletObjA; //�Ѿ� ������Ʈ prefabs
    public GameObject bulletObjB;

    public GameObject ItemCoin;     //�����۵�
    public GameObject ItemPower;
    public GameObject ItemBoom;

    public GameObject player;       //GameManager ���׼� �޾ƿ�

    //������Ʈ Ǯ�� �ϱ� ���� ����ϴ� ����
    public ObjectManager objectManager; 

    SpriteRenderer spriteRenderer;  //�ǰ� ������ �� �������� �̹����� �ٲٱ�

    Animator anim; //����

    //���� ���� ���� �帧�� �ʿ��� ����
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

    //������ ���ߴ� �Լ�
    void Stop()
    {
        //������Ʈ Ǯ���� ���ؼ� �����ǰ� �ѹ�, �ٽ� Ȱ��ȭ �Ǿ��� �� �ѹ�. �� 2���� ȣ�� �Ǳ� ������ �װ��� ����
        if (!gameObject.activeSelf)
            return;
        Rigidbody2D rigid = GetComponent<Rigidbody2D>();
        rigid.velocity = Vector2.zero;  //�ӵ� ���߱�

        Invoke("Think", 2);
    }

    //������ �����ϴ� �Լ�
    void Think()
    { 
        //���� 4��
        patternIndex = patternIndex == 3 ? 0 : patternIndex + 1;
        curPatternCount = 0;        //���� ����� ���� ���� Ƚ�� ���� �ʱ�ȭ
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

    //������ 4�� �߻�
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
        //�� ���Ϻ� Ƚ���� �����ϰ� ���� �������� �Ѿ�� ����
        if (curPatternCount < maxPatternCount[patternIndex]) //���� ���� ���� Ƚ���� ������ �ִ� ���� Ƚ������ ���ٸ�
            Invoke("FireFoward", 2); //���� �������� �Ѿ�� ���� �ƴ� �ٽ� ����
        else
            Invoke("Think", 2);

    }

    //�÷��̾� �������� ����
    void FireShot()
    {
        if (health <= 0)
            return;
        for (int i = 0; i < 5; i++)
        {
            GameObject bullet = objectManager.MakeObj("BulletEnemyB");
            bullet.transform.position = transform.position;


            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
            //��ǥ���� ���� = ��ǥ�� ��ġ - �ڽ��� ��ġ
            Vector2 dirVec = player.transform.position - transform.position;
            //��ġ�� ��ġ�� �ʰ� ���� ���͸� ���Ͽ� ����
            Vector2 ranVec = new Vector2(Random.Range(-0.5f, 0.5f), Random.Range(0f, 2f));
            dirVec += ranVec;
            rigid.AddForce(dirVec.normalized * 3, ForceMode2D.Impulse);
        }
        
        curPatternCount++;
        //�� ���Ϻ� Ƚ���� �����ϰ� ���� �������� �Ѿ�� ����
        if (curPatternCount < maxPatternCount[patternIndex]) //���� ���� ���� Ƚ���� ������ �ִ� ���� Ƚ������ ���ٸ�
            Invoke("FireShot", 3.5f); //���� �������� �Ѿ�� ���� �ƴ� �ٽ� ����
        else
            Invoke("Think", 2);

    }

    //��ä������� �߻�
    void FireArc()
    {
        if (health <= 0)
            return;
       
        //#.Fire Arc Continue Fire
        GameObject bullet = objectManager.MakeObj("BulletEnemyA");
        bullet.transform.position = transform.position;
        bullet.transform.rotation = Quaternion.identity;


        Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
        //Mathf.Sin() : �ﰢ�Լ�
        //Mathf.Cos() : ���� ������ �ٸ� ��, �߻� ����� ����
        //Mathf.PI() : ����
        //���� �ѷ����� ���� �� �� �� ������ ������ �׸�
        Vector2 dirVec = new Vector2(Mathf.Cos(Mathf.PI *10* curPatternCount/ maxPatternCount[patternIndex]),-1);
        rigid.AddForce(dirVec.normalized * 5, ForceMode2D.Impulse);



        curPatternCount++;
        //�� ���Ϻ� Ƚ���� �����ϰ� ���� �������� �Ѿ�� ����
        if (curPatternCount < maxPatternCount[patternIndex]) //���� ���� ���� Ƚ���� ������ �ִ� ���� Ƚ������ ���ٸ�
            Invoke("FireArc", 0.2f); //���� �������� �Ѿ�� ���� �ƴ� �ٽ� ����
        else
            Invoke("Think", 2);

    }

    //�� ������ ��ü ����
    void FireAround()
    {
        if (health <= 0)
            return;

        //#.Fire Around
        int roundNumA = 40;
        int roundNumB = 30;
        //���� Ƚ���� ���� �����Ǵ� �Ѿ� ���� ������ ���̵� ���
        int roundNum = curPatternCount % 2 == 0? roundNumA : roundNumB;
        for (int i = 0; i < roundNumA; i++)
        {
            GameObject bullet = objectManager.MakeObj("BulletBossB");
            bullet.transform.position = transform.position;
            bullet.transform.rotation = Quaternion.identity;


            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
            //���� �ѷ��� PI * 2
            //�����Ǵ� �Ѿ��� ������ Ȱ���Ͽ� �ٲ�
            Vector2 dirVec = new Vector2(Mathf.Cos(Mathf.PI * 2 * i / roundNum)
                                        ,Mathf.Sin(Mathf.PI * 2 * i / roundNum));
            rigid.AddForce(dirVec.normalized * 2, ForceMode2D.Impulse);

            //�Ѿ��� ����Ű�� ���⿡ ���� ȸ���ϰ� �����
            Vector3 rotVec = Vector3.forward * 360 * i / roundNum + Vector3.forward * 90;
            bullet.transform.Rotate(rotVec);
        }
        

        curPatternCount++;
        //�� ���Ϻ� Ƚ���� �����ϰ� ���� �������� �Ѿ�� ����
        if (curPatternCount < maxPatternCount[patternIndex]) //���� ���� ���� Ƚ���� ������ �ִ� ���� Ƚ������ ���ٸ�
            Invoke("FireAround", 0.7f); //���� �������� �Ѿ�� ���� �ƴ� �ٽ� ����
        else
            Invoke("Think", 2);

    }
    void Fire()
    {   
        if (curShotDealy < maxShotDealy) //���� ������ ���� �ʾ��� �� ���� (���� �ð��� ���� ���� �ʾҴ�)
            return;

        if(enemyname == "S") //enemyname�̶�� ������ ������ ���ݹ�� ���ϱ�
        {
            GameObject bullet = objectManager.MakeObj("BulletEnemyA");
            bullet.transform.position = transform.position;
            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
            
            //��ǥ���� ���� = ��ǥ�� ��ġ - �ڽ��� ��ġ
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

            //��ǥ���� ���� = ��ǥ�� ��ġ - �ڽ��� ��ġ
            Vector3 dirVecR = player.transform.position - (transform.position + Vector3.right * 0.3f);
            Vector3 dirVecL = player.transform.position - (transform.position + Vector3.left * 0.3f);

            rigidR.AddForce(dirVecR.normalized * 4, ForceMode2D.Impulse);
            rigidL.AddForce(dirVecL.normalized * 4, ForceMode2D.Impulse);
        }

        curShotDealy = 0; //�����̺��� �ʱ�ȭ
    }

    void Reload()
    {
        curShotDealy += Time.deltaTime;
    }

   

    public void OnHit(int dmg) //�÷��̾ �� �Ѿ��� �¾��� �� ������
    {
        //�������� �������� ��������� �ʵ��� ���� ó��
        if (health <= 0)
            return;
        health -= dmg;
        if (enemyname == "B")
        {
            anim.SetTrigger("OnHit");
        }
        else
        {
            spriteRenderer.sprite = sprites[1]; //��� ��������Ʈ�� 0, �ǰݽ� ��������Ʈ�� 1
            Invoke("ReturnSprite", 0.1f);
        }
            
        

        if (health <= 0) // ü���� 0 ���� �϶�
        {
            Player playerlogic = player.GetComponent<Player>();
            playerlogic.score += EnemyScore;    //�÷��̾��� ������ �ڽ��� ������ �־���

            //#. Random Ratio Item Drop
            int ran = enemyname == "B" ? 0 : Random.Range(0, 10);
            //���� ���ڸ� �̿��Ͽ� Item ���� �ۼ�
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
            //Destroy()�� false�� ��ü
            gameObject.SetActive(false);

            //Quaternion.identity : �⺻ ȸ�� �� 0
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

            //Quaternion.identity : �⺻ ȸ�� �� 0
            transform.rotation = Quaternion.identity;
        }
        else if (collision.gameObject.tag == "PlayerBullet") //�÷��̾��� �Ѿ˿� �¾��� �� 
        {

            Bullet bullet = collision.gameObject.GetComponent<Bullet>();
            OnHit(bullet.dmg); //�Ѿ��� ������ ��ŭ ü���� ����

            collision.gameObject.SetActive(false);  //�ǰݽ� �÷��̾��� �Ѿ˵� ��Ȱ��ȭ
        }
       
    }
}
