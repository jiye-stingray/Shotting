using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject[] enemyObjects; //�� ������Ʈ �迭
    public Transform[] spawnPoints;   //�� ������ ���� ��ġ�� �迭 ����

    public float maxSpawnDelay;     //�� ���� ������ ���� ����
    public float curSpawnDelay;

    void Update()
    {
        curSpawnDelay += Time.deltaTime; 

        if(curSpawnDelay > maxSpawnDelay) //���� ���� ��� �ð��� maxSpawnDelay���� ũ�ٸ�
        {
            SpawnEnemy();
            maxSpawnDelay = Random.Range(0.5f, 3f);
            curSpawnDelay = 0; //�ʱ�ȭ
        }
        
    }

    void SpawnEnemy() //���� ����
    {
        int ranEnemy = Random.Range(0 , 3);
        int ranPoint = Random.Range(0, 5);
        Instantiate(enemyObjects[ranEnemy],
            spawnPoints[ranPoint].position,
            spawnPoints[ranPoint].rotation
            );
    }
}
