using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject[] enemyObjects; //�� ������Ʈ �迭
    public Transform[] spawnPoints;   //�� ������ ���� ��ġ�� �迭 ����

    public float maxSpawnDelay;     //�� ���� ������ ���� ����
    public float curSpawnDelay;

    public GameObject player;   //���� ������� �Ѿ��� �÷��̾ ���ϰ� �ϱ� ���� ����
                                //(�������� ���� �ö�� ���� �ʱ� ������ �̹� ���� �ö�� ������Ʈ�� ������ �Ұ��� �ϴ�.)
    
    [Header("UI")]
    public Text scoreText;      //���� �ؽ�Ʈ
    public Image[] lifeImage;   //��� �̹���
    public GameObject gameOverSet;  


    void Update()
    {
        curSpawnDelay += Time.deltaTime; 

        if(curSpawnDelay > maxSpawnDelay) //���� ���� ��� �ð��� maxSpawnDelay���� ũ�ٸ�
        {
            SpawnEnemy();
            maxSpawnDelay = Random.Range(0.5f, 3f);
            curSpawnDelay = 0; //�ʱ�ȭ
        }

        //#.UI Score Update
        Player playerLogic = player.GetComponent<Player>();
        //string format(): ������ ������� ���ڿ��� ��ȯ���ִ� �Լ�
        //{0:n0}: ���ڸ����� ��ǥ�� �����ִ� ���� ���
        scoreText.text = string.Format("{0:n0}", playerLogic.score);
        
    }

    void SpawnEnemy() //���� ����
    {
        int ranEnemy = Random.Range(0 , 3);
        int ranPoint = Random.Range(0, 9);
        GameObject enemy = Instantiate(enemyObjects[ranEnemy],
                                        spawnPoints[ranPoint].position,
                                        spawnPoints[ranPoint].rotation
                                        );

        Rigidbody2D rigid = enemy.GetComponent<Rigidbody2D>();  
        Enemy enemyLogic = enemy.GetComponent<Enemy>();     //Enemy�� ���� speed������ ����ϱ� ����

        enemyLogic.player = player; //�� ���� ���Ŀ� �÷��̾� ������ �Ѱ��ִ� ��

        //�� ����� �ӵ��� GameManager�� �����ϰ� ����
        if (ranPoint == 5 || ranPoint == 6)
        {   //Right Spawn
            enemy.transform.Rotate(Vector3.back * 90);
            rigid.velocity = new Vector2(enemyLogic.speed * (-1), -1);
        }
        else if(ranPoint == 7 || ranPoint == 8)
        {   //Left Spawn
            enemy.transform.Rotate(Vector3.forward * 90);
            rigid.velocity = new Vector2(enemyLogic.speed, -1);
        }
        else
        {   //Front Spawn
            rigid.velocity = new Vector2(0, enemyLogic.speed * (-1));
        }
    }

    //�÷��̾ �׾����� ���� �̹������� �Լ�
    public void UpdateLifeIcon(int life)
    { 
        //Image�� �ϴ� ��� ���� ���·� �ΰ�, ������ ������ ����

        //# Life Init Disable
        for (int index = 0; index < 3; index++)
        {
            lifeImage[index].color = new Color(1, 1, 1, 0);
        }
        //# Life Active
        for (int index = 0; index < life; index++)
        {
            lifeImage[index].color = new Color(1, 1, 1, 1);
        }
    }

    //������ ���� �÷��̾ �ٽ� ���ͽ�Ű�� �Լ�
    public void RespawnPlayer()
    {
        //�÷��̾� ���ʹ� �ð����� ���Ͽ� Invoke�� ���
        Invoke("RespawnPlayerExe", 2);

    }
    void RespawnPlayerExe()
    {
        //���������� �÷��̾ �ٽ� spawn�ϴ� �Լ�
        player.transform.position = Vector3.down * 3.5f;
        player.SetActive(true);

        Player playerLogic = player.GetComponent<Player>();
        playerLogic.isHit = false;
    }
    public void GameOver()
    {
        gameOverSet.SetActive(true);
    }

    public void GameRetry()
    {
        SceneManager.LoadScene(0);
    }
}
