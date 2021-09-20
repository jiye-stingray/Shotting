using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject[] enemyObjects; //적 오브젝트 배열
    public Transform[] spawnPoints;   //적 프리팹 생성 위치의 배열 변수

    public float maxSpawnDelay;     //적 생성 딜레이 변수 선언
    public float curSpawnDelay;

    void Update()
    {
        curSpawnDelay += Time.deltaTime; 

        if(curSpawnDelay > maxSpawnDelay) //현재 스폰 대기 시간이 maxSpawnDelay보다 크다면
        {
            SpawnEnemy();
            maxSpawnDelay = Random.Range(0.5f, 3f);
            curSpawnDelay = 0; //초기화
        }
        
    }

    void SpawnEnemy() //적군 스폰
    {
        int ranEnemy = Random.Range(0 , 3);
        int ranPoint = Random.Range(0, 5);
        Instantiate(enemyObjects[ranEnemy],
            spawnPoints[ranPoint].position,
            spawnPoints[ranPoint].rotation
            );
    }
}
