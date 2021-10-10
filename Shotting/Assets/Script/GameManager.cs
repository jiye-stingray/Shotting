using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;    //���� �б⸦ ���� ���̺귯��

public class GameManager : MonoBehaviour
{
    public string[] enemyObjects; //�� ������Ʈ type
    public Transform[] spawnPoints;   //�� ������ ���� ��ġ�� �迭 ����

    public float nextSpawnDelay;     //�� ���� ������ ���� ����
    public float curSpawnDelay;

    public GameObject player;   //���� ������� �Ѿ��� �÷��̾ ���ϰ� �ϱ� ���� ����
                                //(�������� ���� �ö�� ���� �ʱ� ������ �̹� ���� �ö�� ������Ʈ�� ������ �Ұ��� �ϴ�.)
    
    [Header("UI")]
    public Text scoreText;      //���� �ؽ�Ʈ
    public Image[] lifeImage;   //��� �̹���
    public Image[] BoomImage;   //��ź �̹���
    public GameObject gameOverSet;

    public ObjectManager objectManager; // ������Ʈ Ǯ �ϱ� ���� ���� �ҷ�����

    //���� ������ ���õ� ����
    [Header("Respawn")]
    public List<Spawn> spawnList;   //������ spawn �� �� �ʿ��� ����(Spawn) ����ü �ҷ��� ����Ʈ
    public int spawnIndex;      //�������� �󸶳� ��ȯ �ƴ��� üũ�ϴ� index
    public bool spawnEnd;       //�÷��� ���� 

    void Awake()
    {
        spawnList = new List<Spawn>();
        enemyObjects = new string[]{ "EnemyS", "EnemyM", "EnemyL" };
        ReadSpawnFile();
    }
    void Update()
    {
        curSpawnDelay += Time.deltaTime;

        //���� ���� ��� �ð��� nextSpawnDelay���� ũ�ٸ� && ���� ������ �� �� �����ٸ�
        if (curSpawnDelay > nextSpawnDelay && spawnEnd == false) 
        {
            SpawnEnemy();
            curSpawnDelay = 0; //�ʱ�ȭ
        }

        //#.UI Score Update
        Player playerLogic = player.GetComponent<Player>();
        //string format(): ������ ������� ���ڿ��� ��ȯ���ִ� �Լ�
        //{0:n0}: ���ڸ����� ��ǥ�� �����ִ� ���� ���
        scoreText.text = string.Format("{0:n0}", playerLogic.score);
        
    }

    //text ���� �б�
    void ReadSpawnFile()
    {
        //#1. �� ������ ���� �ʱ�ȭ
        spawnList.Clear();
        spawnIndex = 0;
        spawnEnd = false;

        //#2.������ ���� �б�
        //TextAsset : �ؽ�Ʈ ���� ���� Ŭ����
        TextAsset textFile = Resources.Load("Stage 0") as TextAsset;
        //StringReader : ���� ���� ���ڿ� ������ �б� Ŭ����
        StringReader stringReader = new StringReader(textFile.text);
        
        //�ؽ�Ʈ ������ �� ���� �ٴٸ� �� ���� �ݺ�
        while (stringReader != null)
        {
            //ReadLine : �ؽ�Ʈ �����͸� �� �� �� ��ȯ(�ڵ� �� �ٲ�)
            string line = stringReader.ReadLine();
            Debug.Log(line);
            if (line == null) break;

            //#.������ ������ ����
            Spawn spawnData = new Spawn();
            //Split �Լ��� �ڸ��� �迭�� ��ȯ��
            //Parse() : string���� ������ ���������� ����ȯ
            spawnData.delay = float.Parse(line.Split(',')[0]);
            spawnData.type = line.Split(',')[1];
            spawnData.point = int.Parse(line.Split(',')[2]);
            spawnList.Add(spawnData);
        }

        //StringRender�� ����� ������ �۾��� ���� �� �� �ݱ�
        //#. �ؽ�Ʈ ���� �ݱ�
        stringReader.Close();

        //�̸� ù��° �⿬ �ð��� ����
        //ù��° ���������� ����
        nextSpawnDelay = spawnList[0].delay;
    }

    void SpawnEnemy() //���� ����
    {
        int enemyIndex = 0;
        switch (spawnList[spawnIndex].type)
        {
            case "S":
                enemyIndex = 0;
                break;
            case "M":
                enemyIndex = 1;
                break;
            case "L":
                enemyIndex = 2;
                break;
            default:
                break;
        }

        int enemyPoint = spawnList[spawnIndex].point;
        //������ Instantiate�� ������Ʈ Ǯ������ ��ü
        GameObject enemy = objectManager.MakeObj(enemyObjects[enemyIndex]);
        //��ġ�� ������ �ν��Ͻ� �������� ����
        enemy.transform.position = spawnPoints[enemyPoint].position;
        Rigidbody2D rigid = enemy.GetComponent<Rigidbody2D>();  
        Enemy enemyLogic = enemy.GetComponent<Enemy>();     //Enemy�� ���� speed������ ����ϱ� ����

        //enemy�� �������̶� �ٸ� ������Ʈ�� �ٷ� �޾� �� �� ����. 
        //�׷��� ���� ���Ŀ�  GameManager���� �Ѱ��ش�
        enemyLogic.player = player; 
        enemyLogic.objectManager = objectManager;   

        //�� ����� �ӵ��� GameManager�� �����ϰ� ����
        if (enemyPoint == 5 || enemyPoint == 6)
        {   //Right Spawn
            enemy.transform.Rotate(Vector3.back * 90);
            rigid.velocity = new Vector2(enemyLogic.speed * (-1), -1);
        }
        else if(enemyPoint == 7 || enemyPoint == 8)
        {   //Left Spawn
            enemy.transform.Rotate(Vector3.forward * 90);
            rigid.velocity = new Vector2(enemyLogic.speed, -1);
        }
        else
        {   //Front Spawn
            rigid.velocity = new Vector2(0, enemyLogic.speed * (-1));
        }

        //#.������ �ε��� ����
        spawnIndex++;
        if (spawnIndex == spawnList.Count)//��� ������ �� ��ȯ ���� ��
        {
            spawnEnd = true;
            return;
        }
        //#.���� ���� ������ ����
        nextSpawnDelay = spawnList[spawnIndex].delay;
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

    //��ź UI 
    public void UpdateBoomIcon(int boom)
    {
        //Image�� �ϴ� ��� ���� ���·� �ΰ�, ������ ������ ����

        //# Boom Init Disable
        for (int index = 0; index < 3; index++)
        {
            BoomImage[index].color = new Color(1, 1, 1, 0);
        }
        //# Boom Active
        for (int index = 0; index < boom; index++)
        {
            BoomImage[index].color = new Color(1, 1, 1, 1);
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
        gameOverSet.SetActive(true);    //GameOver UI Set
    }

    public void GameRetry()
    {
        SceneManager.LoadScene(0);
    }
}
