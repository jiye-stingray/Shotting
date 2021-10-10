using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;    //파일 읽기를 위한 라이브러리

public class GameManager : MonoBehaviour
{
    public string[] enemyObjects; //적 오브젝트 type
    public Transform[] spawnPoints;   //적 프리팹 생성 위치의 배열 변수

    public float nextSpawnDelay;     //적 생성 딜레이 변수 선언
    public float curSpawnDelay;

    public GameObject player;   //적군 비행기의 총알이 플레이어를 향하게 하기 위한 변수
                                //(프리팹은 씬에 올라와 있지 않기 때문에 이미 씬에 올라온 오브젝트에 접근이 불가능 하다.)
    
    [Header("UI")]
    public Text scoreText;      //점수 텍스트
    public Image[] lifeImage;   //목숨 이미지
    public Image[] BoomImage;   //폭탄 이미지
    public GameObject gameOverSet;

    public ObjectManager objectManager; // 오브젝트 풀 하기 위한 로직 불러오기

    //적군 출현에 관련된 변수
    [Header("Respawn")]
    public List<Spawn> spawnList;   //적군들 spawn 할 때 필요한 정보(Spawn) 구조체 불러온 리스트
    public int spawnIndex;      //적군들이 얼마나 소환 됐는지 체크하는 index
    public bool spawnEnd;       //플래그 변수 

    void Awake()
    {
        spawnList = new List<Spawn>();
        enemyObjects = new string[]{ "EnemyS", "EnemyM", "EnemyL" };
        ReadSpawnFile();
    }
    void Update()
    {
        curSpawnDelay += Time.deltaTime;

        //현재 스폰 대기 시간이 nextSpawnDelay보다 크다면 && 아직 스폰이 다 안 끝났다면
        if (curSpawnDelay > nextSpawnDelay && spawnEnd == false) 
        {
            SpawnEnemy();
            curSpawnDelay = 0; //초기화
        }

        //#.UI Score Update
        Player playerLogic = player.GetComponent<Player>();
        //string format(): 지정된 양식으로 문자열을 변환해주는 함수
        //{0:n0}: 세자리마다 쉼표로 나눠주는 숫자 양식
        scoreText.text = string.Format("{0:n0}", playerLogic.score);
        
    }

    //text 파일 읽기
    void ReadSpawnFile()
    {
        //#1. 적 출현의 변수 초기화
        spawnList.Clear();
        spawnIndex = 0;
        spawnEnd = false;

        //#2.리스폰 파일 읽기
        //TextAsset : 텍스트 파일 에셋 클래스
        TextAsset textFile = Resources.Load("Stage 0") as TextAsset;
        //StringReader : 파일 내의 문자열 데이터 읽기 클래스
        StringReader stringReader = new StringReader(textFile.text);
        
        //텍스트 데이터 끝 까지 다다를 때 까지 반복
        while (stringReader != null)
        {
            //ReadLine : 텍스트 데이터를 한 줄 씩 반환(자동 줄 바꿈)
            string line = stringReader.ReadLine();
            Debug.Log(line);
            if (line == null) break;

            //#.리스폰 데이터 생성
            Spawn spawnData = new Spawn();
            //Split 함수로 자르면 배열로 반환됌
            //Parse() : string형을 지정한 변수형으로 형변환
            spawnData.delay = float.Parse(line.Split(',')[0]);
            spawnData.type = line.Split(',')[1];
            spawnData.point = int.Parse(line.Split(',')[2]);
            spawnList.Add(spawnData);
        }

        //StringRender로 열어둔 파일은 작업이 끝난 후 꼭 닫기
        //#. 텍스트 파일 닫기
        stringReader.Close();

        //미리 첫번째 출연 시간을 적용
        //첫번째 스폰딜레이 적용
        nextSpawnDelay = spawnList[0].delay;
    }

    void SpawnEnemy() //적군 스폰
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
        //기존의 Instantiate를 오브젝트 풀링으로 교체
        GameObject enemy = objectManager.MakeObj(enemyObjects[enemyIndex]);
        //위치와 각도는 인스턴스 변수에서 적용
        enemy.transform.position = spawnPoints[enemyPoint].position;
        Rigidbody2D rigid = enemy.GetComponent<Rigidbody2D>();  
        Enemy enemyLogic = enemy.GetComponent<Enemy>();     //Enemy가 가진 speed변수를 사용하기 위해

        //enemy는 프리팹이라서 다른 오브젝트를 바로 받아 올 수 없다. 
        //그래서 생성 직후에  GameManager에서 넘겨준다
        enemyLogic.player = player; 
        enemyLogic.objectManager = objectManager;   

        //적 비행기 속도를 GameManager가 관리하게 수정
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

        //#.리스폰 인덱스 증가
        spawnIndex++;
        if (spawnIndex == spawnList.Count)//모든 적군을 다 소환 했을 때
        {
            spawnEnd = true;
            return;
        }
        //#.다음 스폰 딜레이 갱신
        nextSpawnDelay = spawnList[spawnIndex].delay;
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
