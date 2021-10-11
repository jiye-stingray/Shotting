using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower : MonoBehaviour
{
    public float maxShotDealy;  //�ִ� ������
    public float curShotDealy;  //���� ������

    public ObjectManager objectManager;

    public Vector3 followPos;   //������� ��ġ
    public int followDelay; 
    public Transform parent;
    public Queue<Vector3> parentPos;    //ť

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

    //folloPos ������Ʈ
    void Watch()
    {
        //#.Input Pos
        //�θ� ��ġ�� ������ ������ �������� �ʵ��� ���� �߰�
        if(!parentPos.Contains(parent.position))
            parentPos.Enqueue(parent.position);
        //#.Output Pos
        if (parentPos.Count > followDelay)
            followPos = parentPos.Dequeue();
        else if (parentPos.Count < followDelay)     //ť�� ä�� ���� ������ �θ� ��ġ ����
            followPos = parent.position;

    }
    void Follow()
    {
        transform.position = followPos;
    }

    void Fire()
    {

        if (!Input.GetButton("Fire1")) //��ư üũ
            return;

        if (curShotDealy < maxShotDealy) //���� ������ ���� �ʾ��� �� ���� (���� �ð��� ���� ���� �ʾҴ�)
            return;

        GameObject bullet = objectManager.MakeObj("BulletFollower");
        bullet.transform.position = transform.position;
        Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
        rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);

        curShotDealy = 0; //�����̺��� �ʱ�ȭ
    }

    void Reload()
    {
        curShotDealy += Time.deltaTime;
    }
}
