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

    SpriteRenderer spriteRenderer;  //�ǰ� ������ �� �������� �̹����� �ٲٱ�

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

    }

    void Update()
    {
        Fire();
        Reload();
    }
    void Fire()
    {   
        if (curShotDealy < maxShotDealy) //���� ������ ���� �ʾ��� �� ���� (���� �ð��� ���� ���� �ʾҴ�)
            return;

        if(enemyname == "S") //enemyname�̶�� ������ ������ ���ݹ�� ���ϱ�
        {
            GameObject bullet = Instantiate(bulletObjA, transform.position, transform.rotation);
            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
            
            //��ǥ���� ���� = ��ǥ�� ��ġ - �ڽ��� ��ġ
            Vector3 dirVec = player.transform.position - transform.position;
            rigid.AddForce(dirVec.normalized*3, ForceMode2D.Impulse);
        }
        else if(enemyname == "L")
        {
            GameObject bulletR = Instantiate(bulletObjB, transform.position + Vector3.right * 0.3f, transform.rotation);
            GameObject bulletL = Instantiate(bulletObjB, transform.position + Vector3.left * 0.3f, transform.rotation);

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
        spriteRenderer.sprite = sprites[1]; //��� ��������Ʈ�� 0, �ǰݽ� ��������Ʈ�� 1
        Invoke("ReturnSprite",0.1f);

        if (health <= 0) // ü���� 0 ���� �϶�
        {
            Player playerlogic = player.GetComponent<Player>();
            playerlogic.score += EnemyScore;    //�÷��̾��� ������ �ڽ��� ������ �־���


            //#. Random Ratio Item Drop
            int ran = Random.Range(0, 10);
            //���� ���ڸ� �̿��Ͽ� Item ���� �ۼ�
            if (ran < 3)    //Not Item 30%
            {
                Debug.Log("Not Item");
            }
            else if (ran < 6)   //Coin 30%
            {
                Instantiate(ItemCoin, transform.position, ItemCoin.transform.rotation);
            }
            else if (ran < 8)   //Power 20%
            {
                Instantiate(ItemPower, transform.position, ItemPower.transform.rotation);
            }
            else if (ran < 10)   //Boom 20%
            {
                Instantiate(ItemBoom, transform.position, ItemBoom.transform.rotation);
            }
            

            Destroy(gameObject); 
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
        else if (collision.gameObject.tag == "PlayerBullet") //�÷��̾��� �Ѿ˿� �¾��� �� 
        {

            Bullet bullet = collision.gameObject.GetComponent<Bullet>();
            OnHit(bullet.dmg); //�Ѿ��� ������ ��ŭ ü���� ����

            Destroy(collision.gameObject);  //�ǰݽ� �÷��̾��� �Ѿ˵� ����
        }
       
    }
}
