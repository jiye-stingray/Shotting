using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower : MonoBehaviour
{
    public float maxShotDealy;  //최대 딜레이
    public float curShotDealy;  //현재 딜레이

    public ObjectManager objectManager;

    public Vector3 followPos;   //따라오는 위치
    public int followDelay; 
    public Transform parent;
    public Queue<Vector3> parentPos;    //큐

    void Awake()
    {
        parentPos = new Queue<Vector3>();
    }

    void Update()
    {
        Watch();    
        Follow();
        Fire();
        Reload();
    }

    //folloPos 업데이트
    void Watch()
    {
        //#.Input Pos
        //부모 위치가 가만히 있으면 저장하지 않도록 조건 추가
        if(!parentPos.Contains(parent.position))
            parentPos.Enqueue(parent.position);
        //#.Output Pos
        if (parentPos.Count > followDelay)
            followPos = parentPos.Dequeue();
        else if (parentPos.Count < followDelay)     //큐가 채워 지기 전까진 부모 위치 적용
            followPos = parent.position;

    }
    void Follow()
    {
        transform.position = followPos;
    }

    void Fire()
    {

        if (!Input.GetButton("Fire1")) //버튼 체크
            return;

        if (curShotDealy < maxShotDealy) //아직 장전이 되지 않았을 때 리턴 (장전 시간이 충족 되지 않았다)
            return;

        GameObject bullet = objectManager.MakeObj("BulletFollower");
        bullet.transform.position = transform.position;
        Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
        rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);

        curShotDealy = 0; //딜레이변수 초기화
    }

    void Reload()
    {
        curShotDealy += Time.deltaTime;
    }
}
