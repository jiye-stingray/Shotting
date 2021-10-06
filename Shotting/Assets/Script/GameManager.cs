using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public string[] enemyObjects; //적 오브젝트 type
    public Transform[] spawnPoints;   //적 프리팹 생성 위치의 배열 변수

    public float maxSpawnDelay;     //적 생성 딜레이 변수 선언
    public float curSpawnDelay;

    public GameObject player;   //적군 비행기의 총알이 플레이어를 향하게 하기 위한 변수
                                //(프리팹은 씬에 올라와 있지 않기 때문에 이미 씬에 올라온 오브젝트에 접근이 불가능 하다.)
    
    [Header("UI")]
    public Text scoreText;      //점수 텍스트
    public Image[] lifeImage;   //목숨 이미지
    public Image[] BoomImage;   //폭탄 이미지
    public GameObject gameOverSet;

    public ObjectManager objectManager; // 오브젝트 풀 하기 위한 로직 불러오기

    void Awake()
    {
        enemyObjects = new string[]{ "EnemyL", "EnemyM", "EnemyS" };
    }
    void Update()
    {
        curSpawnDelay += Time.deltaTime; 

        if(curSpawnDelay > maxSpawnDelay) //현재 스폰 대기 시간이 maxSpawnDelay보다 크다면
        {
            SpawnEnemy();
            maxSpawnDelay = Random.Range(0.5f, 3f);
            curSpawnDelay = 0; //초기화
        }

        //#.UI Score Update
        Player playerLogic = player.GetComponent<Player>();
        //string format(): 지정된 양식으로 문자열을 변환해주는 함수
        //{0:n0}: 세자리마다 쉼표로 나눠주는 숫자 양식
        scoreText.text = string.Format("{0:n0}", playerLogic.score);
        
    }

    void SpawnEnemy() //적군 스폰
    {
        int ranEnemy = Random.Range(0 , 3);
        int ranPoint = Random.Range(0, 9);
        //기존의 Instantiate를 오브젝트 풀링으로 교체
        GameObject enemy = objectManager.MakeObj(enemyObjects[ranEnemy]);
        //위치와 각도는 인스턴스 변수에서 적용
        enemy.transform.position = spawnPoints[ranPoint].position;
        Rigidbody2D rigid = enemy.GetComponent<Rigidbody2D>();  
        Enemy enemyLogic = enemy.GetComponent<Enemy>();     //Enemy가 가진 speed변수를 사용하기 위해

        //enemy는 프리팹이라서 다른 오브젝트를 바로 받아 올 수 없다. 
        //그래서 생성 직후에  GameManager에서 넘겨준다
        enemyLogic.player = player; 
        enemyLogic.objectManager = objectManager;   

        //적 비행기 속도를 GameManager가 관리하게 수정
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

    //플레이어가 죽었을때 생명 이미지관리 함수
    public void UpdateLifeIcon(int life)
    { 
        //Image를 일단 모두 투명 상태로 두고, 목숨대로 반투명 설정

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

    //폭탄 UI 
    public void UpdateBoomIcon(int boom)
    {
        //Image를 일단 모두 투명 상태로 두고, 목숨대로 반투명 설정

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

    //공격을 맞은 플레이어를 다시 복귀시키는 함수
    public void RespawnPlayer()
    {
        //플레이어 복귀는 시간차를 위하여 Invoke를 사용
        Invoke("RespawnPlayerExe", 2);

    }
    void RespawnPlayerExe()
    {
        //실질적으로 플레이어를 다시 spawn하는 함수
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
